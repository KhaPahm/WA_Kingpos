using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Runtime.Intrinsics.X86;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.Nhomhang
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        public clsNhomhang item = new clsNhomhang();
        public IActionResult OnGet(string id)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowDelete("6", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string sSQL = "";
                sSQL += "Select MA_NHOMHANG As MA,TEN_NHOMHANG As TEN,GHICHU,SUDUNG,THUCDON,STT,HINHANH,MONTHEM" + "\n";
                sSQL += "From NHOMHANG" + "\n";
                sSQL += "Where MA_NHOMHANG=" + cls_Main.SQLString(id) + "\n";
                DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                foreach (DataRow dr in dt.Rows)
                {
                    item.MA = dr["MA"].ToString();
                    item.TEN = dr["TEN"].ToString();
                    item.GHICHU = dr["GHICHU"].ToString();
                    item.SUDUNG = dr["SUDUNG"].ToString();
                    item.THUCDON = dr["THUCDON"].ToString();
                    item.STT = dr["STT"].ToString();
                    item.HINHANH = dr["HINHANH"] == DBNull.Value ? null : (byte[])dr["HINHANH"];
                    item.MONTHEM = dr["MONTHEM"].ToString();
                }
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "Nhomhang Delete : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        public IActionResult Onpost(string id)
        {
            try
            {
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string sSQL = "";
                sSQL += "Delete From NHOMHANG" + "\n";
                sSQL += "Where MA_NHOMHANG=" + cls_Main.SQLString(id) + "\n";
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
                cls_Main.WriterLog(cls_Main.sFilePath, "Nhomhang Delete : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
    }
}
