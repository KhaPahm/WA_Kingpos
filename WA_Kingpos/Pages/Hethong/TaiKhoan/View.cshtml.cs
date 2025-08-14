using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.TaiKhoan
{
    [Authorize]
    public class ViewModel : PageModel
    {
        public clsTaikhoan item = new clsTaikhoan();
        public bool bSua { get; set; }
        public bool bXoa { get; set; }
        public bool bHuydangnhap { get; set; }
        public bool bTientamung { get; set; }
        public IActionResult OnGet(string id)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowView("19", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                else
                {
                    bSua = cls_UserManagement.AllowEdit("19", HttpContext.Session.GetString("Permission"));
                    bXoa = cls_UserManagement.AllowDelete("19", HttpContext.Session.GetString("Permission"));
                    bHuydangnhap = cls_UserManagement.AllowView("19", HttpContext.Session.GetString("Permission"));
                    bTientamung = cls_UserManagement.AllowView("19", HttpContext.Session.GetString("Permission"));
                }
                //xu ly trang
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
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "SYS_USER view", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }

    }
}
