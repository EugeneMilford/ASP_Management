using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace OfficeManagement.Tests.Pages.OfficeEvents
{
    public class OfficeEventsPageTests
    {
        private static string FindOfficeEventsPagePath()
        {
            var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (dir != null)
            {
                var candidate1 = Path.Combine(dir.FullName, "OfficeManagement", "Pages", "OfficeEvents", "Index.cshtml");
                if (File.Exists(candidate1)) return candidate1;

                var candidate2 = Path.Combine(dir.FullName, "OfficeManagement", "Pages", "OfficeEvents.cshtml");
                if (File.Exists(candidate2)) return candidate2;

                // also support Pages/Events/Index.cshtml variations
                var candidate3 = Path.Combine(dir.FullName, "OfficeManagement", "Pages", "Events", "Index.cshtml");
                if (File.Exists(candidate3)) return candidate3;

                dir = dir.Parent;
            }

            return null;
        }

        [Fact]
        public void OfficeEvents_RazorPage_FileExists()
        {
            var path = FindOfficeEventsPagePath();
            Assert.False(string.IsNullOrEmpty(path), "Could not find OfficeEvents razor page (expected OfficeManagement/Pages/OfficeEvents/Index.cshtml or OfficeManagement/Pages/OfficeEvents.cshtml).");
            Assert.True(File.Exists(path), $"OfficeEvents razor page not found at: {path}");
        }

        [Fact]
        public async Task OfficeEvents_RazorPage_ContainsTableHeaders_And_ActionLinks()
        {
            var path = FindOfficeEventsPagePath();
            Assert.False(string.IsNullOrEmpty(path), "Could not find OfficeEvents razor page.");

            var content = await File.ReadAllTextAsync(path);

            // Page uses Activity collection and displays EventName / EventDescription / EventTime / UsersAssigned
            Assert.Contains("Activity", content);
            Assert.Contains("Activity[0].EventName", content);
            Assert.Contains("Activity[0].EventDescription", content);
            Assert.Contains("Activity[0].EventTime", content);
            Assert.Contains("Activity[0].UsersAssigned", content);

            // Ensuring DisplayFor / DisplayNameFor usages exist for expected properties
            Assert.Contains("DisplayNameFor", content);
            Assert.Contains("DisplayFor(modelItem => item.EventName", content);
            Assert.Contains("DisplayFor(modelItem => item.EventDescription", content);

            // Action links for Edit / Details / Delete are present
            Assert.Contains("asp-page=\"./Edit\"", content);
            Assert.Contains("asp-page=\"./Details\"", content);
            Assert.Contains("asp-page=\"./Delete\"", content);

            // Add New Event button present
            Assert.Contains("asp-page=\"Create\"", content);
            Assert.Contains("Add New Event", content);
        }

        [Fact]
        public async Task OfficeEvents_RazorPage_ContainsTableStructure_And_CardElements()
        {
            var path = FindOfficeEventsPagePath();
            Assert.False(string.IsNullOrEmpty(path), "Could not find OfficeEvents razor page.");

            var content = await File.ReadAllTextAsync(path);

            // Verify the table container and styling classes expected by the UI
            Assert.Contains("table class=\"table", content);
            Assert.Contains("table-striped projects", content);
            Assert.Contains("card-title", content);
            Assert.Contains("Events", content); // page title or card title should contain Events
        }
    }
}