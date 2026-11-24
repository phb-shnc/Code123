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
            .Include(u => u.LessonsModel)
            .OrderBy(u => u.LearningPathId)
            .ThenBy(u => u.OrderIndex);
    protected override async Task PopulateLookupsAsync(UnitModel model) =>
        ViewData["LearningPathId"] = new SelectList(
            await context.LearningPaths.AsNoTracking().OrderBy(p => p.Title).ToListAsync(), nameof(LearningPathModel.Id), nameof(LearningPathModel.Title), model?.LearningPathId);

    protected override async Task ValidateAsync(UnitModel model, int? id)
    {
        // Check whether the selected learning path exists.
        if (!await Context.LearningPaths.AnyAsync(p => p.Id == model.LearningPathId))
        {
            ModelState.AddModelError(
                nameof(UnitModel.LearningPathId),
                "Selected Learning Path does not exist.");

            return;
        }

        // Check whether the order number is already used
        // by another unit in the same learning path.
        bool orderExists = await Context.Units.AnyAsync(u =>
            u.LearningPathId == model.LearningPathId &&
            u.OrderIndex == model.OrderIndex &&
            u.Id != id);

        if (orderExists)
        {
            ModelState.AddModelError(
                nameof(UnitModel.OrderIndex),
                "This order number is already used in the selected learning path.");
        }
    }

    protected override void Apply(UnitModel existing, UnitModel model)
    {
        existing.LearningPathId = model.LearningPathId;
        existing.Title = model.Title;
        existing.OrderIndex = model.OrderIndex;
    }

    [HttpGet]
    public override async Task<IActionResult> Index()
    {
        await PopulateLookupsAsync(new UnitModel());

        return await base.Index();
    }

}
