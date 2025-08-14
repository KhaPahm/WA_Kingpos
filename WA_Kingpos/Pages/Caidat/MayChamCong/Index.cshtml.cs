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
    public class IndexModel : PageModel
    {
        public const string Perm_CatdatMayChamCong = "202002131";

        public List<clsMayChamCong> listitem = new();
        //public bool bThem { get; set; }
        public bool bSua { get; set; }
        //public bool bXoa { get; set; }
        private readonly HttpClient _httpClient;

        public IndexModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IActionResult> OnGet()
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowView(Perm_CatdatMayChamCong, HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                else
                {
                    //bThem = cls_UserManagement.AllowAdd("23120802", HttpContext.Session.GetString("Permission"));
                    bSua = cls_UserManagement.AllowEdit(Perm_CatdatMayChamCong, HttpContext.Session.GetString("Permission"));
                    //bXoa = cls_UserManagement.AllowDelete("23120802", HttpContext.Session.GetString("Permission"));
                }
                //xu ly trang
                var url = new Uri(new Uri(cls_AppSettings.Host_BioPush), "data/device/onlinelist");
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var jsonData = await response.Content.ReadAsStringAsync();
                listitem = JsonConvert.DeserializeObject<List<clsMayChamCong>>(jsonData)!;
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "MayChamCong", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
    }
}
