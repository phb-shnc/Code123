
using Code123.Data;
using Code123.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Code123.Controllers;

public class ExerciseQuestionsController(ApplicationDbContext context)
    : CrudController<ExerciseQuestionModel>(context)
{
    protected override string DisplayName => "Exercise question";

    protected override IQueryable<ExerciseQuestionModel> GetQuery() =>
        DbSet.AsNoTracking()
            .Include(q => q.LessonsModel)
                .ThenInclude(l => l!.UnitsModel)
                    .ThenInclude(u => u!.LearningPathsModel)
            .OrderBy(q => q.LessonId)
            .ThenBy(q => q.Id);

    protected override async Task PopulateLookupsAsync(ExerciseQuestionModel model)
    {
        var lessons = await context.Lessons
            .AsNoTracking()
            .Include(l => l.UnitsModel)
                .ThenInclude(u => u!.LearningPathsModel)
            .OrderBy(l => l.UnitsModel!.LearningPathId)
            .ThenBy(l => l.UnitId)
            .ThenBy(l => l.Id)
            .ToListAsync();

        var lessonOptions = lessons.Select(lesson => new SelectListItem
        {
            Value = lesson.Id.ToString(),
            Text = $"{lesson.UnitsModel?.LearningPathsModel?.Title ?? "Unknown"} / " +
                   $"{lesson.UnitsModel?.Title ?? "Unknown"} / {lesson.Title}"
        }).ToList();

        ViewData["LessonId"] = new SelectList(
            lessonOptions,
            "Value",
            "Text",
            model.LessonId);
    }

    protected override async Task ValidateAsync(
        ExerciseQuestionModel model,
        int? id)
    {
        if (!await Context.Lessons.AnyAsync(l => l.Id == model.LessonId))
        {
            ModelState.AddModelError(
                nameof(ExerciseQuestionModel.LessonId),
                "Selected lesson does not exist.");
        }
    }

    protected override void Apply(
        ExerciseQuestionModel existing,
        ExerciseQuestionModel model)
    {
        existing.LessonId = model.LessonId;
        existing.QuestionText = model.QuestionText;
        existing.Options = model.Options;
        existing.CorrectAnswer = model.CorrectAnswer;
    }

    [HttpGet]
    public override async Task<IActionResult> Index()
    {
        await PopulateLookupsAsync(new ExerciseQuestionModel());
        return await base.Index();
    }
}
