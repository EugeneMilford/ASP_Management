using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OfficeManagement.Areas.Identity.Data;
using OfficeManagement.Data;
using OfficeManagement.Models;
using System.Threading.Tasks;

namespace OfficeManagement.Pages
{
    public class CalendarModel : PageModel
    {
        private readonly OfficeContext _context;
        private readonly UserManager<OfficeUser> _userManager;

        public CalendarModel(OfficeContext context, UserManager<OfficeUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetEventsAsync()
        {
            var events = await _context.CalendarEvents
                .Include(e => e.User)
                .Select(e => new
                {
                    id = e.CalendarId,
                    title = e.Title,
                    description = e.Description,
                    start = e.Start,
                    end = e.End,
                    userName = e.User.UserName,
                    userId = e.UserId
                })
                .ToListAsync();
            return new JsonResult(events);
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostAddEventAsync([FromBody] CalendarEvent eventData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            eventData.UserId = currentUser.Id;
            _context.CalendarEvents.Add(eventData);
            await _context.SaveChangesAsync();

            return new JsonResult(new { success = true, id = eventData.CalendarId });
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostUpdateEventAsync([FromBody] CalendarEvent eventData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var existingEvent = await _context.CalendarEvents
                .FirstOrDefaultAsync(e => e.CalendarId == eventData.CalendarId && e.UserId == currentUser.Id);

            if (existingEvent == null)
            {
                return NotFound(new { success = false, message = "Event not found or not authorized" });
            }

            existingEvent.Title = eventData.Title;
            existingEvent.Description = eventData.Description;
            existingEvent.Start = eventData.Start;
            existingEvent.End = eventData.End;

            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true });
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostDeleteEventAsync([FromBody] DeleteEventRequest request)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var existingEvent = await _context.CalendarEvents
                .FirstOrDefaultAsync(e => e.CalendarId == request.CalendarId && e.UserId == currentUser.Id);

            if (existingEvent == null)
            {
                return NotFound(new { success = false, message = "Event not found or not authorized" });
            }

            _context.CalendarEvents.Remove(existingEvent);
            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true });
        }
    }

    public class DeleteEventRequest
    {
        public int CalendarId { get; set; }
    }
}