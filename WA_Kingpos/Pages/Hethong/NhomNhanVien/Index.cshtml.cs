using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.NhomNhanVien
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public const string PermNhomNhanVien = "11";

        public List<clsNhomNhanVien> listitem = new();
        public bool bThem { get; set; }
        public bool bSua { get; set; }
        public bool bXoa { get; set; }
        public IActionResult OnGet()
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowView(PermNhomNhanVien, HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                else
                {
                    bThem = cls_UserManagement.AllowAdd(PermNhomNhanVien, HttpContext.Session.GetString("Permission"));
                    bSua = cls_UserManagement.AllowEdit(PermNhomNhanVien, HttpContext.Session.GetString("Permission"));
                    bXoa = cls_UserManagement.AllowDelete(PermNhomNhanVien, HttpContext.Session.GetString("Permission"));
                }
                //xu ly trang
                listitem = LoadList();
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "SYS_GROUP", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }

        public static List<clsNhomNhanVien> LoadList(string? id = null)
        {
            // SELECT * FROM dbo.DM_NHANVIEN WHERE TRANGTHAI <> 2 ORDER BY TENNHANVIEN
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = $"Select * From dbo.fn_View_NhomNhanVien() ";
            if (!string.IsNullOrEmpty(id))
            {
                sSQL += $" Where MANHOM={ExtensionObject.toSqlPar(id)}";
            }
            sSQL += $" ORDER BY TENNHOM_NHANVIEN";
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            return dt.ToList<clsNhomNhanVien>();
        }

    }
}
