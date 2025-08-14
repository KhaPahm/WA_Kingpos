using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Data;
using System.Net;
using System.Text;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.DangKyKhuonMatZk
{
    [Authorize]
    public class CreateModel : PageModel
    {
        //[BindProperty]


        private readonly HttpClient _httpClient;

        /// <summary>
        /// keep index state
        /// </summary>
        [BindProperty]
        public bool bOnlyNoFace { get; set; } = true;

        public CreateModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public clsKhachHangDangKyFace item { get; set; } = new();



        public IActionResult OnGet(string? id1, bool? id2)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowAdd(IndexModel.Perm_DangKyKhuonMatZk, HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                if (id2 != null)
                {
                    this.bOnlyNoFace = Convert.ToBoolean(id2);
                }
                LoadData(id1);

                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "DangKyKhuonMatZk Add", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        public async Task<IActionResult> OnPost(clsKhachHangDangKyFace item)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    //xu ly trang
                    LoadData(item.FACE_ID);
                    return Page();
                }

                var url = new Uri(new Uri(cls_AppSettings.Host_BioPush), "data/user/addPhoto");
                var body = new { Pin = item.FACE_ID, content = item.FACE_PHOTO };
                var stringContent = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(url, stringContent);
                response.EnsureSuccessStatusCode();
                var paramRedirect = new { id1 = item.FACE_ID };
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var jsonData = await response.Content.ReadAsStringAsync();
                    var responseData = JsonConvert.DeserializeObject<ApiResponseInfo>(jsonData)!;
                    if (responseData.Code > 0)
                    {
                        // success, save database
                        string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                        string sSQL = "";
                        sSQL += $"EXEC dbo.sp_Add_zkFaceDataKhachHang @Pin = {ExtensionObject.toSqlPar(body.Pin)}, @face_photo = {ExtensionObject.toSqlPar(body.content)}";
                        bool bRunSQL = cls_Main.ExecuteSQL(sSQL, sConnectionString_live);
                        if (bRunSQL)
                        {
                            TempData["AlertMessage"] = "Lưu thành công";
                            TempData["AlertType"] = "alert-success";
                            return RedirectToPage("Index", new { id1 = bOnlyNoFace });
                        }
                        else
                        {
                            return RedirectToPage("/ErrorPage", new { id = "Lưu không thành công : " + sSQL });
                        }
                    }
                    else
                    {
                        TempData["AlertMessage"] = $"Lỗi: {responseData.Message} ({responseData.Code})";
                        TempData["AlertType"] = "alert-danger";
                        return RedirectToPage("Create", paramRedirect);
                    }
                }
                else
                {
                    TempData["AlertMessage"] = $"Lỗi kết nối máy chủ: {response.StatusCode}";
                    TempData["AlertType"] = "alert-danger";
                    return RedirectToPage("Create", paramRedirect);
                }
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "DangKyKhuonMatZk Add", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }

        private void LoadData(string? id1)
        {

            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = $"SELECT * FROM dbo.fn_View_DISPLAY_ORDER_FACE('0','','') "
                        + $" WHERE FACE_ID = {id1}";
            var dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            item = dt.ToList<clsKhachHangDangKyFace>().First();
            item.FACE_PHOTO = "";
        }

    }
}
