using AdminDashboard.Models;
using Microsoft.EntityFrameworkCore;

namespace AdminDashboard.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<LearningPathModel> LearningPathsModel { get; set; }
        public DbSet<UnitModel> UnitsModel { get; set; }
        public DbSet<LessonModel> LessonsModel { get; set; }
        public DbSet<ExerciseQuestionModel> ExerciseQuestionsModel { get; set; }
        public DbSet<UserModel> Users { get; set; }
    }
}
