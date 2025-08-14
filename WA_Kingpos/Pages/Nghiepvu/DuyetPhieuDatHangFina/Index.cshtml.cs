using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.DatHang
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public List<clsDathang> listitem = new List<clsDathang>();
        public IActionResult OnGet()
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowView("14102302", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string nguoitao = User.FindFirst("Username")?.Value;
                string manhanvien = User.FindFirst("UserId")?.Value;
                string sSQL = "EXEC SP_FINA_SELECT_DATA @LOAI=1, @USERLOGIN =" + cls_Main.SQLString(manhanvien);
                DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                foreach (DataRow dr in dt.Rows)
                {
                    clsDathang item = new clsDathang();
                    item.maphieu = dr["MAPDNDC"].ToString();
                    item.ngaylap = string.Format("{0:dd/MM/yyyy}", DateTime.Parse(dr["NGAYLAPPHIEU"].ToString()));
                    item.trangthai=dr["TRANGTHAI1"].ToString();
                    item.nhanvien = dr["TENNHANVIEN"].ToString();
                    item.khachhang = dr["TENKHACHHANG"].ToString();
                    item.kho = dr["TEN_KHO"].ToString();
                    item.sotien = decimal.Parse(dr["SOTIEN"].ToString()).ToString("N0");
                    listitem.Add(item);
                }
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "DuyetPhieuDatHangFina", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
    }
}
