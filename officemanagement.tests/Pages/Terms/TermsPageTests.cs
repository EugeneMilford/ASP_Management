using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace OfficeManagement.Tests.Pages.Terms
{
    public class TermsPageTests
    {
        private static string FindTermsPagePath()
        {
            var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (dir != null)
            {
                // Common locations: Pages/Terms.cshtml or Pages/Account/Terms.cshtml or Pages/Shared/Terms.cshtml
                var candidates = new[]
                {
                    Path.Combine(dir.FullName, "OfficeManagement", "Pages", "Terms.cshtml"),
                    Path.Combine(dir.FullName, "OfficeManagement", "Pages", "Account", "Terms.cshtml"),
                    Path.Combine(dir.FullName, "OfficeManagement", "Pages", "Identity", "Terms.cshtml"),
                    Path.Combine(dir.FullName, "OfficeManagement", "Pages", "Shared", "Terms.cshtml"),
                    Path.Combine(dir.FullName, "OfficeManagement", "Pages", "Register", "Terms.cshtml"),
                    Path.Combine(dir.FullName, "OfficeManagement", "Pages", "Account", "RegisterConfirmation.cshtml") // fallback
                };

                foreach (var candidate in candidates)
                {
                    if (File.Exists(candidate)) return candidate;
                }

                dir = dir.Parent;
            }

            return null;
        }

        [Fact]
        public void Terms_RazorPage_FileExists()
        {
            var path = FindTermsPagePath();
            Assert.False(string.IsNullOrEmpty(path), "Could not find Terms razor page (expected OfficeManagement/Pages/Terms.cshtml or Pages/Account/Terms.cshtml).");
            Assert.True(File.Exists(path), $"Terms razor page not found at: {path}");
        }

        [Fact]
        public async Task Terms_RazorPage_ContainsAcceptCheckbox_AndContinueButton()
        {
            var path = FindTermsPagePath();
            Assert.False(string.IsNullOrEmpty(path), "Could not find Terms razor page.");

            var content = await File.ReadAllTextAsync(path);

            // Ensure there is a checkbox bound to AcceptTerms
            Assert.Contains("asp-for=\"AcceptTerms\"", content);
            Assert.Contains("id=\"AcceptTerms\"", content);

            // Ensure there is a hidden ReturnUrl input
            Assert.Contains("asp-for=\"ReturnUrl\"", content);
            Assert.Contains("type=\"hidden\"", content);

            // Ensure form method is POST and a submit button exists
            Assert.Contains("<form method=\"post\"", content, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("Continue", content);

            // Basic content presence checks
            Assert.Contains("Terms and Conditions", content);
            Assert.Contains("Welcome to our Office Management System", content);
            Assert.Contains("Privacy Policy", content, StringComparison.OrdinalIgnoreCase);

            // Image reference (optional)
            Assert.Contains("img/terms-image.jpg", content, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task Terms_RazorPage_ShowsValidationPlaceholder()
        {
            var path = FindTermsPagePath();
            Assert.False(string.IsNullOrEmpty(path), "Could not find Terms razor page.");

            var content = await File.ReadAllTextAsync(path);

            // Page shows the ModelState validation area for errors (checks for ModelState usage)
            Assert.Contains("ModelState", content);
            Assert.Contains("error-message", content);
        }
    }
}
