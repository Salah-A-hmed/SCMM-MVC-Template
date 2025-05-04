using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using SCMS.Data;
using SCMS.IRepo;
using SCMS.Models;

namespace SCMS.Repos
{
    public class EventBookingRepo : BasicRepo<EventBooking>, IEventBookingRepo
    {
        public EventBookingRepo(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<EventBooking>> GetBookingsByStudentAsync(int studentId, params Func<IQueryable<EventBooking>, IIncludableQueryable<EventBooking, object>>[] includes)
        {
            IQueryable<EventBooking> query = _context.EventBookings
                .Where(b => b.StudentId == studentId && !b.IsCancelled);

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = include(query);
                }
            }

            return await query.ToListAsync();
        }

        public async Task<EventBooking> GetBookingByEventAndStudentAsync(int eventId, int studentId)
        {
            return await _context.EventBookings
                .FirstOrDefaultAsync(b => b.EventId == eventId && b.StudentId == studentId);
        }
    }
}
