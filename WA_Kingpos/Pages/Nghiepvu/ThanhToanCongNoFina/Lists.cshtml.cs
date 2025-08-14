using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.Nghiepvu.ThanhToanCongNoFina
{
    [Authorize]
    public class ListsModel : PageModel
    {
        public List<PhieuDuyetThu> listitem = new List<PhieuDuyetThu>();
        public IActionResult OnGet()
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowView("14102304", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                //string nguoitao = User.FindFirst("Username")?.Value;
                string manhanvien = User.FindFirst("UserId")?.Value;
                string sSQL = "EXEC SP_FINA_SELECT_DATA @LOAI=65, @USERLOGIN =" + cls_Main.SQLString(manhanvien);
                DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                foreach (DataRow dr in dt.Rows)
                {
                    PhieuDuyetThu item = new PhieuDuyetThu();
                    item.SO_PHIEU = dr["MAPDNDT"].ToString();
                    item.NGAYLAPPHIEU = string.Format("{0:dd/MM/yyyy}", DateTime.Parse(dr["NGAYLAPPHIEU"].ToString()));
                    item.SOTIEN = decimal.Parse(dr["TONGTIEN"].ToString()).ToString("N0");
                    item.LY_DO = dr["LYDO"].ToString();
                    item.TRANG_THAI = dr["TRANGTHAI"].ToString();
                    item.TRANG_THAI1 = dr["TRANGTHAI1"].ToString();
                    item.NHAN_VIEN = dr["TENNHANVIEN"].ToString();
                    listitem.Add(item);
                }
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "ThanhToanCongNoFina List", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
    }
}
