using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.Nhanvien
{
    [Authorize]
    public class ViewModel : PageModel
    {
        public clsNhanvien item = new clsNhanvien();
        public bool bSua { get; set; }
        public bool bXoa { get; set; }
        public IActionResult OnGet(string id)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowView("11", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                else
                {
                    bSua = cls_UserManagement.AllowEdit("11", HttpContext.Session.GetString("Permission"));
                    bXoa = cls_UserManagement.AllowDelete("11", HttpContext.Session.GetString("Permission"));
                }
                //xu ly trang
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string sSQL = "EXEC SP_W_GETNHANVIEN " + cls_Main.SQLString(id);
                DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                foreach (DataRow dr in dt.Rows)
                {
                    item.manhanvien = dr["MANHANVIEN"].ToString();
                    item.tennhanvien = dr["TENNHANVIEN"].ToString();
                    item.ngaysinh = dr["NGAYSINH"].ToString();
                    item.gioitinh = dr["GIOITINH"].ToString();
                    item.diachi = dr["DIACHI"].ToString();
                    item.dienthoai = dr["DIENTHOAI"].ToString();
                    item.email = dr["EMAIL"].ToString();
                    item.phongban = dr["TENCHUCVU"].ToString();
                    item.ghichu = dr["GHICHU"].ToString();
                    item.sudung = dr["SUDUNG"].ToString();

                    item.NGAYNHANVIEC = dr["NGAYNHANVIEC"]?.ToStrDate();
                    //item.NGUOIQUANLY = dr["NGUOIQUANLY"]?.ChangeType<int?>();
                    //item.TEN_NGUOIQUANLY = dr["TEN_NGUOIQUANLY"]?.ChangeType<string?>();
                    item.TRANGTHAI = dr["TRANGTHAI"]?.ChangeType<int?>();
                }
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "DM_NHANVIEN", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }

    }
}
