using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.Donvitinh
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public List<clsDonvitinh> listitem = new List<clsDonvitinh>();
        public bool bThem { get; set; }
        public bool bSua { get; set; }
        public bool bXoa { get; set; }
        public IActionResult OnGet()
        {
            try
            {
                //check quyen
                if (!cls_UserManagement .AllowView ("7",HttpContext.Session .GetString ("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                else
                {
                    bThem = cls_UserManagement.AllowAdd("7", HttpContext.Session.GetString("Permission"));
                    bSua = cls_UserManagement.AllowEdit("7", HttpContext.Session.GetString("Permission"));
                    bXoa = cls_UserManagement.AllowDelete("7", HttpContext.Session.GetString("Permission"));
                }
                //xu ly trang
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string sSQL = "Select * From DONVITINH";
                DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                foreach (DataRow dr in dt.Rows)
                {
                    clsDonvitinh item = new clsDonvitinh();
                    item.ma_donvitinh = dr["MA_DONVITINH"].ToString();
                    item.ten_donvitinh = dr["TEN_DONVITINH"].ToString();
                    item.ghichu = dr["GHICHU"].ToString();
                    item.sudung = dr["SUDUNG"].ToString();

                    listitem.Add(item);
                }
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "DM_Donvitinh", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage",new { id = ex.ToString() });
            }
        }
    }
}
