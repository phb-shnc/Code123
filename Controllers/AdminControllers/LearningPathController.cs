using Code123.Data;
using Code123.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Code123.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class LearningPathsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const string V = "~/Views/Admin/LearningPath/";

        public LearningPathsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Display all learning paths.
        public async Task<IActionResult> Index()
        {
            var paths = await _context.LearningPaths
                .AsNoTracking()
                .OrderBy(path => path.DisplayOrder)
                .ThenBy(path => path.Title)
                .Select(path => new LearningPathListItemViewModel
                {
                    Id = path.Id,
                    Title = path.Title,
                    Description = path.Description,
                    Level = path.Level,
                    Status = path.Status,
                    DisplayOrder = path.DisplayOrder,
                    UnitCount = path.UnitsModel.Count(),
                    LessonCount = path.UnitsModel
                        .SelectMany(unit => unit.LessonsModel)
                        .Count(),
                    StudentCount = null
                })
                .ToListAsync();

            return View(V + "Index.cshtml", paths);
        }

        // Create and edit happen in the modal on the Index page.
        [HttpGet]
        public IActionResult Create() => RedirectToAction(nameof(Index), new { create = true });

        [HttpGet]
        public IActionResult Edit(int id) => RedirectToAction(nameof(Index));

        // Create or update a learning path.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(LearningPathFormModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please provide valid learning path information.";
                return RedirectToAction(nameof(Index));
            }

            LearningPathModel? path;

            if (model.Id == 0)
            {
                path = new LearningPathModel();
                _context.LearningPaths.Add(path);
            }
            else
            {
                path = await _context.LearningPaths.FindAsync(model.Id);

                if (path == null)
                    return NotFound();
            }

            path.Title = model.Title;
            path.Description = model.Description;
            path.Level = model.Level;
            path.Status = model.Status;
            path.DisplayOrder = model.DisplayOrder;

            try
            {
                await _context.SaveChangesAsync();

                TempData["Success"] = model.Id == 0
                    ? "Learning path created successfully."
                    : "Learning path updated successfully.";
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = "The learning path could not be saved.";
            }

            return RedirectToAction(nameof(Index));
        }

        // Delete a learning path.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var path = await _context.LearningPaths.FindAsync(id);

            if (path == null)
                return NotFound();

            bool hasUnits = await _context.Units
                .AnyAsync(unit => unit.LearningPathId == id);

            if (hasUnits)
            {
                TempData["Error"] = "This learning path cannot be deleted because it still contains units.";
                return RedirectToAction(nameof(Index));
            }

            _context.LearningPaths.Remove(path);

            try
            {
                await _context.SaveChangesAsync();
                TempData["Success"] = "Learning path deleted successfully.";
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = "The learning path could not be deleted.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}