using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WA_Kingpos.Pages
{
    public class ErrorPageModel : PageModel
    {
        public string RequestId { get; set; }
        public void OnGet(string id)
        {
            RequestId = id;
        }
    }
}
