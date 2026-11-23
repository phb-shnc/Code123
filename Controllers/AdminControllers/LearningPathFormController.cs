using Code123.Models;
using Microsoft.AspNetCore.Mvc;

namespace Code123.Controllers
{
    // Preview controller so you can click through every template.
    // Views are looked up in Views/AdminViews/.
    [Route("admin")]
    public class LearningPathFormController : Controller
    {
        [HttpGet("dashboard")]
        public IActionResult Dashboard() => View();

        [HttpGet("learning-paths")]
        public IActionResult LearningPaths() => View("List");

        [HttpGet("learning-paths/create")]
        public IActionResult Create() => View("Form", new LearningPathFormModel());

        [HttpGet("learning-paths/edit/{id:int}")]
        public IActionResult Edit(int id) =>
            View("Form", new LearningPathFormModel { Id = id, Title = "Web development basics", Level = "Beginner", Status = "Published" });

        [HttpPost("learning-paths/save")]
        [ValidateAntiForgeryToken]
        public IActionResult Save(LearningPathFormModel model)
        {
            if (!ModelState.IsValid)
                return View("Form", model);          // shows the error banner + field messages

            TempData["Success"] = "Learning path saved.";   // shows the success banner
            return RedirectToAction(nameof(LearningPaths));
        }

        [HttpPost("learning-paths/delete/{id:int}")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            TempData["Success"] = "Learning path deleted.";
            return RedirectToAction(nameof(LearningPaths));
        }
    }
}