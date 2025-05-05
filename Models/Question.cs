using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SCMM.Models
{
    public enum QuestionType
    {
        MultipleChoice,
        Essay
    }
    public class Question
    {
       

        [Key]
        public int QuestionId { get; set; }

        [Required]
        public int ExamId { get; set; }

        [Required]
        [StringLength(1000)]
        public string QuestionText { get; set; }

        [Required]
        public QuestionType QuestionType { get; set; }

        [Required]
        public int Points { get; set; }

        public int? OrderNumber { get; set; }

        // Navigation properties
        [ForeignKey("ExamId")]
        public virtual Exam Exam { get; set; }
        public virtual ICollection<Option> Options { get; set; }
    }
}
