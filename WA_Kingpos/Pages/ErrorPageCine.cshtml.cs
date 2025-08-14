using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WA_Kingpos.Pages
{
    public class ErrorPageCineModel : PageModel
    {
        [BindProperty]
        public string TextError { get; set; }
        public void OnGet(string id)
        {
            if (id == "1")
            {
                TextError = "Mã QR không chính xác";
            }
            else
            {
                TextError = id;
            }
        }
    }
}
