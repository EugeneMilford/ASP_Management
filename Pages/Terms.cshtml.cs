using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OfficeManagement.Pages
{
    public class TermsModel : PageModel
    {
        [BindProperty]
        public string ReturnUrl { get; set; }  // Bind returnUrl from the form

        public async Task<IActionResult> OnPostAsync()
        {
            // If returnUrl is not provided, default to /Index
            ReturnUrl ??= Url.Content("~/Welcome");

            TempData["Notification"] = "Thank you for accepting the terms and conditions. Please go to the Profile page to complete your profile.";

            // Redirect to the specified returnUrl or default to /Index
            return Redirect(ReturnUrl);
        }
    }
}


