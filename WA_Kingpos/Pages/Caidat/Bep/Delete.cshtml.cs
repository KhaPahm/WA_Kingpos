using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.Bep
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        public clsBep item = new clsBep();
        public IActionResult OnGet(string id)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowDelete("8", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string sSQL = "SELECT DM_BEP.*,CUAHANG.TEN_CUAHANG FROM DM_BEP INNER JOIN CUAHANG ON DM_BEP.MA_CUAHANG = CUAHANG.MA_CUAHANG WHERE MA_BEP=" + cls_Main.SQLString(id);
                DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                foreach (DataRow dr in dt.Rows)
                {
                    item.ma_bep = dr["MA_BEP"].ToString();
                    item.ten_bep = dr["TEN_BEP"].ToString();
                    item.mayinbep = dr["MAYINBEP"].ToString();
                    item.ip = dr["IP"].ToString();
                    item.ghichu = dr["GHICHU"].ToString();
                    item.sudung = dr["SUDUNG"].ToString();
                    item.ma_cuahang = dr["MA_CUAHANG"].ToString();
                    item.ten_cuahang = dr["TEN_CUAHANG"].ToString();
                }
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "DM_Bep Delete : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        public IActionResult Onpost(string id)
        {
            try
            {
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string sSQL = "";
                sSQL += "Delete From DM_BEP" + "\n";
                sSQL += "Where MA_BEP=" + cls_Main.SQLString(id) + "\n";
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
                cls_Main.WriterLog(cls_Main.sFilePath, "DM_Bep Delete : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
    }
}
