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
            await Context.LearningPaths.AsNoTracking().OrderBy(p => p.Title).ToListAsync(),
            nameof(LearningPathModel.Id),
            nameof(LearningPathModel.Title),
            model?.LearningPathId);

    protected override async Task ValidateAsync(UnitModel model, int? id)
    {
        // The selected learning path must exist
        if (!await Context.LearningPaths.AnyAsync(p => p.Id == model.LearningPathId))
        {
            ModelState.AddModelError(
                nameof(UnitModel.LearningPathId),
                "Selected Learning Path does not exist.");
            return;
        }

        // The order number must be unique within the learning path
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