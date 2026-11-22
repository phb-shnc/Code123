using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AdminDashboard.Models 
{
    public class UnitModel : IdentityModel
    {
        [BindNever] public int Id { get; set; }

        [Display (Name="Learning Path"),Range(1,int.MaxValue, ErrorMessage = "Select Learning Path")]
        public int LearningPathId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Display (Name ="Order"), Range(0, int.MaxValue)]
        public int OrderIndex { get; set; } = 0;

        [BindNever, ValidateNever] public LearningPathModel? LearningPathsModel { get; set; }
        [BindNever,ValidateNever] public ICollection<LessonModel>LessonsModel { get; set; } = [];
    }

    

}
