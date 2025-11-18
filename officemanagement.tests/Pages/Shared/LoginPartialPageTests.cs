using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeManagement.Tests.Pages.Shared
{
    public class LoginPartialPageTests
    {
        private static string FindLoginPartialPath()
        {
            var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (dir != null)
            {
                var candidate1 = Path.Combine(dir.FullName, "OfficeManagement", "Pages", "_LoginPartial.cshtml");
                if (File.Exists(candidate1)) return candidate1;

                var candidate2 = Path.Combine(dir.FullName, "OfficeManagement", "Pages", "Shared", "_LoginPartial.cshtml");
                if (File.Exists(candidate2)) return candidate2;

                var candidate3 = Path.Combine(dir.FullName, "OfficeManagement", "Views", "Shared", "_LoginPartial.cshtml");
                if (File.Exists(candidate3)) return candidate3;

                dir = dir.Parent;
            }

            return null;
        }

        [Fact]
        public void LoginPartial_FileExists()
        {
            var path = FindLoginPartialPath();
            Assert.False(string.IsNullOrEmpty(path), "Could not find _LoginPartial.cshtml (expected in Pages/ or Pages/Shared/ or Views/Shared/).");
            Assert.True(File.Exists(path), $"_LoginPartial.cshtml not found at: {path}");
        }

        [Fact]
        public async Task LoginPartial_Contains_Register_And_Login_Links_WhenNotSignedIn()
        {
            var path = FindLoginPartialPath();
            Assert.False(string.IsNullOrEmpty(path), "Could not find _LoginPartial.cshtml.");

            var content = await File.ReadAllTextAsync(path);

            // Register and Login links
            Assert.Contains("asp-page=\"/Account/Register\"", content.Replace("'", "\""), StringComparison.OrdinalIgnoreCase);
            Assert.Contains("asp-page=\"/Account/Login\"", content.Replace("'", "\""), StringComparison.OrdinalIgnoreCase);

            // Buttons have expected ids used in UI
            Assert.Contains("id=\"register\"", content);
            Assert.Contains("id=\"login\"", content);
        }

        [Fact]
        public async Task LoginPartial_Contains_Logout_Form_WhenSignedIn()
        {
            var path = FindLoginPartialPath();
            Assert.False(string.IsNullOrEmpty(path), "Could not find _LoginPartial.cshtml.");

            var content = await File.ReadAllTextAsync(path);

            // Signed-in branch contains a logout form posting to Identity logout page
            Assert.Contains("asp-page=\"/Account/Logout\"", content.Replace("'", "\""), StringComparison.OrdinalIgnoreCase);
            Assert.Contains("id=\"logout\"", content);
            Assert.Contains("Hello,", content); // greeting text that includes user name or email
        }
    }
}
