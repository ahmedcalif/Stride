using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Stride.Data.DatabaseModels;

namespace Stride.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder); {}
      

         
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
