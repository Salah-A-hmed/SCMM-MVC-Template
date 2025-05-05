using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.X509Certificates;

namespace SCMM.Models
{
    public class Student
    {
        // TODO: ID, Name, Age, Email, AcademicYear, ImagePath, etc.

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int NationalId { get; set; }

        [Required]
        public int Age { get; set; }

        [Required]
        public string AcademicYear { get; set; }

        public int GroupId { get; set; }

        public ICollection<SupportTicket> SupportTickets { get; set; }
    }
}
