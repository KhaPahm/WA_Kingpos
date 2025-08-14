using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Data;
using System.Net;
using System.Text;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.MayChamCong
{
    [Authorize]
    public class EditModel : PageModel
    {
        [BindProperty]
        public clsMayChamCong item { get; set; } = new();
        private readonly HttpClient _httpClient;

        [BindProperty]
        public string SerialNumber { get; set; } = "";

        public EditModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IActionResult> OnGet(string id)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowEdit(IndexModel.Perm_CatdatMayChamCong, HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                await LoadData(id);
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "MayChamCong Edit : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        public async Task<IActionResult> OnPost()
        {
            if (!cls_UserManagement.AllowEdit(IndexModel.Perm_CatdatMayChamCong, HttpContext.Session.GetString("Permission")))
            {
                return RedirectToPage("/AccessDenied");
            }
            try
            {
                //if (item.ID == "")
                //{
                //    ModelState.AddModelError(string.Empty, "ID Khách Hàng Không Được Trống");
                //}
                //if (!CheckCMND(item))
                //{
                //    ModelState.AddModelError("item.CMND", "CMND Này Đã Tồn Tại");
                //}
                if (!ModelState.IsValid)
                {
                    await LoadData(SerialNumber);
                    return Page();
                }
                var url = new Uri(new Uri(cls_AppSettings.Host_BioPush), "data/remote/SetOptions");
                var body = new { SN = SerialNumber, item.IPAddress, item.GATEIPAddress, item.NetMask };
                var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
                var paramRedirect = new { id = SerialNumber };
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var jsonData = await response.Content.ReadAsStringAsync();
                    var responseData = JsonConvert.DeserializeObject<ApiResponseInfo>(jsonData)!;
                    if (responseData.Code >= 0)
                    {
                        // success
                        TempData["AlertMessage"] = "Lưu thành công";
                        TempData["AlertType"] = "alert-success";
                        return RedirectToPage("Index");
                    }
                    else
                    {
                        TempData["AlertMessage"] = $"Lỗi: {responseData.Message} ({responseData.Code})";
                        TempData["AlertType"] = "alert-danger";
                        return RedirectToPage("Edit", paramRedirect);
                    }
                }
                else
                {
                    TempData["AlertMessage"] = $"Lỗi kết nối máy chủ: {response.StatusCode}";
                    TempData["AlertType"] = "alert-danger";
                    return RedirectToPage("Edit", paramRedirect);
                }

            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "MayChamCong Edit : " + SerialNumber, ex.ToString(), "0");
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
