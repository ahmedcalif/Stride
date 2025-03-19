using Microsoft.EntityFrameworkCore;
using Stride.Data.DatabaseModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Stride.Data.Data
{
    public class ApplicationDBContext : IdentityDbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
            : base(options)
        {
        }

      

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
             
           
           
        }

        public DbSet<User> Users{ get; set; }
        public DbSet<UserSetting> UserSettings { get; set; }

        public DbSet<UserAchievements> UserAchievements { get; set;}
        public DbSet<Habit> Habits { get; set; }
        public DbSet<Goal> Goals { get; set; } 

        public DbSet<GoalPriority> GoalPrioritiy { get; set; }

        public DbSet<Achievement> Achievements { get; set;}

        public DbSet<AchievementType> AchievementType { get; set; }

        public DbSet<HabitFrequency> HabitFrequency { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        public DbSet<NotificationType> NotificationType { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Theme> Theme { get; set; }

        


    }
}