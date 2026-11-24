using Code123.Data;
using Code123.Models;
using Microsoft.AspNetCore.Mvc; 
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Code123.Controllers;

public class ExerciseQuestionsController(ApplicationDbContext context) : CrudController<ExerciseQuestionModel>(context)
{
    protected override string DisplayName => "Exercise question";
    protected override IQueryable<ExerciseQuestionModel> GetQuery() =>
        DbSet.AsNoTracking()
            .Include(q => q.LessonsModel)
            .OrderBy(q => q.LessonId)
            .ThenBy(q => q.Id);
    protected override async Task PopulateLookupsAsync(ExerciseQuestionModel model) =>
        ViewData["LessonId"] = new SelectList(
            await Context.Lessons.AsNoTracking().OrderBy(l => l.Title).ToListAsync(),
            nameof(LessonModel.Id), nameof(LessonModel.Title), model?.LessonId);

    protected override async Task ValidateAsync(ExerciseQuestionModel model, int? id)
    {
        if (!await Context.Lessons.AnyAsync(l => l.Id == model.LessonId))
            ModelState.AddModelError(nameof(ExerciseQuestionModel.LessonId), "Selected lesson does not exist.");
    }

    protected override void Apply(ExerciseQuestionModel existing, ExerciseQuestionModel model)
    {
        existing.LessonId = model.LessonId;
        existing.QuestionText = model.QuestionText;
        existing.Options = model.Options;
        existing.CorrectAnswer = model.CorrectAnswer;
    }
}
