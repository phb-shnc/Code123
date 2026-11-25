
using Code123.Controllers;
using Code123.Data;
using Code123.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Code123.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class LessonsController(ApplicationDbContext context) : CrudController<LessonModel>(context)
{
    protected override string DisplayName => "Lesson";

    protected override IQueryable<LessonModel> GetQuery() =>
        DbSet.AsNoTracking()
            .Include(l => l.UnitsModel)
                .ThenInclude(u => u!.LearningPathsModel)
            .Include(l => l.ExerciseQuestions)
            .OrderBy(l => l.UnitId)
            .ThenBy(l => l.Id);

    protected override async Task PopulateLookupsAsync(LessonModel model)
    {
        var units = await Context.Units
            .AsNoTracking()
            .Include(u => u.LearningPathsModel)
            .OrderBy(u => u.LearningPathId)
            .ThenBy(u => u.OrderIndex)
            .ToListAsync();

        var unitOptions = units.Select(unit => new SelectListItem
        {
            Value = unit.Id.ToString(),
            Text = $"{unit.LearningPathsModel?.Title ?? "Unknown"} / {unit.Title}"
        }).ToList();

        ViewData["UnitId"] = new SelectList(
            unitOptions,
            "Value",
            "Text",
            model.UnitId);
    }

    protected override async Task ValidateAsync(LessonModel model, int? id)
    {
        if (!await Context.Units.AnyAsync(u => u.Id == model.UnitId))
        {
            ModelState.AddModelError(
                nameof(LessonModel.UnitId),
                "Selected Unit does not exist.");
        }
    }

    protected override void Apply(LessonModel existing, LessonModel model)
    {
        existing.UnitId = model.UnitId;
        existing.Title = model.Title;
        existing.Content = model.Content;
    }

    [HttpGet]
    public override async Task<IActionResult> Index()
    {
        await PopulateLookupsAsync(new LessonModel());
        return await base.Index();
    }
}