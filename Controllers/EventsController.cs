// Controllers/EventsController.cs (for Employees)
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SCMS.DTOs;
using SCMS.Services;
using System.Threading.Tasks;
using SCMS.Models;
using System.Security.Claims;
using AutoMapper;

namespace SCMS.Controllers
{
    [Authorize(Roles = "Employee")]
    public class EventsController : Controller
    {
        private readonly IEventService _eventService;
        private readonly IMapper _mapper;

        public EventsController(IEventService eventService, IMapper mapper)
        {
            _eventService = eventService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var events = await _eventService.GetAllEventsAsync();
            return View(events);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventDTO eventDto)
        {
            if (ModelState.IsValid)
            {
                var eventModel = _mapper.Map<Event>(eventDto);
                //var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                await _eventService.CreateEventAsync(eventModel/*, userId*/);
                return RedirectToAction(nameof(Index));
            }
            return View(eventDto);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var eventModel = await _eventService.GetEventByIdAsync(id);
            if (eventModel == null)
            {
                return NotFound();
            }

            var eventDto = _mapper.Map<EventDTO>(eventModel);
            return View(eventDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EventDTO eventDto)
        {
            //if (id != eventDto.Id)
            //{
            //    return NotFound();
            //}

            if (ModelState.IsValid)
            {
                var eventModel = _mapper.Map<Event>(eventDto);
                await _eventService.UpdateEventAsync(eventModel);
                return RedirectToAction(nameof(Index));
            }
            return View(eventDto);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var eventModel = await _eventService.GetEventByIdAsync(id);
            if (eventModel == null)
            {
                return NotFound();
            }

            return View(eventModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _eventService.DeleteEventAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
