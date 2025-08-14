using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Security.Cryptography;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.TaiXe
{
    [Authorize]
    public class TaixeViewModel : PageModel
    {
        public TaiXeModels item = new TaiXeModels();
        public bool bSua { get; set; }
        public bool bXoa { get; set; }
        public IActionResult OnGet(string id)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowView("23120802", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                else
                {
                    bSua = cls_UserManagement.AllowEdit("23120802", HttpContext.Session.GetString("Permission"));
                    bXoa = cls_UserManagement.AllowDelete("23120802", HttpContext.Session.GetString("Permission"));
                }
                //xu ly trang
                LoadData(id);
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "TaiXeFina View : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        private void LoadData(string id)
        {
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "EXEC SP_FINA_SELECT_DATA @LOAI=18, @MANHANVIEN=" + cls_Main.SQLString(id);
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                item.MA = dr["MA"].ToString();
                item.ID = dr["ID"].ToString();
                item.TEN = dr["TEN"].ToString();
                item.DIENTHOAI = dr["DIENTHOAI"].ToString();
                item.CMND = dr["CMND"].ToString();
                item.WEBSITE = dr["WEBSITE"].ToString();
                item.SUDUNG = dr["SUDUNG"].ToString();
            }
        }
    }
}
