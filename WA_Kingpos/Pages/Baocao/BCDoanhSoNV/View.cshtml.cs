using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.Baocao.BCDoanhSoNV
{
    [Authorize]
    public class ViewModel : PageModel
    {
        public clsDathang item = new clsDathang();

        public List<clsDathang> listitem = new List<clsDathang>();
        public int? tongsoluong { get; set; } = 0;
        public long? tongthanhtien { get; set; } = 0;
        public IActionResult OnGet(string id)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowAdd("23120902", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                LoadData(id);

                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "DSDondathangFina view", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        private void LoadData(string id)
        {
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "EXEC SP_FINA_SELECT_DATA @LOAI=3, @MAPHIEU=" + cls_Main.SQLString(id);
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            item.kho = dt.Rows[0]["TEN_KHO"].ToString();
            item.maphieu = dt.Rows[0]["MAPDNDC"].ToString();
            item.nhanvien = dt.Rows[0]["TENNHANVIEN"].ToString();
            item.ngaylap = dt.Rows[0]["NGAYLAPPHIEU"].ToString();
            item.khachhang = dt.Rows[0]["TENKHACHHANG"].ToString();
            item.dienthoai = dt.Rows[0]["DIENTHOAI"].ToString();
            item.diachi = dt.Rows[0]["DIACHI"].ToString();
            item.email = dt.Rows[0]["EMAIL"].ToString();
            item.masothue = dt.Rows[0]["MST"].ToString();
            item.cccd = dt.Rows[0]["CMND"].ToString();
            //
            foreach (DataRow dr in dt.Rows)
            {
                clsDathang item = new clsDathang();
                item.masanpham = dr["MA_VACH"].ToString();
                item.tensanpham = dr["TEN"].ToString();
                item.donvitinh = dr["DVT"].ToString();
                item.dongia = decimal.Parse(dr["GIANHAP"].ToString()).ToString("N0");
                item.soluong = decimal.Parse(dr["SL"].ToString()).ToString("N0");
                item.thanhtien = decimal.Parse(dr["THANHTIEN"].ToString()).ToString("N0");
                item.stt = (1 + dt.Rows.IndexOf(dr)).ToString();

                listitem.Add(item);

                tongsoluong = tongsoluong + int.Parse(dr["SL"].ToString());
                tongthanhtien = tongthanhtien + long.Parse(dr["THANHTIEN"].ToString());
            }
        }
    }
}
