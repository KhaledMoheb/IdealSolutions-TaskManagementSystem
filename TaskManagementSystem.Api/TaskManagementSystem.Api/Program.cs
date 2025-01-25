using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Diagnostics;
using TaskManagementSystem.Api.Repositories;
using TaskManagementSystem.Api.Data;
using TaskManagementSystem.Api.Helpers;
using TaskManagementSystem.Api.Models;
using TaskManagementSystem.Api.RepositoryContracts;
using TaskManagementSystem.Api.Services;
using TaskManagementSystem.Api.ServicesContracts;
using Microsoft.Extensions.Options;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Task Management API",
        Version = "v1",
        Description = "API for Task Management, including user login and authentication."
    });
    // Optionally, enable XML comments for Swagger documentation
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    // Add JWT support
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your token.",
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});


// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontendLocalhost", builder =>
    {
        builder.WithOrigins("http://localhost:4200") // Allow the Angular app
               .AllowAnyHeader() // Allow any headers
               .AllowAnyMethod() // Allow any HTTP methods
               .AllowCredentials(); // Allow credentials
    });
});

// Add InMemory database with Identity
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("TaskManagementDb"));

// Add Identity services
builder.Services.AddIdentity<DBUser, IdentityRole<int>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Register custom services
builder.Services.AddScoped<IJwtHelper, JwtHelper>();  // Add your custom JwtHelper here
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();

// Add authentication (JWT)
builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
        };
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireClaim("role", "Admin"));
    options.AddPolicy("UserPolicy", policy => policy.RequireClaim("role", "User"));
});

builder.Services.AddAuthentication("CookieAuthentication")
    .AddCookie("CookieAuthentication", options =>
    {
        options.Cookie.Name = "authToken"; // Name of the cookie
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Use HTTPS
        options.LoginPath = "/Login/login"; // Redirect to login if unauthenticated
        options.AccessDeniedPath = "/Login/AccessDenied"; // Redirect if unauthorized
    });

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = context =>
    {
        // Redirect to /Login/login instead of returning 401
        context.Response.Redirect("/Login/login");
        return Task.CompletedTask;
    };
    options.Events.OnRedirectToAccessDenied = context =>
    {
        // Redirect to /Login/login instead of returning 403
        context.Response.Redirect("/Login/login");
        return Task.CompletedTask;
    };

    // Optionally set the default login path
    options.LoginPath = "/Login/login";
    options.AccessDeniedPath = "/Login/login";
});

var app = builder.Build();

// Apply CORS policy to the request pipeline
app.UseCors("AllowFrontendLocalhost");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Seed users data
using (var scope = app.Services.CreateScope())
{
    AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var userService = scope.ServiceProvider.GetRequiredService<UserService>();
    await userService.SeedUsersAsync();

    UserManager<DBUser> _userManager = scope.ServiceProvider.GetRequiredService<UserManager<DBUser>>();
    DBUser admin = await _userManager.FindByNameAsync("admin");
    DBUser user = await _userManager.FindByNameAsync("user1");
    Debug.WriteLine("admin.PasswordHash"+ admin.PasswordHash);
    Debug.WriteLine("user.PasswordHash" + user.PasswordHash);
    await dbContext.SeedTasksToUser(user!.Id);

    // Get all tasks and print to console
    var tasks = dbContext.Tasks.ToList();
    foreach (var task in tasks)
    {
        Console.WriteLine($"Task: {task.Title} - {task.Status}");
    }
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
