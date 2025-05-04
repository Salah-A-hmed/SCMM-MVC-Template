using Microsoft.EntityFrameworkCore.Query;
using SCMS.Models;

namespace SCMS.IRepo
{

    public interface IEventBookingRepo : IBasicRepo<EventBooking>
    {
        Task<IEnumerable<EventBooking>> GetBookingsByStudentAsync(int studentId, params Func<IQueryable<EventBooking>, IIncludableQueryable<EventBooking, object>>[] includes);
        Task<EventBooking> GetBookingByEventAndStudentAsync(int eventId, int studentId);
    }
}
