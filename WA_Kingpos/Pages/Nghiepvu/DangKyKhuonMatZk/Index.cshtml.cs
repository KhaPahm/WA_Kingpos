using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.DangKyKhuonMatZk
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public const string Perm_DangKyKhuonMatZk = "202002131";

        public List<clsKhachHangDangKyFace> listitem = new();
        public bool bThem { get; set; }
        public bool bXoa { get; set; }

        [BindProperty]
        public bool bOnlyNoFace { get; set; } = true;

        public IActionResult OnGet(bool? id1)
        {
            try
            {
                //check quyen
                var quyen = CheckQuyen();
                if (quyen != null)
                {
                    return quyen;
                }
                //xu ly trang
                LoadData(id1);
                //listitem.Clear();
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "DangKyKhuonMatZk", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }

        public IActionResult OnPost(string submitButton, [FromForm] string? face_id)
        {
            //check quyen
            var quyen = CheckQuyen();
            if (quyen != null)
            {
                return quyen;
            }
            var paramRedirect = new { id1 = bOnlyNoFace };
            if (submitButton == "view")
            {
                return RedirectToPage("Index", paramRedirect);
            }
            if (submitButton == "import")
            {
                return RedirectToPage("Import", paramRedirect);
            }
            else if (submitButton == "delete")
            {
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                var Username = User.Claims.FirstOrDefault(c => c.Type == "Username")?.Value;
                string sSQL = $"UPDATE dbo.DISPLAY_ORDER_FACE SET TRANGTHAI = 0, NGUOISUA = {ExtensionObject.toSqlPar(Username)}, NGAYSUA = GETDATE() WHERE FACE_ID = {ExtensionObject.toSqlPar(face_id)}";
                bool bRunSQL = cls_Main.ExecuteSQL(sSQL, sConnectionString_live);
                if (bRunSQL)
                {

                    TempData["AlertMessage"] = "Xóa thành công";
                    TempData["AlertType"] = "alert-success";
                }
                else
                {
                    TempData["AlertMessage"] = $"Xóa không thành công";
                    TempData["AlertType"] = "alert-danger";
                }
                return RedirectToPage("Index", paramRedirect);
            }
            return RedirectToPage("/AccessDenied");
        }



        private IActionResult? CheckQuyen()
        {
            if (!cls_UserManagement.AllowView(Perm_DangKyKhuonMatZk, HttpContext.Session.GetString("Permission")))
            {
                return RedirectToPage("/AccessDenied");
            }
            else
            {
                bThem = cls_UserManagement.AllowAdd(Perm_DangKyKhuonMatZk, HttpContext.Session.GetString("Permission"));
                //bSua = cls_UserManagement.AllowEdit(Perm_DangKyKhuonMatZk, HttpContext.Session.GetString("Permission"));
                bXoa = cls_UserManagement.AllowDelete(Perm_DangKyKhuonMatZk, HttpContext.Session.GetString("Permission"));
            }
            return null;
        }

        private void LoadData(bool? id1)
        {
            if (id1 != null)
            {
                bOnlyNoFace = Convert.ToBoolean(id1);
            }

            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            //  AND IS_ADD_FACE = 0 IS_ADD = 1 
            string sSQL = $"SELECT * FROM dbo.fn_View_DISPLAY_ORDER_FACE('0','','') "
                    + $" WHERE TRANGTHAI = 1 AND NGAYKETTHUC >= GETDATE() ";
            if (bOnlyNoFace)
            {
                sSQL += " AND FACE_PHOTO IS NULL ";
            }
            sSQL += " ORDER BY TENPHONG, TEN_KH ";
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            listitem = dt.ToList<clsKhachHangDangKyFace>();
        }
    }
}
