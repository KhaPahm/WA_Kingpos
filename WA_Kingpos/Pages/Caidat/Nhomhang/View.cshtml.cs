using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.Nhomhang
{
    [Authorize]
    public class ViewModel : PageModel
    {
        public clsNhomhang item = new clsNhomhang();
        public bool bSua { get; set; }
        public bool bXoa { get; set; }
        public IActionResult OnGet(string id)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowView("6", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                else
                {
                    bSua = cls_UserManagement.AllowEdit("6", HttpContext.Session.GetString("Permission"));
                    bXoa = cls_UserManagement.AllowDelete("6", HttpContext.Session.GetString("Permission"));
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
                cls_Main.WriterLog(cls_Main.sFilePath, "Nhomhang View : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
    }
}
