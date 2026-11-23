using Code123.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Code123.Controllers;

public class AdminController(ApplicationDbContext context) : Controller
{
    [HttpGet]
    public IActionResult Index()=> RedirectToAction (nameof(Dashboard));

    [HttpGet("/admin-preview/dashboard")]
    public async Task<IActionResult> Dashboard() =>
    View("~/Views/AdminViews/Dashboard.cshtml",
        new DashboardViewModel(
            await context.LearningPaths.CountAsync(),
            await context.Units.CountAsync(),
            await context.Lessons.CountAsync(),
            await context.ExerciseQuestions.CountAsync(),
            await context.Users.CountAsync()));
}

public record DashboardViewModel(int LearningPaths, int Units, int Lessons, int Questions, int Users);

