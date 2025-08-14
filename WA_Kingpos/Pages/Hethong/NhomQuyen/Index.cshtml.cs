using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.NhomQuyen
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public List<clsNhomquyen> listitem = new List<clsNhomquyen>();
        public bool bThem { get; set; }
        public bool bSua { get; set; }
        public bool bXoa { get; set; }
        public IActionResult OnGet()
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowView("2", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                else
                {
                    bThem = cls_UserManagement.AllowAdd("2", HttpContext.Session.GetString("Permission"));
                    bSua = cls_UserManagement.AllowEdit("2", HttpContext.Session.GetString("Permission"));
                    bXoa = cls_UserManagement.AllowDelete("2", HttpContext.Session.GetString("Permission"));
                }
                //xu ly trang
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string sSQL = "select * from SYS_GROUP order by GroupName";
                DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                foreach (DataRow dr in dt.Rows)
                {
                    clsNhomquyen item = new clsNhomquyen();
                    item.ma_nhomquyen = dr["GroupID"].ToString();
                    item.ten_nhomquyen = dr["GroupName"].ToString();
                    item.ghichu = dr["Description"].ToString();
                    item.sudung = dr["Active"].ToString();
                    listitem.Add(item);
                }
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "SYS_GROUP", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        
    }
}
