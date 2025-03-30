using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeManagement.Areas.Identity.Data;
using System.Threading.Tasks;

namespace OfficeManagement.Pages
{
    public class TermsModel : PageModel
    {
        [BindProperty]
        public bool AcceptTerms { get; set; }

        [BindProperty]
        public string ReturnUrl { get; set; }  // Bind returnUrl from the form

        public void OnGet(string returnUrl = null)
        {
            // Set the ReturnUrl property to the provided returnUrl or default to the Index page
            ReturnUrl = returnUrl ?? Url.Content("~/Index");
        }

        public async Task<IActionResult> OnPostAsync(
        [FromServices] SignInManager<OfficeUser> signInManager,
        [FromServices] UserManager<OfficeUser> userManager)
        {
            if (AcceptTerms)
            {
                // Refresh the sign-in to ensure the cookie is updated
                var user = await userManager.GetUserAsync(User);
                if (user != null)
                {
                    await signInManager.RefreshSignInAsync(user);
                }
                return LocalRedirect(ReturnUrl ?? Url.Content("~/Index"));
            }

            ModelState.AddModelError("", "You must accept the terms to continue.");
            return Page();
        }
    }
}