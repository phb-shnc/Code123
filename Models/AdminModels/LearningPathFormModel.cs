using System.ComponentModel.DataAnnotations;

namespace Code123.Models
{
    
    public class LearningPathFormModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Enter a title.")]
        [StringLength(120, ErrorMessage = "Title must be 120 characters or fewer.")]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string Level { get; set; } = "Beginner"; 

        public string Status { get; set; } = "Draft";

        [Range(1, 999, ErrorMessage = "Enter an order between 1 and 999.")]
        public int DisplayOrder { get; set; } = 1;
    }
}