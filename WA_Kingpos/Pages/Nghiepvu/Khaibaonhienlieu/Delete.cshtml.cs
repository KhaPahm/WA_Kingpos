using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WA_Kingpos.Data;

namespace WA_Kingpos.Pages.Nghiepvu.Khaibaonhienlieu
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        public IActionResult OnGet(string id)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowDelete("24030608", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string sSQL = "";
                sSQL += "Update DM_DOITUONG_BCNL Set " + "\n";
                sSQL += "NGUOISUA=" + cls_Main.SQLString(User.FindFirst("Username")?.Value) + ",";
                sSQL += "NGAYSUA=GETDATE()" + ",";
                sSQL += "SUDUNG=" + cls_Main.SQLString("0") + "\n";
                sSQL += "Where ID=" + cls_Main.SQLString(id) + "\n";
                bool bRunSQL = cls_Main.ExecuteSQL(sSQL, sConnectionString_live);
                if (bRunSQL)
                {
                    return RedirectToPage("Index");
                }
                else
                {
                    return RedirectToPage("/ErrorPage", new { id = "Lưu không thành công : " + sSQL });
                }
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "khaibaosudungnguyenlieu Delete", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
    }
}
