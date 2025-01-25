using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Api.Models;

namespace TaskManagementSystem.Api.Data
{
    // The AppDbContext class represents the session with the database 
    // and provides access to the database tables, including users and tasks.
    // It inherits from IdentityDbContext, which includes user authentication 
    // and authorization features.
    public class AppDbContext : IdentityDbContext<DBUser, IdentityRole<int>, int>
    {
        // The constructor passes the DbContextOptions to the base class
        // for configuration purposes (e.g., connection string, database options).
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // DbSet represents the "Tasks" table in the database. Each task in this 
        // table will be assigned to a user, and we store the title, description, 
        // status, and user ID.
        public DbSet<DBTask> Tasks { get; set; }

        // OnModelCreating allows customizing the model configuration.
        // Here we simply call the base implementation to handle Identity-related configuration.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        // SeedTasksToUser is a helper method that seeds tasks to a user 
        // if no tasks have been assigned to them yet.
        public async Task SeedTasksToUser(int userId)
        {
            // Check if any tasks already exist for the given user. 
            // If tasks exist, there's no need to seed new ones.
            if (Tasks.Any(t => t.AssignedUserId == userId))
            {
                return; // Exit early if tasks for the user are already in the database.
            }

            // If no tasks are found for the user, add predefined tasks.
            var tasks = new List<DBTask>
            {
                // Define three tasks for the user with different statuses.
                new DBTask { Title = "Task 1", Description = "First Task", Status = "Pending", AssignedUserId = userId },
                new DBTask { Title = "Task 2", Description = "Second Task", Status = "In Progress", AssignedUserId = userId },
                new DBTask { Title = "Task 3", Description = "Third Task", Status = "Completed", AssignedUserId = userId }
            };

            // Add the tasks to the "Tasks" table.
            Tasks.AddRange(tasks);

            // Save the changes to the database.
            await SaveChangesAsync();
        }
    }
}
