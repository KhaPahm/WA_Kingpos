using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.Bep
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public List<clsBep> listitem = new List<clsBep>();
        public bool bThem { get; set; }
        public bool bSua { get; set; }
        public bool bXoa { get; set; }
        public IActionResult OnGet()
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowView("8", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                else
                {
                    bThem = cls_UserManagement.AllowAdd("8", HttpContext.Session.GetString("Permission"));
                    bSua = cls_UserManagement.AllowEdit("8", HttpContext.Session.GetString("Permission"));
                    bXoa = cls_UserManagement.AllowDelete("8", HttpContext.Session.GetString("Permission"));
                }
                //xu ly trang
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string sSQL = "SELECT DM_BEP.*,CUAHANG.TEN_CUAHANG FROM DM_BEP INNER JOIN CUAHANG ON DM_BEP.MA_CUAHANG = CUAHANG.MA_CUAHANG";
                DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                foreach (DataRow dr in dt.Rows)
                {
                    clsBep item = new clsBep();
                    item.ma_bep = dr["MA_BEP"].ToString();
                    item.ten_bep = dr["TEN_BEP"].ToString();
                    item.mayinbep = dr["MAYINBEP"].ToString();
                    item.ip = dr["IP"].ToString();
                    item.ghichu = dr["GHICHU"].ToString();
                    item.sudung = dr["SUDUNG"].ToString();
                    item.ma_cuahang = dr["MA_CUAHANG"].ToString();
                    item.ten_cuahang = dr["TEN_CUAHANG"].ToString();

                    listitem.Add(item);
                }
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "DM_Bep", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
    }
}
