// Services/EventService.cs
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SCMS.IRepo;
using SCMS.Models;
using SCMS.Repos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCMS.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepo _eventRepo;
        private readonly IEventBookingRepo _eventBookingRepo;
        private readonly IMapper _mapper;

        public EventService(IEventRepo eventRepo, IEventBookingRepo eventBookingRepo, IMapper mapper)
        {
            _eventRepo = eventRepo;
            _eventBookingRepo = eventBookingRepo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Event>> GetAllEventsAsync()
        {
            return await _eventRepo.GetAllAsync();
        }

        public async Task<IEnumerable<Event>> GetActiveEventsAsync()
        {
            return await _eventRepo.GetActiveEventsAsync(
                e => e.Include(ev => ev.Bookings));
        }

        public async Task<Event> GetEventByIdAsync(int id)
        {
            return await _eventRepo.GetByIdAsync(id,
                e => e.Include(ev => ev.Bookings));
        }

        public async Task CreateEventAsync(Event eventModel/*, int userId*/)
        {
            //eventModel.CreatedBy = userId;
            await _eventRepo.AddAsync(eventModel);
        }

        public async Task UpdateEventAsync(Event eventModel)
        {
            await _eventRepo.UpdateAsync(eventModel);
        }

        public async Task DeleteEventAsync(int id)
        {
            await _eventRepo.DeleteAsync(id);
        }

        public async Task<bool> BookEventAsync(int eventId, int studentId)
        {
            var eventModel = await _eventRepo.GetByIdAsync(eventId);
            if (eventModel == null || !eventModel.IsActive)
                return false;

            var bookingCount = await _eventRepo.GetBookingCountAsync(eventId);
            if (bookingCount >= eventModel.Capacity)
                return false;

            var hasBooking = await _eventRepo.HasBookingAsync(eventId, studentId);
            if (hasBooking)
                return false;

            var booking = new EventBooking
            {
                EventId = eventId,
                StudentId = studentId
            };

            await _eventBookingRepo.AddAsync(booking);
            return true;
        }

        public async Task<bool> CancelBookingAsync(int eventId, int studentId)
        {
            var booking = await _eventBookingRepo.GetBookingByEventAndStudentAsync(eventId, studentId);
            if (booking == null || booking.IsCancelled)
                return false;

            booking.IsCancelled = true;
            await _eventBookingRepo.UpdateAsync(booking);
            return true;
        }

        public async Task<IEnumerable<EventBooking>> GetStudentBookingsAsync(int studentId)
        {
            return await _eventBookingRepo.GetBookingsByStudentAsync(studentId,
                b => b.Include(book => book.Event));
        }

        public async Task<int> GetEventBookingCountAsync(int eventId)
        {
            return await _eventRepo.GetBookingCountAsync(eventId);
        }

        public async Task<bool> HasStudentBookedEventAsync(int eventId, int studentId)
        {
            return await _eventRepo.HasBookingAsync(eventId, studentId);
        }
    }
}