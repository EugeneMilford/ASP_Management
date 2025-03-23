using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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

        public IActionResult OnPost()
        {
            if (AcceptTerms)
            {
                // Log that terms were accepted
                Console.WriteLine("Terms accepted!");
                return LocalRedirect("~/Index");
            }

            // Log that terms were not accepted
            Console.WriteLine("Terms were not accepted! Returning to the same page.");
            ModelState.AddModelError("", "You must accept the terms to continue.");
            return Page();
        }
    }
}
