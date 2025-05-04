// Services/IEventService.cs
using SCMS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCMS.Services
{
    public interface IEventService
    {
        Task<IEnumerable<Event>> GetAllEventsAsync();
        Task<IEnumerable<Event>> GetActiveEventsAsync();
        Task<Event> GetEventByIdAsync(int id);
        Task CreateEventAsync(Event eventModel/*, int userId*/);
        Task UpdateEventAsync(Event eventModel);
        Task DeleteEventAsync(int id);
        Task<bool> BookEventAsync(int eventId, int studentId);
        Task<bool> CancelBookingAsync(int eventId, int studentId);
        Task<IEnumerable<EventBooking>> GetStudentBookingsAsync(int studentId);
        Task<int> GetEventBookingCountAsync(int eventId);
        Task<bool> HasStudentBookedEventAsync(int eventId, int studentId);
    }
}
