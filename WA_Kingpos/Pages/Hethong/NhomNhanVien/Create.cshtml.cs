using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.NhomNhanVien
{
    [Authorize]
    public class CreateModel : PageModel
    {
        //[BindProperty]
        public clsNhomNhanVien item { get; set; } = new();

        public List<clsNhanvien> listNhanVien = new();
        public IActionResult OnGet()
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowAdd(IndexModel.PermNhomNhanVien, HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                LoadData();
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "NhomNhanVien Add", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        public IActionResult OnPost(clsNhomNhanVien item, [FromForm(Name = "THEM_THANHVIEN")] List<string?> listThemThanhVien)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    LoadData();
                    return Page();
                }
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                var userId = User.Claims.FirstOrDefault(c => c.Type == "Username")?.Value;
                // chèn người quản lý vào nếu chưa có trong danh sách thành viên
                item.THANHVIEN = string.Join(",", listThemThanhVien.Concat(new List<string?> { item.MA_NGUOIQUANLY?.ToString() })
                    .Select(r => { uint ret; uint.TryParse(r, out ret); return ret; }).Where(r => r > 0).Distinct());

                string sSQL = @"INSERT INTO dbo.DM_NHOM_NHANVIEN(TENNHOM_NHANVIEN,MA_NGUOIQUANLY,THANHVIEN,NGUOITAO) "
                        + $" VALUES ({ExtensionObject.toSqlPar(item.TENNHOM_NHANVIEN)}, {ExtensionObject.toSqlPar(item.MA_NGUOIQUANLY)}"
                        + $", {ExtensionObject.toSqlPar(item.THANHVIEN)}, {ExtensionObject.toSqlPar(userId)})";
                bool bRunSQL = cls_Main.ExecuteSQL(sSQL, sConnectionString_live);
                if (bRunSQL)
                {
                    TempData["AlertMessage"] = "Lưu thành công";
                    TempData["AlertType"] = "alert-success";
                    return RedirectToPage("Create");
                }
                else
                {
                    return RedirectToPage("/ErrorPage", new { id = "Lưu không thành công : " + sSQL });
                }
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "NhomNhanVien Add", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }

        private void LoadData()
        {
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = $"SELECT * FROM dbo.DM_NHANVIEN WHERE TRANGTHAI <> 2 ORDER BY TENNHANVIEN ";
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            listNhanVien = dt.ToList<clsNhanvien>();
        }
    }
}
