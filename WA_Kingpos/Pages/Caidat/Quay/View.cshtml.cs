using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.Quay
{
    [Authorize]
    public class ViewModel : PageModel
    {
        public clsQuay item = new clsQuay();
        public bool bSua { get; set; }
        public bool bXoa { get; set; }
        public IActionResult OnGet(string id)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowView("4", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                else
                {
                    bSua = cls_UserManagement.AllowEdit("4", HttpContext.Session.GetString("Permission"));
                    bXoa = cls_UserManagement.AllowDelete("4", HttpContext.Session.GetString("Permission"));
                }
                //xu ly trang
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string sSQL = "select Q.*, K.TEN_KHO,C.TEN_CUAHANG from QUAY Q, KHO K , CUAHANG C where K.MA_KHO = Q.MA_KHO and K.MA_CUAHANG=C.MA_CUAHANG AND MA_QUAY =" + cls_Main.SQLString(id);
                DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                foreach (DataRow dr in dt.Rows)
                {
                    item.ma_quay = dr["MA_QUAY"].ToString();
                    item.ten_quay = dr["TEN_QUAY"].ToString();
                    item.ma_kho = dr["MA_KHO"].ToString();
                    item.ten_kho = dr["TEN_KHO"].ToString();
                    item.ten_cuahang = dr["TEN_CUAHANG"].ToString();
                    item.ghichu = dr["GHICHU"].ToString();
                    item.sudung = dr["SUDUNG"].ToString();
                }
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "QUAY View : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
    }
}
