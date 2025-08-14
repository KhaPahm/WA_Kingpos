using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.Baocao.BangLuongFina
{
    public class DetailModel : PageModel
    {
        public string MaNhanVien { get; set; } = "";

        public string sNgay { get; set; } = "";

        public List<ThongTinBangLuong> listitem { set; get; } = new();

        public List<ThongTinDoanhSo> listDoanhSo { set; get; } = new();

        /// <summary>
        /// bên page index, chỉ giữ tạm để truyền khi back lại index
        /// </summary>
        public string strNhomNhanVien { get; set; } = "";

        public IActionResult OnGet(string id1, string id2, string id4)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowView("23120906", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                LoadData(id1, id2, id4);
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "DoanhSoFina Get", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }

        private void LoadData(string id1, string id2, string id4)
        {
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            if (!string.IsNullOrEmpty(id1))
            {
                MaNhanVien = id1;
            }
            if (!string.IsNullOrEmpty(id2))
            {
                sNgay = id2;
            }
            if (!string.IsNullOrEmpty(id4))
            {
                strNhomNhanVien = id4;
            }
            if (string.IsNullOrEmpty(MaNhanVien) || string.IsNullOrEmpty(sNgay))
            {
                return;
            }
            var ngay_bd = sNgay.Split('-')[0].Trim();
            var ngay_kt = sNgay.Split('-')[1].Trim();

            var sSQL = $"SELECT * FROM dbo.fn_BangLuongNhanVien({cls_Main.SQLString(MaNhanVien)}, 0, {cls_Main.SQLString(ngay_bd)}, {cls_Main.SQLString(ngay_kt)})";
            var dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            listitem = dt.ToList<ThongTinBangLuong>();

            // xem danh sách nhân viên cấp dưới
            sSQL = $"SELECT * FROM dbo.fn_BangLuongNhanVien({cls_Main.SQLString(MaNhanVien)}, 1, {cls_Main.SQLString(ngay_bd)}, {cls_Main.SQLString(ngay_kt)})";
            var dt2 = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            listitem.AddRange(dt2.ToList<ThongTinBangLuong>());

            sSQL = $"SELECT * FROM dbo.fn_Ct_DoanhSoNhanVien({cls_Main.SQLString(MaNhanVien)} , 0, {cls_Main.SQLString(ngay_bd)}, {cls_Main.SQLString(ngay_kt)}, '0')";
            var dt3 = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            listDoanhSo = dt3.ToList<ThongTinDoanhSo>();

            sSQL = $"SELECT * FROM dbo.fn_Ct_DoanhSoNhanVien({cls_Main.SQLString(MaNhanVien)} , 1, {cls_Main.SQLString(ngay_bd)}, {cls_Main.SQLString(ngay_kt)}, '0') ORDER BY MANHANVIEN ";
            var dt4 = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            listDoanhSo.AddRange(dt4.ToList<ThongTinDoanhSo>());
        }


    }
}
