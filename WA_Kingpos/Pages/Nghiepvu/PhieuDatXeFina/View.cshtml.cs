using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.Nghiepvu.PhieuDatXeFina
{
    [Authorize]
    public class ViewModel : PageModel
    {
        public clsDathang item = new clsDathang();

        public List<clsDathang> listitem = new List<clsDathang>();
        public long? tongsoluong { get; set; } = 0;
        public long? tongthanhtien { get; set; } = 0;
        public IActionResult OnGet(string id)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowView("14102310", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                LoadData(id);

                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "PhieuDatXeFina view", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        private void LoadData(string id)
        {
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "EXEC SP_FINA_SELECT_DATA @LOAI=50, @MAPHIEU=" + cls_Main.SQLString(id);
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            item.trangthai = dt.Rows[0]["TRANGTHAI"].ToString();
            item.kho = dt.Rows[0]["TEN_KHO"].ToString();
            item.maphieu = dt.Rows[0]["MAPDNDC"].ToString();
            item.nhanvien = dt.Rows[0]["TENNHANVIEN"].ToString();
            item.ngaylap = dt.Rows[0]["NGAYLAPPHIEU"].ToString();
            item.ngaygiaohang= dt.Rows[0]["NGAYTHANHTOAN"].ToString();
            item.khachhang = dt.Rows[0]["TENKHACHHANG"].ToString();
            item.dienthoai = dt.Rows[0]["DIENTHOAI"].ToString();
            item.diachi = dt.Rows[0]["BIENSOXE"].ToString();
            item.cccd = dt.Rows[0]["CMND"].ToString();
            //
            foreach (DataRow dr in dt.Rows)
            {
                clsDathang item = new clsDathang();
                item.masanpham = dr["MA_VACH"].ToString();
                item.tensanpham = dr["TEN"].ToString();
                item.donvitinh = dr["NGAYLAPPHIEU1"].ToString();
                item.dongia = dr["TEN1"].ToString();
                item.thanhtien = decimal.Parse(dr["TONGTIEN"].ToString()).ToString("N0");
                item.stt = (1 + dt.Rows.IndexOf(dr)).ToString();
                item.soluong = decimal.Parse(dr["SOLUONG"].ToString()).ToString("N0");
                listitem.Add(item);
                tongthanhtien = tongthanhtien + long.Parse(dr["TONGTIEN"].ToString());
                tongsoluong = tongsoluong + long.Parse(dr["SOLUONG"].ToString());
            }
        }
    }
}
