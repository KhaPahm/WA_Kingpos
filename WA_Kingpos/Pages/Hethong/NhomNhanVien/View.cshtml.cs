using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.NhomNhanVien
{
    [Authorize]
    public class ViewModel : PageModel
    {
        public clsNhomNhanVien item = new();
        public bool bSua { get; set; }
        public bool bXoa { get; set; }
        public IActionResult OnGet(string id)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowView(IndexModel.PermNhomNhanVien, HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                else
                {
                    bSua = cls_UserManagement.AllowEdit(IndexModel.PermNhomNhanVien, HttpContext.Session.GetString("Permission"));
                    bXoa = cls_UserManagement.AllowDelete(IndexModel.PermNhomNhanVien, HttpContext.Session.GetString("Permission"));
                }
                //xu ly trang
                item = IndexModel.LoadList(id).First();
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "SYS_GROUP View : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
    }
}
