using SCMM.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SCMM.Models
{
    public class SupportTicket
    {
        // TODO: ID, StudentID, Title, Description, Status, Response

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public Categories Category { get; set; }

        [Required]
        public Complaints_status Status { get; set; } = Complaints_status.Pending;

        public DateTime DateTime { get; set; } = DateTime.Now;

       
        
        [Required]
        public int StudentId { get; set; }

        [ForeignKey("StudentId")]
        public Student Student { get; set; }
        public string? Answer { get; set; }

    }
}
