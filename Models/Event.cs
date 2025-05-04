// Models/Event.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SCMS.Models
{
    public class Event
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }


        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Capacity { get; set; }
        public int RegisteredCount { get; set; } = 0; // Number of students registered for the event
        public bool IsActive { get; set; } = true;

        //public int CreatedBy { get; set; } // User ID of creator

        public ICollection<EventBooking> Bookings { get; set; }
    }
}
