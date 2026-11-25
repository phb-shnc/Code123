using Code123.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Code123.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class DashboardController(ApplicationDbContext context) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        // Newest 5 users, with their role names
        var recent = await context.Users
            .OrderByDescending(u => u.CreatedAt)
            .Take(5)
            .Select(u => new
            {
                u.Name,
                u.Username,
                u.Email,
                u.CreatedAt,
                Roles = u.UserRoles.Select(ur => ur.Role.RoleName).ToList()
            })
            .ToListAsync();

        var recentUsers = recent
            .Select(u => new UserSummary(u.Name, u.Username, u.Email, string.Join(", ", u.Roles), u.CreatedAt))
            .ToList();

        var model = new DashboardViewModel(
            await context.LearningPaths.CountAsync(),
            await context.Units.CountAsync(),
            await context.Lessons.CountAsync(),
            await context.ExerciseQuestions.CountAsync(),
            await context.Users.CountAsync(),
            recentUsers);

        return View("~/Views/Admin/Dashboard/Index.cshtml", model);
    }
}

public record UserSummary(string Name, string Username, string Email, string Roles, DateTime CreatedAt);

public record DashboardViewModel(
    int LearningPaths,
    int Units,
    int Lessons,
    int Questions,
    int Users,
    List<UserSummary> RecentUsers);