using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace OfficeManagement.Tests.Pages.Tasks
{
    public class TasksPageTests
    {
        private static string FindTasksPagePath()
        {
            var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (dir != null)
            {
                var candidate1 = Path.Combine(dir.FullName, "OfficeManagement", "Pages", "Tasks", "Index.cshtml");
                if (File.Exists(candidate1)) return candidate1;

                var candidate2 = Path.Combine(dir.FullName, "OfficeManagement", "Pages", "Tasks.cshtml");
                if (File.Exists(candidate2)) return candidate2;

                // also support Pages/Assignments/Index.cshtml variations
                var candidate3 = Path.Combine(dir.FullName, "OfficeManagement", "Pages", "Assignments", "Index.cshtml");
                if (File.Exists(candidate3)) return candidate3;

                dir = dir.Parent;
            }

            return null;
        }

        [Fact]
        public void Tasks_RazorPage_FileExists()
        {
            var path = FindTasksPagePath();
            Assert.False(string.IsNullOrEmpty(path), "Could not find Tasks razor page (expected OfficeManagement/Pages/Tasks/Index.cshtml or OfficeManagement/Pages/Tasks.cshtml).");
            Assert.True(File.Exists(path), $"Tasks razor page not found at: {path}");
        }

        [Fact]
        public async Task Tasks_RazorPage_ContainsTableHeaders_And_ActionLinks()
        {
            var path = FindTasksPagePath();
            Assert.False(string.IsNullOrEmpty(path), "Could not find Tasks razor page.");

            var content = await File.ReadAllTextAsync(path);

            // Page uses Assignment collection and displays AssignmentName / AssignmentDescription / User / StartDate / EndDate
            Assert.Contains("Assignment", content);
            Assert.Contains("Assignment[0].AssignmentName", content);
            Assert.Contains("Assignment[0].AssignmentDescription", content);
            Assert.Contains("Assignment[0].StartDate", content);
            Assert.Contains("Assignment[0].EndDate", content);

            // Ensure DisplayNameFor / DisplayFor usages exist for expected properties
            Assert.Contains("DisplayNameFor", content);
            Assert.Contains("DisplayFor(modelItem => item.AssignmentName", content);
            Assert.Contains("DisplayFor(modelItem => item.AssignmentDescription", content);

            // Action links for Edit / Details / Delete are present
            Assert.Contains("asp-page=\"./Edit\"", content);
            Assert.Contains("asp-page=\"./Details\"", content);
            Assert.Contains("asp-page=\"./Delete\"", content);
        }

        [Fact]
        public async Task Tasks_RazorPage_ContainsCreateLink_And_TableStructure()
        {
            var path = FindTasksPagePath();
            Assert.False(string.IsNullOrEmpty(path), "Could not find Tasks razor page.");

            var content = await File.ReadAllTextAsync(path);

            // Ensure the Create link/button exists
            Assert.Contains("asp-page=\"Create\"", content);
            Assert.Contains("Create New", content, StringComparison.OrdinalIgnoreCase);

            // Verify the table container and styling classes expected by the UI
            Assert.Contains("table class=\"table", content);
            Assert.Contains("<thead", content);
            Assert.Contains("<tbody", content);
        }
    }
}
