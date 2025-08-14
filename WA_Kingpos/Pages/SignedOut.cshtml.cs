using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WA_Kingpos.Data;

namespace WA_Kingpos.Pages
{
    [Authorize]
    public class SignedOutModel : PageModel
    {
        public IActionResult OnGet()
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    // Clear all data within TempData
                    TempData.Clear();
                    // Clear all data within ViewData
                    ViewData.Clear();
                    // Clear all session data
                    HttpContext.Session.Clear();
                    // Clear the existing external cookie
                    HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    // Redirect to home page if the user is authenticated.
                    return RedirectToPage("/Index");
                }

                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "SignedOut", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
           
        }
    }
}
