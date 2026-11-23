
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Code123.Models
{
    public class LearningPathModel : IEntity
    {
        [BindNever]
        public int Id { get; set; }

        [Required]
        [StringLength(120)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [AllowedValues("Beginner", "Intermediate", "Advanced")]
        public string Level { get; set; } = "Beginner";

        [Required]
        [StringLength(20)]
        [AllowedValues("Draft", "Published")]
        public string Status { get; set; } = "Draft";

        [Range(1, 999)]
        public int DisplayOrder { get; set; } = 1;

        [BindNever]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BindNever, ValidateNever]
        public ICollection<UnitModel> UnitsModel { get; set; } = [];
    }
}
