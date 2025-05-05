using System.ComponentModel.DataAnnotations;

namespace SCMM.Models
{
    public class Exam
    {
        [Key]
        public int ExamId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Required]
        public int DurationInMinutes { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public int TotalPoints { get; set; }

        [Required]
        public int PassingScore { get; set; }

        // Navigation properties
        public virtual ICollection<Question> Questions { get; set; }
        public virtual ICollection<StudentExam> StudentExams { get; set; }
    }
}
