using Code123.Data;
using Code123.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace Code123.Controllers;

public class UnitsController(ApplicationDbContext context) : CrudController<UnitModel>(context)
{
    protected override string DisplayName => "Unit";
    protected override IQueryable<UnitModel> GetQuery() =>
        DbSet.AsNoTracking()
            .Include(u => u.LearningPathsModel)
            .OrderBy(u => u.LearningPathId)
            .ThenBy(u => u.OrderIndex);
    protected override async Task PopulateLookupsAsync(UnitModel model) =>
        ViewData["LearningPathId"] = new SelectList(
            await context.LearningPaths.AsNoTracking().OrderBy(p => p.Title).ToListAsync(), nameof(LearningPathModel.Id), nameof(LearningPathModel.Title), model?.LearningPathId);

    protected override async Task ValidateAsync (UnitModel model, int? id)
    {
        if (!await Context.LearningPaths.AnyAsync (p => p.Id == model.LearningPathId))
            ModelState.AddModelError(nameof(UnitModel.LearningPathId), "Selected Learning Path does not exist.");
    }

    protected override void Apply(UnitModel existing, UnitModel model)
    {
        existing.LearningPathId = model.LearningPathId;
        existing.Title = model.Title;
        existing.OrderIndex = model.OrderIndex;
    }

}
