// Repositories/IEventRepo.cs
using SCMS.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore.Query;

namespace SCMS.IRepo
{
    public interface IEventRepo : IBasicRepo<Event>
    {
        Task<IEnumerable<Event>> GetActiveEventsAsync(params Func<IQueryable<Event>, IIncludableQueryable<Event, object>>[] includes);
        Task<bool> HasBookingAsync(int eventId, int studentId);
        Task<int> GetBookingCountAsync(int eventId);
    }

}