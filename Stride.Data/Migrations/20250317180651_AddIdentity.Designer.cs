﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Stride.Data.Data;

#nullable disable

namespace Stride.Migrations
{
    [DbContext(typeof(ApplicationDBContext))]
    [Migration("20250317180651_AddIdentity")]
    partial class AddIdentity
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("longtext");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("longtext");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("longtext");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("longtext");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("longtext");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("longtext");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("longtext");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("longtext");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("longtext");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("RoleId")
                        .HasColumnType("varchar(255)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Value")
                        .HasColumnType("longtext");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("Stride.Data.DatabaseModels.Achievement", b =>
                {
                    b.Property<int>("achievement_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("achievement_id"));

                    b.Property<int>("achievement_type_id")
                        .HasColumnType("int");

                    b.Property<DateTime>("created_at")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("description")
                        .HasColumnType("longtext");

                    b.Property<string>("icon_url")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool>("is_active")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("requirements")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int?>("user_id")
                        .HasColumnType("int");

                    b.HasKey("achievement_id");

                    b.HasIndex("achievement_type_id");

                    b.HasIndex("user_id");

                    b.ToTable("Achievements");
                });

            modelBuilder.Entity("Stride.Data.DatabaseModels.AchievementType", b =>
                {
                    b.Property<int>("achievement_type_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("achievement_type_id"));

                    b.Property<string>("description")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.HasKey("achievement_type_id");

                    b.ToTable("AchievementType");
                });

            modelBuilder.Entity("Stride.Data.DatabaseModels.Category", b =>
                {
                    b.Property<int>("category_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("category_id"));

                    b.Property<string>("description")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("category_id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("Stride.Data.DatabaseModels.Goal", b =>
                {
                    b.Property<int>("goal_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("goal_id"));

                    b.Property<int>("category_id")
                        .HasColumnType("int");

                    b.Property<string>("description")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("end_date")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("goal_priority_id")
                        .HasColumnType("int");

                    b.Property<bool>("is_completed")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime>("start_date")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("title")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("user_id")
                        .HasColumnType("int");

                    b.HasKey("goal_id");

                    b.HasIndex("category_id");

                    b.HasIndex("goal_priority_id");

                    b.HasIndex("user_id");

                    b.ToTable("Goals");
                });

            modelBuilder.Entity("Stride.Data.DatabaseModels.GoalPriority", b =>
                {
                    b.Property<int>("goal_priority_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("goal_priority_id"));

                    b.Property<string>("name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.HasKey("goal_priority_id");

                    b.ToTable("GoalPrioritiy");
                });

            modelBuilder.Entity("Stride.Data.DatabaseModels.Habit", b =>
                {
                    b.Property<int>("habit_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("habit_id"));

                    b.Property<DateTime>("created_at")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("description")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<int>("habit_frequency_id")
                        .HasColumnType("int");

                    b.Property<DateTime>("reminder_time")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("title")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<DateTime>("updated_at")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("user_id")
                        .HasColumnType("int");

                    b.HasKey("habit_id");

                    b.HasIndex("habit_frequency_id");

                    b.HasIndex("user_id");

                    b.ToTable("Habits");
                });

            modelBuilder.Entity("Stride.Data.DatabaseModels.HabitFrequency", b =>
                {
                    b.Property<int>("habit_frequency_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("habit_frequency_id"));

                    b.Property<string>("name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.HasKey("habit_frequency_id");

                    b.ToTable("HabitFrequency");
                });

            modelBuilder.Entity("Stride.Data.DatabaseModels.Notification", b =>
                {
                    b.Property<int>("notification_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("notification_id"));

                    b.Property<string>("content")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool>("is_read")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("notification_type_id")
                        .HasColumnType("int");

                    b.Property<DateTime>("timestamp")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("user_id")
                        .HasColumnType("int");

                    b.HasKey("notification_id");

                    b.HasIndex("notification_type_id");

                    b.HasIndex("user_id");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("Stride.Data.DatabaseModels.NotificationType", b =>
                {
                    b.Property<int>("notification_type_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("notification_type_id"));

                    b.Property<string>("description")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("notification_type_id");

                    b.ToTable("NotificationType");
                });

            modelBuilder.Entity("Stride.Data.DatabaseModels.Theme", b =>
                {
                    b.Property<int>("theme_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("theme_id"));

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("theme_id");

                    b.ToTable("Theme");
                });

            modelBuilder.Entity("Stride.Data.DatabaseModels.User", b =>
                {
                    b.Property<int>("user_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("user_id"));

                    b.Property<string>("email")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("password_hash")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("username")
                        .HasColumnType("longtext");

                    b.HasKey("user_id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Stride.Data.DatabaseModels.UserAchievements", b =>
                {
                    b.Property<int>("user_achievement_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("user_achievement_id"));

                    b.Property<DateTime>("awarded_at")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("is_displayed")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("progress")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("user_achievement_id");

                    b.ToTable("UserAchievements");
                });

            modelBuilder.Entity("Stride.Data.DatabaseModels.UserSetting", b =>
                {
                    b.Property<int>("user_setting_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("user_setting_id"));

                    b.Property<DateTime>("created_at")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("email_notifications")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("push_notifications")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("theme_id")
                        .HasColumnType("int");

                    b.Property<int>("user_id")
                        .HasColumnType("int");

                    b.HasKey("user_setting_id");

                    b.HasIndex("theme_id");

                    b.HasIndex("user_id")
                        .IsUnique();

                    b.ToTable("UserSettings");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Stride.Data.DatabaseModels.Achievement", b =>
                {
                    b.HasOne("Stride.Data.DatabaseModels.AchievementType", "achievementType")
                        .WithMany()
                        .HasForeignKey("achievement_type_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Stride.Data.DatabaseModels.User", null)
                        .WithMany("Achievements")
                        .HasForeignKey("user_id");

                    b.Navigation("achievementType");
                });

            modelBuilder.Entity("Stride.Data.DatabaseModels.Goal", b =>
                {
                    b.HasOne("Stride.Data.DatabaseModels.Category", "Category")
                        .WithMany("Goals")
                        .HasForeignKey("category_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Stride.Data.DatabaseModels.GoalPriority", "Priority")
                        .WithMany()
                        .HasForeignKey("goal_priority_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Stride.Data.DatabaseModels.User", "User")
                        .WithMany()
                        .HasForeignKey("user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("Priority");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Stride.Data.DatabaseModels.Habit", b =>
                {
                    b.HasOne("Stride.Data.DatabaseModels.HabitFrequency", "HabitFrequency")
                        .WithMany("Habits")
                        .HasForeignKey("habit_frequency_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Stride.Data.DatabaseModels.User", "user")
                        .WithMany()
                        .HasForeignKey("user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("HabitFrequency");

                    b.Navigation("user");
                });

            modelBuilder.Entity("Stride.Data.DatabaseModels.Notification", b =>
                {
                    b.HasOne("Stride.Data.DatabaseModels.NotificationType", "NotificationType")
                        .WithMany("Notifications")
                        .HasForeignKey("notification_type_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Stride.Data.DatabaseModels.User", "User")
                        .WithMany("Notifications")
                        .HasForeignKey("user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("NotificationType");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Stride.Data.DatabaseModels.UserSetting", b =>
                {
                    b.HasOne("Stride.Data.DatabaseModels.Theme", "Theme")
                        .WithMany()
                        .HasForeignKey("theme_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Stride.Data.DatabaseModels.User", "User")
                        .WithOne("UserSetting")
                        .HasForeignKey("Stride.Data.DatabaseModels.UserSetting", "user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Theme");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Stride.Data.DatabaseModels.Category", b =>
                {
                    b.Navigation("Goals");
                });

            modelBuilder.Entity("Stride.Data.DatabaseModels.HabitFrequency", b =>
                {
                    b.Navigation("Habits");
                });

            modelBuilder.Entity("Stride.Data.DatabaseModels.NotificationType", b =>
                {
                    b.Navigation("Notifications");
                });

            modelBuilder.Entity("Stride.Data.DatabaseModels.User", b =>
                {
                    b.Navigation("Achievements");

                    b.Navigation("Notifications");

                    b.Navigation("UserSetting");
                });
#pragma warning restore 612, 618
        }
    }
}
