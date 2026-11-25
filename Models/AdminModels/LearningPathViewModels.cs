using System.ComponentModel.DataAnnotations;

namespace Code123.Areas.Admin.Controllers
{
    // Used by Index.cshtml (the table)
    public class LearningPathListItemViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public int UnitCount { get; set; }
        public int LessonCount { get; set; }
        public int? StudentCount { get; set; }
    }

    // Used by the modal form and the Save action
    public class LearningPathFormModel
    {
        public int Id { get; set; }

        [Required, StringLength(120)]
        public string Title { get; set; } = string.Empty;

        [Required, StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string Level { get; set; } = "Beginner";

        [Required]
        public string Status { get; set; } = "Draft";

        public int DisplayOrder { get; set; } = 1;
    }
}