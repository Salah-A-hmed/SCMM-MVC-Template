// Repositories/EventRepo.cs
using SCMS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SCMS.IRepo;
using SCMS.Data;

namespace SCMS.Repos
{
    public class EventRepo : BasicRepo<Event>, IEventRepo
    {
        public EventRepo(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Event>> GetActiveEventsAsync(params Func<IQueryable<Event>, IIncludableQueryable<Event, object>>[] includes)
        {
            IQueryable<Event> query = _context.Events.Where(e => e.IsActive);

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = include(query);
                }
            }

            return await query.ToListAsync();
        }

        public async Task<bool> HasBookingAsync(int eventId, int studentId)
        {
            return await _context.EventBookings
                .AnyAsync(b => b.EventId == eventId && b.StudentId == studentId && !b.IsCancelled);
        }

        public async Task<int> GetBookingCountAsync(int eventId)
        {
            return await _context.EventBookings
                .CountAsync(b => b.EventId == eventId && !b.IsCancelled);
        }
    }
}