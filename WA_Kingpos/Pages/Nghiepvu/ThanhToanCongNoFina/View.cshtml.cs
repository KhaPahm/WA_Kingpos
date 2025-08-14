using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.Nghiepvu.ThanhToanCongNoFina
{
    [Authorize]
    public class ViewModel : PageModel
    {
        public PhieuDuyetThu PHIEUDUYETTHU = new PhieuDuyetThu();

        public List<DONDATModels> ListDONDAT = new List<DONDATModels>();
        public IActionResult OnGet(string id)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowAdd("14102304", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                LoadData(id);

                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "ThanhToanCongNoFina view", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        private void LoadData(string id)
        {
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "EXEC SP_FINA_SELECT_DATA @LOAI=66, @MAPHIEU=" + cls_Main.SQLString(id);
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            PHIEUDUYETTHU.SO_PHIEU = dt.Rows[0]["MAPDNDT"].ToString();
            PHIEUDUYETTHU.NHAN_VIEN = dt.Rows[0]["TENNHANVIEN"].ToString();
            PHIEUDUYETTHU.NGAYLAPPHIEU = String.Format("{0:dd/MM/yyyy}", dt.Rows[0]["NGAYLAPPHIEU"]);
            PHIEUDUYETTHU.LY_DO = dt.Rows[0]["LYDO"].ToString();
            PHIEUDUYETTHU.TRANG_THAI = dt.Rows[0]["TRANGTHAI"].ToString();
            PHIEUDUYETTHU.SOTIEN = int.Parse(dt.Rows[0]["TONGTIEN"].ToString()).ToString("N0");
            //
            foreach (DataRow dr in dt.Rows)
            {
                DONDATModels item = new DONDATModels();
                item.PHIEUDATHANG = dr["HANGHOA"].ToString();
                item.NHANVIEN_DATHANG = dr["TENNHANVIEN_DH"].ToString();
                item.THANHTIEN = int.Parse(dr["DATRA"].ToString()).ToString("N0");
                item.KHACHHANG = dr["TEN"].ToString();
                item.NGAYLAP = String.Format("{0:dd/MM/yyyy}", dr["NGAYLAPPHIEU"]);
                item.STT = (1 + dt.Rows.IndexOf(dr)).ToString();
                ListDONDAT.Add(item);

            }
        }
    }
}
