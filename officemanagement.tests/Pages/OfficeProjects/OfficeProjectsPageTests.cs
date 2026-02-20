using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace OfficeManagement.Tests.Pages.OfficeProjects
{
    public class OfficeProjectsPageTests
    {
        private static string FindOfficeProjectsPagePath()
        {
            var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (dir != null)
            {
                var candidate1 = Path.Combine(dir.FullName, "OfficeManagement", "Pages", "OfficeProjects", "Index.cshtml");
                if (File.Exists(candidate1)) return candidate1;

                var candidate2 = Path.Combine(dir.FullName, "OfficeManagement", "Pages", "OfficeProjects.cshtml");
                if (File.Exists(candidate2)) return candidate2;

                // also support Pages/Projects/Index.cshtml variations
                var candidate3 = Path.Combine(dir.FullName, "OfficeManagement", "Pages", "Projects", "Index.cshtml");
                if (File.Exists(candidate3)) return candidate3;

                dir = dir.Parent;
            }

            return null;
        }

        [Fact]
        public void OfficeProjects_RazorPage_FileExists()
        {
            var path = FindOfficeProjectsPagePath();
            Assert.False(string.IsNullOrEmpty(path), "Could not find OfficeProjects razor page (expected OfficeManagement/Pages/OfficeProjects/Index.cshtml or OfficeManagement/Pages/OfficeProjects.cshtml).");
            Assert.True(File.Exists(path), $"OfficeProjects razor page not found at: {path}");
        }

        [Fact]
        public async Task OfficeProjects_RazorPage_ContainsTableHeaders_And_ActionLinks()
        {
            var path = FindOfficeProjectsPagePath();
            Assert.False(string.IsNullOrEmpty(path), "Could not find OfficeProjects razor page.");

            var content = await File.ReadAllTextAsync(path);

            // Page uses Project collection and displays ProjectName / Description / CreatedDate / ProjectUser
            Assert.Contains("Project", content);
            Assert.Contains("Project[0].ProjectName", content);
            Assert.Contains("Project[0].Description", content);
            Assert.Contains("Project[0].CreatedDate", content);
            Assert.Contains("Project[0].ProjectUser", content);

            // Ensuring DisplayNameFor / DisplayFor usages exist for expected properties
            Assert.Contains("DisplayNameFor", content);
            Assert.Contains("DisplayFor(modelItem => item.ProjectName", content);
            Assert.Contains("DisplayFor(modelItem => item.Description", content);

            // Action links for Edit / Details / Delete are present
            Assert.Contains("asp-page=\"./Edit\"", content);
            Assert.Contains("asp-page=\"./Details\"", content);
            Assert.Contains("asp-page=\"./Delete\"", content);

            // Add New Project button present
            Assert.Contains("asp-page=\"Create\"", content);
            Assert.Contains("Add New Project", content);
        }

        [Fact]
        public async Task OfficeProjects_RazorPage_ContainsTableStructure_And_CardElements()
        {
            var path = FindOfficeProjectsPagePath();
            Assert.False(string.IsNullOrEmpty(path), "Could not find OfficeProjects razor page.");

            var content = await File.ReadAllTextAsync(path);

            // Verify the table container and styling classes expected by the UI
            Assert.Contains("table class=\"table", content);
            Assert.Contains("table-striped projects", content);
            Assert.Contains("card-title", content);
            Assert.Contains("Projects", content); // page title or card title should contain Projects
        }
    }
}
