using Code123.Data;
using Code123.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Code123.Controllers;

public class LessonsController(ApplicationDbContext context) : CrudController<LessonModel>(context)
{
    protected override string DisplayName => "Lesson";
    protected override IQueryable<LessonModel> GetQuery() => DbSet.AsNoTracking().Include(l => l.UnitId).OrderBy(l => l.UnitId).ThenBy(l => l.Id);
    protected override async Task PopulateLookupsAsync(LessonModel model) =>
        ViewData["UnitId"] = new SelectList(
            await context.Units.AsNoTracking().OrderBy(u => u.Title).ToListAsync(), nameof(UnitModel.Id), nameof(UnitModel.Title), model?.UnitId);
    protected override async Task ValidateAsynch(LessonModel model, int? id)
    {
        if (!await Context.Units.AnyAsync(u => u.Id == model.UnitId))
            ModelState.AddModelError(nameof(LessonModel.UnitId), "Selected Unit does not exist.");
    }
    protected override void Apply(LessonModel existing, LessonModel model)
    {
        existing.UnitId = model.UnitId;
        existing.Title = model.Title;
        existing.Content = model.Content;
       
    }
}