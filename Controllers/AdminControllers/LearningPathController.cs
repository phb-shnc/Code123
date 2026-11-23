using Code123.Data;
using Code123.Models;


namespace Code123.Controllers;

public class LearningPathController(ApplicationDbContext context) : CrudController<LearningPathModel>(context)
{
	protected override string DisplayName => "Learning Path";
    protected override void Apply(
    LearningPathModel existing,
    LearningPathModel model)
    {
        existing.Title = model.Title;
        existing.Description = model.Description;
        existing.Level = model.Level;
        existing.Status = model.Status;
        existing.DisplayOrder = model.DisplayOrder;
    }
}
