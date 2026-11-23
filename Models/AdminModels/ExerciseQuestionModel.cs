using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Code123.Models;

public class ExerciseQuestionModel : IEntity, IValidatableObject
{
    [BindNever] public int Id { get; set; }

    [Display(Name = "Lesson"), Range(1, int.MaxValue, ErrorMessage = "Select A Lesson")]
    public int LessonId { get; set; }

    [Display(Name = "Question"),Required, StringLength(1000)]  
    public string QuestionText { get; set; } = string.Empty;

    [Required]
    [StringLength(4000)]
    public string Options{ get; set; } = "[]";

    [Display(Name = "Correct Answer"), Required, StringLength(300)]
    public string CorrectAnswer { get; set; } = string.Empty;

    [BindNever, ValidateNever] public LessonModel? LessonsModel { get; set; }

    public IEnumerable<ValidationResult> Validate(
           ValidationContext validationContext)
    {
        string[]? options = null;

        try
        {
            options = JsonSerializer.Deserialize<string[]>(Options ?? "");
        }
        catch (JsonException)
        {
        }

        if (options is null || options.Length < 2 ||
            options.Any(string.IsNullOrWhiteSpace))
        {
            yield return new ValidationResult(
                "Options must be a JSON array of at least two non-empty strings, e.g. [\"A\",\"B\"].",
                [nameof(Options)]);
        }
        else if (!options.Contains(CorrectAnswer))
        {
            yield return new ValidationResult(
                "Correct answer must exactly match one of the options.",
                [nameof(CorrectAnswer)]);
        }
    }
}
