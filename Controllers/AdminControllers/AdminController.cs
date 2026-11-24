using Code123.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Code123.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController(ApplicationDbContext context) : Controller
    {
        [HttpGet]
        public IActionResult Index() => RedirectToAction(nameof(Dashboard));

        // System Statistics Dashboard
        [HttpGet("/admin-preview/dashboard")]
        public async Task<IActionResult> Dashboard() =>
            View("~/Views/Admin/Dashboard/Index.cshtml",
                new DashboardViewModel(
                    await context.LearningPaths.CountAsync(),
                    await context.Units.CountAsync(),
                    await context.Lessons.CountAsync(),
                    await context.ExerciseQuestions.CountAsync(),
                    await context.Users.CountAsync()));

        // User Management (Merged from the new controller)
        [HttpGet]
        public async Task<IActionResult> Users()
        {
            var users = await context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();

            return View(users);
        }
    }

    public record DashboardViewModel(int LearningPaths, int Units, int Lessons, int Questions, int Users);
}