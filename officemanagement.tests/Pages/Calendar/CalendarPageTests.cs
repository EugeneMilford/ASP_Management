using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace OfficeManagement.Tests.Pages.Calendar
{
    public class CalendarPageTests
    {
        private static string FindCalendarPagePath()
        {
            var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (dir != null)
            {
                var candidate = Path.Combine(dir.FullName, "OfficeManagement", "Pages", "Calendar.cshtml");
                if (File.Exists(candidate)) return candidate;

                candidate = Path.Combine(dir.FullName, "OfficeManagement", "Pages", "Calendar", "Index.cshtml");
                if (File.Exists(candidate)) return candidate;

                dir = dir.Parent;
            }

            return null;
        }

        [Fact]
        public void Calendar_RazorPage_FileExists()
        {
            var path = FindCalendarPagePath();
            Assert.False(string.IsNullOrEmpty(path), "Could not find Calendar razor page (expected OfficeManagement/Pages/Calendar.cshtml or OfficeManagement/Pages/Calendar/Index.cshtml).");
            Assert.True(File.Exists(path), $"Calendar razor page not found at: {path}");
        }

        [Fact]
        public async Task Calendar_RazorPage_ContainsCalendarDiv_And_EventHandlers()
        {
            var path = FindCalendarPagePath();
            Assert.False(string.IsNullOrEmpty(path), "Could not find Calendar razor page.");

            var content = await File.ReadAllTextAsync(path);

            // Basic structural checks
            Assert.Contains("id='calendar'", content.Replace("\"", "'"), StringComparison.OrdinalIgnoreCase);
            Assert.Contains("eventClick", content);
            Assert.Contains("select:", content);
            Assert.Contains("events: '/Calendar?handler=Events'", content);

            // AJAX handler URLs used by the page
            Assert.Contains("/Calendar?handler=AddEvent", content);
            Assert.Contains("/Calendar?handler=UpdateEvent", content);
            Assert.Contains("/Calendar?handler=DeleteEvent", content);

            // Anti-forgery token usage in the form submission
            Assert.Contains("__RequestVerificationToken", content);

            // External library references used to render the calendar
            Assert.Contains("fullcalendar", content, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("moment.js", content, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("bootstrap", content, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task Calendar_RazorPage_EventContentIncludesUserNameRendering()
        {
            var path = FindCalendarPagePath();
            Assert.False(string.IsNullOrEmpty(path), "Could not find Calendar razor page.");

            var content = await File.ReadAllTextAsync(path);

            // The page uses extendedProps.userName in eventContent — verifying that the snippet exists
            Assert.Contains("extendedProps.userName", content);
            Assert.Contains("By: ${userName}", content.Replace("\"", "'"), StringComparison.OrdinalIgnoreCase);
        }
    }
}
