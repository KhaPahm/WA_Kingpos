using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.MayChamCong
{
    [Authorize]
    public class ViewModel : PageModel
    {
        [BindProperty]
        public clsMayChamCong item { get; set; } = new();
        private readonly HttpClient _httpClient;

        [BindProperty]
        public string SerialNumber { get; set; } = "";

        public ViewModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IActionResult> OnGet(string id)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowView(IndexModel.Perm_CatdatMayChamCong, HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                await LoadData(id);
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "MayChamCong View : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }

        private async Task LoadData(string? id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                this.SerialNumber = id;
            }
            var url = new Uri(new Uri(cls_AppSettings.Host_BioPush), $"data/device/onlinelist?SN={SerialNumber}");
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var jsonData = await response.Content.ReadAsStringAsync();
            item = JsonConvert.DeserializeObject<List<clsMayChamCong>>(jsonData)!.First();

        }

    }
}
