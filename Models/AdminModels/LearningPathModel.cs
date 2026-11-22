using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AdminDashboard.Models
{
    public class LearningPathModel : IdentityModel
    {
        //prevents tampering//
        [BindNever] public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set;} = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Description { get; set;} = string.Empty;

        [BindNever] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BindNever,ValidateNever] public ICollection <UnitModel> UnitsModel { get; set; } = [];


    }
    
}


