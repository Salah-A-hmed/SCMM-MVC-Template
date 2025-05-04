using System.ComponentModel.DataAnnotations.Schema;

namespace SCMS.Models
{
    public class EventBooking
    {
        public int Id { get; set; }
        [ForeignKey("Event")]
        public int EventId { get; set; }
        public Event Event { get; set; }
        [ForeignKey("Student")]
        public int StudentId { get; set; } // User ID of student
        public Student Student { get; set; }
        public DateTime BookingDate { get; set; } = DateTime.Now;
        public bool IsCancelled { get; set; } = false;
    }
}
