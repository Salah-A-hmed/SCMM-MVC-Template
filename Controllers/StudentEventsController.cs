// Controllers/StudentEventsController.cs (for Students)
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SCMS.Services;
using System.Security.Claims;
using System.Threading.Tasks;
using SCMS.DTOs;
using AutoMapper;

namespace SCMS.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentEventsController : Controller
    {
        private readonly IEventService _eventService;
        private readonly IMapper _mapper;

        public StudentEventsController(IEventService eventService, IMapper mapper)
        {
            _eventService = eventService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var events = await _eventService.GetActiveEventsAsync();
            var studentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            ViewData["Id"] = studentId;
            //var eventDetails = _mapper.Map<IEnumerable<EventDetailsDTO>>(events);
            //foreach (var eventDetail in eventDetails)
            //{
            //    eventDetail.HasBooked = await _eventService.HasStudentBookedEventAsync(eventDetail.Id, studentId);
            //}

            return View(events);
        }

        public async Task<IActionResult> Book(int id)
        {
            var studentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var success = await _eventService.BookEventAsync(id, studentId);
            
            if (!success)
            {
                TempData["ErrorMessage"] = "Unable to book the event. It might be full or you've already booked it.";
            }
            else
            {
                ViewData["Id"] = studentId;
                TempData["SuccessMessage"] = "Event booked successfully!";
            }
            
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Cancel(int id)
        {
            var studentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var success = await _eventService.CancelBookingAsync(id, studentId);
            
            if (!success)
            {
                TempData["ErrorMessage"] = "Unable to cancel the booking.";
            }
            else
            {
                ViewData["Id"] = studentId;
                TempData["SuccessMessage"] = "Booking cancelled successfully!";
            }
            
            return RedirectToAction(nameof(MyBookings));
        }

        public async Task<IActionResult> MyBookings()
        {
            var studentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var bookings = await _eventService.GetStudentBookingsAsync(studentId);
            ViewData["Id"] = studentId;
            var bookingDtos = _mapper.Map<IEnumerable<EventBookingDTO>>(bookings);
            
            return View(bookingDtos);
        }
    }
}