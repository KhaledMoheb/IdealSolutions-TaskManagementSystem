using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Api.Models;

namespace TaskManagementSystem.Api.Data
{
    public class AppDbContext : IdentityDbContext<DBUser, IdentityRole<int>, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<DBTask> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public async Task SeedTasksToUser(int userId)
        {
            // Check if tasks already exist for the user
            if (Tasks.Any(t => t.AssignedUserId == userId))
            {
                return; // Tasks already exist for this user; no need to seed
            }

            // Add new tasks for the user
            var tasks = new List<DBTask>
            {
                new DBTask { Title = "Task 1", Description = "First Task", Status = "Pending", AssignedUserId = userId },
                new DBTask { Title = "Task 2", Description = "Second Task", Status = "In Progress", AssignedUserId = userId },
                new DBTask { Title = "Task 3", Description = "Third Task", Status = "Completed", AssignedUserId = userId }
            };

            Tasks.AddRange(tasks);

            // Save changes to the database
            await SaveChangesAsync();
        }
    }
}
