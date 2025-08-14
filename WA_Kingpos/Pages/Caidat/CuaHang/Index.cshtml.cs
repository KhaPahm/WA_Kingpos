using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.CuaHang
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public List<Cuahang> listitem = new List<Cuahang>();
        public bool bThem { get; set; }
        public bool bSua { get; set; }
        public bool bXoa { get; set; }
        public IActionResult OnGet()
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
                    bThem = cls_UserManagement.AllowAdd("4", HttpContext.Session.GetString("Permission"));
                    bSua = cls_UserManagement.AllowEdit("4", HttpContext.Session.GetString("Permission"));
                    bXoa = cls_UserManagement.AllowDelete("4", HttpContext.Session.GetString("Permission"));
                }
                //xu ly trang
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string sSQL = "Select * From CUAHANG";
                DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                foreach (DataRow dr in dt.Rows)
                {
                    Cuahang item = new Cuahang();
                    item.ma_cuahang = dr["MA_CUAHANG"].ToString();
                    item.ten_cuahang = dr["TEN_CUAHANG"].ToString();
                    item.ghichu = dr["GHICHU"].ToString();
                    item.sudung = dr["SUDUNG"].ToString();
                    item.header = dr["Header"].ToString();
                    item.footer = dr["Footer"].ToString();
                    item.header_ve = dr["Header_Ve"].ToString();
                    item.footer_ve = dr["Footer_Ve"].ToString();
                    item.image_ve = dr["Image_Ve"].ToString();
                    item.madiadiem = dr["MADIADIEM"].ToString();

                    listitem.Add(item);
                }
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "CUAHANG", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
    }
}
