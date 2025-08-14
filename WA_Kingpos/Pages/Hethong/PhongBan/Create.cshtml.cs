using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.PhongBan
{
    [Authorize]
    public class CreateModel : PageModel
    {
        [BindProperty]
        public ClsPhongban item { get; set; }
        public IActionResult OnGet()
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowAdd("10", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "DM_CHUCVU Add", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        public IActionResult OnPost(ClsPhongban item)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string sSQL = "";
                sSQL += "Insert into DM_CHUCVU(TENCHUCVU,GHICHU,SUDUNG)" + "\n";
                sSQL += "Values ( ";
                sSQL += cls_Main.SQLStringUnicode(item.ten_phongban) + ",";
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
                cls_Main.WriterLog(cls_Main.sFilePath, "DM_CHUCVU Add", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
    }
}
