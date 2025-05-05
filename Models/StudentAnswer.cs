using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SCMM.Models
{
    public class StudentAnswer
    {
        [Key]
        public int StudentAnswerId { get; set; }

        [Required]
        public int StudentExamId { get; set; }

        [Required]
        public int QuestionId { get; set; }

        public int? SelectedOptionId { get; set; }

        [StringLength(2000)]
        public string EssayAnswer { get; set; }

        public bool? IsCorrect { get; set; }

        // Navigation properties
        [ForeignKey("StudentExamId")]
        public virtual StudentExam StudentExam { get; set; }

        [ForeignKey("QuestionId")]
        public virtual Question Question { get; set; }

        [ForeignKey("SelectedOptionId")]
        public virtual Option SelectedOption { get; set; }
    }
}