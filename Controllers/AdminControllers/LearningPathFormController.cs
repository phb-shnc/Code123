using Code123.Models;
using Microsoft.AspNetCore.Mvc;

namespace Code123.Controllers
{
    [Route("admin-preview")]
    public class LearningPathFormController : Controller
    {
        private const string V = "~/Views/AdminViews/";

        [HttpGet("learning-paths")]
        public IActionResult LearningPaths() => View(V + "List.cshtml");

        [HttpGet("learning-paths/create")]
        public IActionResult Create() => View(V + "Form.cshtml", new LearningPathFormModel());

        [HttpGet("learning-paths/edit/{id:int}")]
        public IActionResult Edit(int id) =>
            View(V + "Form.cshtml", new LearningPathFormModel { Id = id, Title = "Web development basics", Level = "Beginner", Status = "Published" });

        [HttpPost("learning-paths/save")]
        [ValidateAntiForgeryToken]
        public IActionResult Save(LearningPathFormModel model)
        {
            if (!ModelState.IsValid)
                return View(V + "Form.cshtml", model);

            TempData["Success"] = "Learning path saved.";
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