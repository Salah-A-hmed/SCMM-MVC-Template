using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SCMM.Models
{
    public class Option
    {
        [Key]
        public int OptionId { get; set; }

        [Required]
        public int QuestionId { get; set; }

        [Required]
        [StringLength(500)]
        public string OptionText { get; set; }

        [Required]
        public bool IsCorrect { get; set; }

        // Navigation property
        [ForeignKey("QuestionId")]
        public virtual Question Question { get; set; }
    }
}