using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SCMM.Models
{
    public class StudentExam
    {
        [Key]
        public int StudentExamId { get; set; }

        [Required]
        public int ExamId { get; set; }

        [Required]
        public string StudentId { get; set; }

        public DateTime? StartedAt { get; set; }

        public DateTime? SubmittedAt { get; set; }

        public int? Score { get; set; }

        public bool IsSubmitted { get; set; }

        // Navigation properties
        [ForeignKey("ExamId")]
        public virtual Exam Exam { get; set; }

        [ForeignKey("StudentId")]
        public virtual User Student { get; set; }

        public virtual ICollection<StudentAnswer> StudentAnswers { get; set; }
    }
}