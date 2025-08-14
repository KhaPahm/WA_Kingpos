using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.NhomQuyen
{
    [Authorize]
    public class CreateModel : PageModel
    {
        [BindProperty]
        public clsNhomquyen item { get; set; }
        public IActionResult OnGet()
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowAdd("2", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "SYS_GROUP Add", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        public IActionResult OnPost(clsNhomquyen item)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string sSQL = "";
                sSQL += "Insert into SYS_GROUP(GroupName,Description,Active)" + "\n";
                sSQL += "Values ( ";
                sSQL += cls_Main.SQLStringUnicode(item.ten_nhomquyen) + ",";
                sSQL += cls_Main.SQLStringUnicode(item.ghichu) + ",";
                sSQL += cls_Main.SQLBit(bool.Parse(item.sudung)) + ")";
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
                cls_Main.WriterLog(cls_Main.sFilePath, "SYS_GROUP Add", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
    }
}
