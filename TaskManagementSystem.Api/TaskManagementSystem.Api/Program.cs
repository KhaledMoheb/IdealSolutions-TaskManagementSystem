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

// Add Swagger for API documentation and testing.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Task Management API", // API title
        Version = "v1", // API version
        Description = "API for Task Management, including user login and authentication." // API description
    });

    // Include XML comments for Swagger UI (optional).
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    // Add JWT Bearer authentication support in Swagger.
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
                    Id = "Bearer" // The ID used for bearer token authorization
                }
            },
            new string[] { }
        }
    });
});

// Configure CORS policy to allow frontend requests from localhost:4200 (Angular app).
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontendLocalhost", builder =>
    {
        builder.WithOrigins("http://localhost:4200") // Allow the Angular app
               .AllowAnyHeader() // Allow any headers
               .AllowAnyMethod() // Allow any HTTP methods
               .AllowCredentials(); // Allow credentials like cookies or authorization headers
    });
});

// Add In-Memory database with Identity for user management.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("TaskManagementDb")); // Use in-memory database for testing

// Configure Identity for user management (DBUser entity and roles).
builder.Services.AddIdentity<DBUser, IdentityRole<int>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Register custom services for JWT handling and user/task management.
builder.Services.AddScoped<IJwtHelper, JwtHelper>();  // Custom JWT helper for token generation and validation
builder.Services.AddScoped<UserService>(); // User service to handle user-related operations
builder.Services.AddScoped<ITaskService, TaskService>(); // Task service to handle task-related operations
builder.Services.AddScoped<ITaskRepository, TaskRepository>(); // Repository for task operations

// Add authentication (JWT) configuration.
builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // Allow non-HTTPS for testing (ensure to use HTTPS in production)
        options.SaveToken = true; // Save token in request headers
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true, // Validate the issuer of the token
            ValidateAudience = true, // Validate the audience of the token
            ValidateIssuerSigningKey = true, // Validate the signing key of the token
            ValidIssuer = builder.Configuration["Jwt:Issuer"], // Issuer of the token
            ValidAudience = builder.Configuration["Jwt:Audience"], // Audience for the token
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"])) // Key used to sign the token
        };
    });

// Add authorization policies based on user roles (Admin, User).
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireClaim("role", "Admin"));
    options.AddPolicy("UserPolicy", policy => policy.RequireClaim("role", "User"));
});

// Configure Cookie Authentication (optional alongside JWT).
builder.Services.AddAuthentication("CookieAuthentication")
    .AddCookie("CookieAuthentication", options =>
    {
        options.Cookie.Name = "authToken"; // Cookie name for session token
        options.Cookie.HttpOnly = true; // Prevent JavaScript from accessing the cookie
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Enforce HTTPS for cookie security
        options.LoginPath = "/Login/login"; // Redirect path for login
        options.AccessDeniedPath = "/Login/AccessDenied"; // Redirect path for unauthorized access
    });

// Configure application cookie options for login redirection.
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.Redirect("/Login/login"); // Redirect to login if unauthenticated
        return Task.CompletedTask;
    };
    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.Redirect("/Login/login"); // Redirect to login if access denied
        return Task.CompletedTask;
    };

    // Default login path
    options.LoginPath = "/Login/login";
    options.AccessDeniedPath = "/Login/login";
});

var app = builder.Build();

// Apply CORS policy to the request pipeline to allow frontend access.
app.UseCors("AllowFrontendLocalhost");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Enable Swagger UI in development environment
    app.UseSwaggerUI(); // Add Swagger UI to view API documentation
}

// Seed the database with initial data for users and tasks (only for development/testing purposes).
using (var scope = app.Services.CreateScope())
{
    AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var userService = scope.ServiceProvider.GetRequiredService<UserService>();
    await userService.SeedUsersAsync(); // Seed user data (admin and user)

    UserManager<DBUser> _userManager = scope.ServiceProvider.GetRequiredService<UserManager<DBUser>>();
    DBUser admin = await _userManager.FindByNameAsync("admin");
    DBUser user = await _userManager.FindByNameAsync("user1");
    await dbContext.SeedTasksToUser(user!.Id); // Seed tasks to user1

    // Log all tasks to the console (for debugging purposes).
    var tasks = dbContext.Tasks.ToList();
    foreach (var task in tasks)
    {
        Console.WriteLine($"Task: {task.Title} - {task.Status}");
    }
}

app.UseAuthentication(); // Enable authentication middleware
app.UseAuthorization(); // Enable authorization middleware

// Map controller endpoints.
app.MapControllers();

app.Run(); // Start the application
