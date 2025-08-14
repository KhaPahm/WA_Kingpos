using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.CuaHang
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        public Cuahang item = new Cuahang();
        public IActionResult OnGet(string id)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowDelete("4", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                LoadData(id);
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "CUAHANG Delete : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        public IActionResult Onpost(string id)
        {
            try
            {
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string sSQL = "Select * From KHO Where MA_CUAHANG=" + cls_Main.SQLString(id);
                DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                if(dt.Rows.Count > 0)
                {
                    ViewData["Message"] = "Cửa hàng tồn tại kho không thể xóa.";
                    LoadData(id);
                    return Page();
                }    
                sSQL = "";
                sSQL += "Delete From CUAHANG" + "\n";
                sSQL += "Where MA_CUAHANG=" + cls_Main.SQLString(id) + "\n";
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
                cls_Main.WriterLog(cls_Main.sFilePath, "CUAHANG Delete : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        private void LoadData(string id)
        {
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "Select * From CUAHANG Where MA_CUAHANG=" + cls_Main.SQLString(id);
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                item.ma_cuahang = dr["MA_CUAHANG"].ToString();
                item.ten_cuahang = dr["TEN_CUAHANG"].ToString();
                item.ghichu = dr["GHICHU"].ToString();
                item.sudung = dr["SUDUNG"].ToString();
            }
        }

    }
}
