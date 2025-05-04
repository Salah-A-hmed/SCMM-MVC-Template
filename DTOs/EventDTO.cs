// DTOs/EventDTO.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace SCMS.DTOs
{
    public class EventDTO
    {
        
        [Required]
        public string Title { get; set; }
        
        [Required]
        public DateTime StartDate { get; set; }
        
        [Required]
        public string Location { get; set; }
        
        [Required]
        [Range(1, int.MaxValue)]
        public int Capacity { get; set; }
    }

    public class EventDetailsDTO : EventDTO
    {
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public int CurrentBookings { get; set; }
        public bool HasBooked { get; set; }
    }

    public class EventBookingDTO
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string EventTitle { get; set; }
        public DateTime EventStartDate { get; set; }
        public DateTime BookingDate { get; set; }
    }
}
