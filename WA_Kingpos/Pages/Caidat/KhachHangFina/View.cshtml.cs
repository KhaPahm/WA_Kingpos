using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Security.Cryptography;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.KHACHANG
{
    [Authorize]
    public class KhachHangViewModel : PageModel
    {
        public KhachHangModels item = new KhachHangModels();
        public bool bSua { get; set; }
        public bool bXoa { get; set; }
        public IActionResult OnGet(string id)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowView("23120801", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                else
                {
                    bSua = cls_UserManagement.AllowEdit("23120801", HttpContext.Session.GetString("Permission"));
                    bXoa = cls_UserManagement.AllowDelete("23120801", HttpContext.Session.GetString("Permission"));
                }
                //xu ly trang
                LoadData(id);
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "KhachHangFina View : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        private void LoadData(string id)
        {
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string manhanvien = User.FindFirst("UserId")?.Value;
            string sSQL = "EXEC SP_FINA_SELECT_DATA @LOAI=17, @MANHANVIEN=" + cls_Main.SQLString(id);
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                item.MA = dr["MA"].ToString();
                item.TEN = dr["TEN"].ToString();
                item.DIACHI = dr["DIACHI"].ToString();
                item.DIENTHOAI = dr["DIENTHOAI"].ToString();
                item.FAX = dr["FAX"].ToString();
                item.EMAIL = dr["EMAIL"].ToString();
                item.CMND = dr["CMND"].ToString();
                item.SUDUNG = dr["SUDUNG"].ToString();
            }
        }
    }
}
