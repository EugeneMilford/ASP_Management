using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace OfficeManagement.Tests.Pages.OfficeBugTracking
{
    public class OfficeBugTrackingPageTests
    {
        private static string FindBugTrackingPagePath()
        {
            var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (dir != null)
            {
                var candidate1 = Path.Combine(dir.FullName, "OfficeManagement", "Pages", "OfficeBugTracking", "Index.cshtml");
                if (File.Exists(candidate1)) return candidate1;

                var candidate2 = Path.Combine(dir.FullName, "OfficeManagement", "Pages", "OfficeBugTracking.cshtml");
                if (File.Exists(candidate2)) return candidate2;

                dir = dir.Parent;
            }

            return null;
        }

        [Fact]
        public void BugTracking_RazorPage_FileExists()
        {
            var path = FindBugTrackingPagePath();
            Assert.False(string.IsNullOrEmpty(path), "Could not find BugTracking razor page (expected OfficeManagement/Pages/OfficeBugTracking/Index.cshtml or OfficeManagement/Pages/OfficeBugTracking.cshtml).");
            Assert.True(File.Exists(path), $"BugTracking razor page not found at: {path}");
        }

        [Fact]
        public async Task BugTracking_RazorPage_ContainsTableAndActionLinks()
        {
            var path = FindBugTrackingPagePath();
            Assert.False(string.IsNullOrEmpty(path), "Could not find BugTracking razor page.");

            var content = await File.ReadAllTextAsync(path);

            // Ensuring the page binds to the BugTracking model 
            Assert.Contains("BugTracking", content);

            // Ensuring the table headers use the DisplayNameFor helpers for the expected properties
            Assert.Contains("BugTracking[0].Title", content);
            Assert.Contains("BugTracking[0].Priority", content);
            Assert.Contains("BugTracking[0].Project", content);
            Assert.Contains("BugTracking[0].Description", content);
            Assert.Contains("BugTracking[0].CreatedDate", content);
            Assert.Contains("BugTracking[0].Status", content);
            Assert.Contains("BugTracking[0].Submitter", content);

            // Ensuring action links for Edit / Details / Delete exist
            Assert.Contains("asp-page=\"./Edit\"", content);
            Assert.Contains("asp-page=\"./Details\"", content);
            Assert.Contains("asp-page=\"./Delete\"", content);

            // Ensuring the Create/Add New Ticket button (shown for admins/demo admins) is present
            Assert.Contains("asp-page=\"./Create\"", content);

            // Ensuring role/authorization checks or flags are present in the markup
            Assert.Contains("isAdmin", content);
            Assert.Contains("isDemoAdmin", content);
        }

        [Fact]
        public async Task BugTracking_RazorPage_ContainsSearchAndTableContainer()
        {
            var path = FindBugTrackingPagePath();
            Assert.False(string.IsNullOrEmpty(path), "Could not find BugTracking razor page.");

            var content = await File.ReadAllTextAsync(path);

            // Search input and table container checks
            Assert.Contains("input type=\"text\" name=\"table_search\"", content.Replace("'", "\""));
            Assert.Contains("table class=\"table", content);
            Assert.Contains("table-responsive", content);
            Assert.Contains("Open Projects", content);
        }
    }
}
