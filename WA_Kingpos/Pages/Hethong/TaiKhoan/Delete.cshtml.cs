using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.TaiKhoan
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        public clsTaikhoan item = new clsTaikhoan();
        public IActionResult OnGet(string id)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowDelete("19", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                LoadData(id);
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "SYS_USER Delete : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        private void LoadData(string id)
        {
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "EXEC SP_W_GETTAIKHOAN " + cls_Main.SQLString(id);
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                item.userid = dr["UserId"].ToString();
                item.nhomquyen = dr["GroupName"].ToString();
                item.nhanvien = dr["TENNHANVIEN"].ToString();
                item.ngaysinh = dr["NGAYSINH"].ToString();
                item.gioitinh = dr["GIOITINH"].ToString();
                item.diachi = dr["DIACHI"].ToString();
                item.dienthoai = dr["DIENTHOAI"].ToString();
                item.sudung = dr["Active"].ToString();
                item.trangthai = dr["TRANGTHAI"].ToString();
                item.thietbi = dr["PCName"].ToString();
                item.thoiggian = dr["THOIGIAN"].ToString();
                item.tientamung = dr["TamUng"].ToString();
            }
        }
        public IActionResult Onpost(string id)
        {
            try
            {
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string sSQL = "";              
                sSQL += "Delete From SYS_USER" + "\n";
                sSQL += "Where UserID=" + cls_Main.SQLString(id) + "\n";
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
                cls_Main.WriterLog(cls_Main.sFilePath, "SYS_USER Delete : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
    }
}
