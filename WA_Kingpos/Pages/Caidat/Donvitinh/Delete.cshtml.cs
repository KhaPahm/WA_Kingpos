using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Runtime.Intrinsics.X86;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.Donvitinh
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        public clsDonvitinh item = new clsDonvitinh();
        public IActionResult OnGet(string id)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowDelete("7", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string sSQL = "Select * From DONVITINH Where MA_DONVITINH=" + cls_Main.SQLString(id);
                DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                foreach (DataRow dr in dt.Rows)
                {
                    item.ma_donvitinh = dr["MA_DONVITINH"].ToString();
                    item.ten_donvitinh = dr["TEN_DONVITINH"].ToString();
                    item.ghichu = dr["GHICHU"].ToString();
                    item.sudung = dr["SUDUNG"].ToString();
                }
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "DM_Donvitinh Delete : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        public IActionResult Onpost(string id)
        {
            try
            {
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string sSQL = "";
                sSQL += "Delete From DONVITINH" + "\n";
                sSQL += "Where MA_DONVITINH=" + cls_Main.SQLString(id) + "\n";
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
                cls_Main.WriterLog(cls_Main.sFilePath, "DM_Donvitinh Delete : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
    }
}
