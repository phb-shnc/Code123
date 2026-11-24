using Code123.Models;
using Microsoft.EntityFrameworkCore;

namespace Code123.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Learning DB Sets
        public DbSet<LearningPathModel> LearningPaths { get; set; } = null!;
        public DbSet<UnitModel> Units { get; set; } = null!;
        public DbSet<LessonModel> Lessons { get; set; } = null!;
        public DbSet<ExerciseQuestionModel> ExerciseQuestions { get; set; } = null!;

        // Registration & Identity DB Sets
        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Always call base when working with Identity or complex configurations
            base.OnModelCreating(modelBuilder);

            // --- Learning Path Configurations ---
            modelBuilder.Entity<LearningPathModel>(entity =>
            {
                entity.ToTable("LearningPaths");

                entity.Property(path => path.Level)
                    .HasMaxLength(20);

                entity.Property(path => path.Status)
                    .HasMaxLength(20);

                entity.Property(path => path.DisplayOrder)
                    .HasDefaultValue(1);

                entity.Property(path => path.CreatedAt)
                    .HasDefaultValueSql("SYSUTCDATETIME()");

                entity.HasMany(path => path.UnitsModel)
                    .WithOne(unit => unit.LearningPathsModel)
                    .HasForeignKey(unit => unit.LearningPathId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<UnitModel>(entity =>
            {
                entity.ToTable("Units");

                entity.HasMany(unit => unit.LessonsModel)
                    .WithOne(lesson => lesson.UnitsModel)
                    .HasForeignKey(lesson => lesson.UnitId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<LessonModel>(entity =>
            {
                entity.ToTable("Lessons", table =>
                    table.HasCheckConstraint(
                        "CK_Lessons_Content",
                        "DATALENGTH([Content]) <= 100000"));

                entity.HasMany(lesson => lesson.ExerciseQuestions)
                    .WithOne(question => question.LessonsModel)
                    .HasForeignKey(question => question.LessonId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ExerciseQuestionModel>(entity =>
            {
                entity.ToTable("ExerciseQuestions");
            });

            // --- Registration & Identity Configurations ---
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");

                entity.Property(u => u.Name).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Username).IsRequired().HasMaxLength(50);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(200);
                entity.Property(u => u.PasswordHash).IsRequired();

                entity.HasIndex(u => u.Username).IsUnique();
                entity.HasIndex(u => u.Email).IsUnique();
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Roles");

                entity.Property(r => r.RoleName).IsRequired().HasMaxLength(50);
                entity.HasIndex(r => r.RoleName).IsUnique();
            });

            // Many-to-many join table with composite key for Users and Roles
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.ToTable("UserRoles");

                entity.HasKey(ur => new { ur.UserId, ur.RoleId });

                entity.HasOne(ur => ur.User)
                      .WithMany(u => u.UserRoles)
                      .HasForeignKey(ur => ur.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ur => ur.Role)
                      .WithMany(r => r.UserRoles)
                      .HasForeignKey(ur => ur.RoleId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Seed default roles (Student and Admin only)
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, RoleName = "Student" },
                new Role { Id = 2, RoleName = "Admin" }
            );
        }
    }
}
