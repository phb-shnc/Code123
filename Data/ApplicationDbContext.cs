using Code123.Models;
using Microsoft.EntityFrameworkCore;

namespace Code123.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<LearningPathModel> LearningPaths { get; set; } = null!;
        public DbSet<UnitModel> Units { get; set; } = null!;
        public DbSet<LessonModel> Lessons { get; set; } = null!;
        public DbSet<ExerciseQuestionModel> ExerciseQuestions { get; set; } = null!;
        public DbSet<UserModel> Users { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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

            modelBuilder.Entity<UserModel>(entity =>
            {
                entity.ToTable("Users");
            });
        }
    }
}
