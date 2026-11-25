using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Code123.Areas.Student.Controllers
{
    [Area("Student")]
    [Authorize(Roles = "Student,Admin")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            // Explicitly point to the global Views folder structure
            return View("~/Views/Student/Dashboard/Index.cshtml");
        }
    }
}