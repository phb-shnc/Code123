using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AdminDashboard.Models 
{
    public class LessonModel : IdentityModel
    {
        [BindNever] public int Id { get; set; }

        [Display(Name = "Unit"), Range(1, int.MaxValue, ErrorMessage = "Select A Unit")]
        public int UnitId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required][StringLength(50_000)]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; } = string.Empty;

        [BindNever, ValidateNever]public UnitModel? UnitsModel { get; set; }
        [BindNever, ValidateNever]public ICollection<ExerciseQuestionModel> ExerciseQuestions { get; set; } = [];



    }
}

