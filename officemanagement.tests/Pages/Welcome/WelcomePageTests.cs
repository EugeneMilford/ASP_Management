using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace OfficeManagement.Tests.Pages.Welcome
{
    public class WelcomePageTests
    {
        private static string FindWelcomePagePath()
        {
            var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (dir != null)
            {
                var candidate1 = Path.Combine(dir.FullName, "OfficeManagement", "Pages", "Welcome.cshtml");
                if (File.Exists(candidate1)) return candidate1;

                var candidate2 = Path.Combine(dir.FullName, "OfficeManagement", "Pages", "Index.cshtml"); // some projects use Index as landing
                if (File.Exists(candidate2))
                {
                    var content = File.ReadAllText(candidate2);
                    if (content.Contains("Welcome to Office Management")) return candidate2;
                }

                var candidate3 = Path.Combine(dir.FullName, "OfficeManagement", "Pages", "Home", "Welcome.cshtml");
                if (File.Exists(candidate3)) return candidate3;

                dir = dir.Parent;
            }

            return null;
        }

        [Fact]
        public void Welcome_RazorPage_FileExists()
        {
            var path = FindWelcomePagePath();
            Assert.False(string.IsNullOrEmpty(path), "Could not find Welcome razor page (expected OfficeManagement/Pages/Welcome.cshtml or similar).");
            Assert.True(File.Exists(path), $"Welcome razor page not found at: {path}");
        }

        [Fact]
        public async Task Welcome_RazorPage_ContainsHeroAndDemoButtons()
        {
            var path = FindWelcomePagePath();
            Assert.False(string.IsNullOrEmpty(path), "Could not find Welcome razor page.");

            var content = await File.ReadAllTextAsync(path);

            // Basic page title and hero content
            Assert.Contains("Welcome to Office Management", content);
            Assert.Contains("hero-heading", content);

            // Demo login buttons must be present with correct names/values
            Assert.Contains("name=\"role\" value=\"DemoUser\"", content);
            Assert.Contains("name=\"role\" value=\"DemoAdmin\"", content);

            // Ensure partial for login is referenced
            Assert.Contains("_LoginPartial", content, StringComparison.OrdinalIgnoreCase);

            // Hero image reference
            Assert.Contains("img/hero-img.png", content, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task Welcome_RazorPage_ContainsForm_Post_DemoLogin_Handler()
        {
            var path = FindWelcomePagePath();
            Assert.False(string.IsNullOrEmpty(path), "Could not find Welcome razor page.");

            var content = await File.ReadAllTextAsync(path);

            // The form uses POST and specific handler 'DemoLogin'
            Assert.Contains("form method=\"post\"", content, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("asp-page-handler=\"DemoLogin\"", content, StringComparison.OrdinalIgnoreCase);
        }
    }
}