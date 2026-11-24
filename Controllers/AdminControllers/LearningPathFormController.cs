
using Code123.Data;
using Code123.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Code123.Controllers
{
    [Route("admin-preview")]
    public class LearningPathFormController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const string V = "~/Views/Admin/LearningPath/";

        public LearningPathFormController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Display all learning paths.
        [HttpGet("learning-paths")]
        public async Task<IActionResult> LearningPaths()
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

            return View("~/Views/Admin/LearningPath/Index.cshtml", paths);
        }

        // Open the Create form page if needed.
        [HttpGet("learning-paths/create")]
        public IActionResult Create()
        {
            return View(V + "Form.cshtml", new LearningPathFormModel());
        }

        // Retrieve an existing learning path for editing.
        [HttpGet("learning-paths/edit/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            var path = await _context.LearningPaths
                .AsNoTracking()
                .FirstOrDefaultAsync(path => path.Id == id);

            if (path == null)
                return NotFound();

            var model = new LearningPathFormModel
            {
                Id = path.Id,
                Title = path.Title,
                Description = path.Description,
                Level = path.Level,
                Status = path.Status,
                DisplayOrder = path.DisplayOrder
            };

            return View(V + "Form.cshtml", model);
        }

        // Create or update a learning path.
        [HttpPost("learning-paths/save")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(LearningPathFormModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please provide valid learning path information.";
                return RedirectToAction(nameof(LearningPaths));
            }

            LearningPathModel path;

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

            return RedirectToAction(nameof(LearningPaths));
        }

        // Delete a learning path.
        [HttpPost("learning-paths/delete/{id:int}")]
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
                TempData["Error"] =
                    "This learning path cannot be deleted because it still contains units.";
                return RedirectToAction(nameof(LearningPaths));
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

            return RedirectToAction(nameof(LearningPaths));
        }
    }

    // Data displayed in the Learning Paths table.
    public class LearningPathListItemViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int UnitCount { get; set; }
        public int LessonCount { get; set; }
        public int? StudentCount { get; set; }
        public string Description { get; set; } = string.Empty;
        public int DisplayOrder { get; set; } = 1;
    }
}
