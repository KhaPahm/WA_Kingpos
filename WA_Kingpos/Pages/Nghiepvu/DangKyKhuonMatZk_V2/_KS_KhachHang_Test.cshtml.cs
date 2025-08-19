using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.Nghiepvu.DangKyKhuonMatZk_V2
{
    public class _KS_KhachHang_TestModel : PageModel
    {
        [BindProperty] public cls_KS_KhachHang KS_KhachHang { get; set; } = new cls_KS_KhachHang();
        [BindProperty(SupportsGet = true)] public string Mode { get; set; } // view | edit | create
        [BindProperty(SupportsGet = true)] public int? Id { get; set; }

        public bool IsReadOnly => string.Equals(Mode, "view", StringComparison.OrdinalIgnoreCase);
        public bool IsCreate => string.Equals(Mode, "create", StringComparison.OrdinalIgnoreCase);
        public string Title => IsCreate ? "Thêm thông tin khách hàng" : IsReadOnly ? "Xem thông tin khách hàng" : "Sửa thông tin khách hàng";

        public IActionResult OnGet()
        {
            if (IsCreate)
            {
                KS_KhachHang = new();
                return Page();
            }

            if (Id == null)
            {
                return BadRequest();
            }

            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = $"EXEC SP_GET_DS_KS_KHACHHANG @MaKH = " + Id.ToString();
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            if (dt.Rows.Count > 0)
            {
                KS_KhachHang = dt.ToList<cls_KS_KhachHang>().FirstOrDefault();
            }
            return Page();
        }

        public IActionResult OnPostSave()
        {
            //if(!ModelState.IsValid)
            //{
            //    return Page();
            //}

            string sSql = "";
            if (string.Equals(Mode, "create", StringComparison.OrdinalIgnoreCase))
            {
                string manhanvien = User.FindFirst("UserId")?.Value;
                sSql = "INSERT INTO KS_KHACHHANG (TEN, NGAYSINH, GIOITINH, DIACHI, DIENTHOAI, EMAIL, CMND, QUOCTICH, NGAYTAO, NGUOITAO, FACE_PHOTO) " +
                    $"VALUES (" +
                    $"{cls_Main.SQLStringUnicode(KS_KhachHang.TEN)}, " +
                    $"{cls_Main.SQLString(KS_KhachHang.NGAYSINH)}, " +
                    $"{(KS_KhachHang.GIOITINH.ToUpper() == "NAM" ? "1" : "0")}, " +
                    $"{cls_Main.SQLStringUnicode(KS_KhachHang.DIACHI)}, " +
                    $"{cls_Main.SQLString(KS_KhachHang.DIENTHOAI)}, " +
                    $"{cls_Main.SQLString(KS_KhachHang.EMAIL)}, " +
                    $"{cls_Main.SQLString(KS_KhachHang.CCCD)}," +
                    $"{cls_Main.SQLString(KS_KhachHang.QUOCTICH)}, " +
                    $"GETDATE(), " +
                    $"{(string.IsNullOrEmpty(manhanvien) ? "116" : manhanvien)}, " +
                    $"{cls_Main.SQLString(KS_KhachHang.FACE_PHOTO)})";

            }
            else if (string.Equals(Mode, "edit", StringComparison.OrdinalIgnoreCase))
            {
                sSql = $"UPDATE KS_KHACHHANG SET " +
                    $"TEN = {cls_Main.SQLStringUnicode(KS_KhachHang.TEN)}, " +
                    $"NGAYSINH = {cls_Main.SQLString(KS_KhachHang.NGAYSINH)}, " +
                    $"GIOITINH = {(KS_KhachHang.GIOITINH.ToUpper() == "NAM" ? "1" : "0")}, " +
                    $"DIACHI = {cls_Main.SQLStringUnicode(KS_KhachHang.DIACHI)}, " +
                    $"DIENTHOAI = {cls_Main.SQLString(KS_KhachHang.DIENTHOAI)}, " +
                    $"EMAIL = {cls_Main.SQLString(KS_KhachHang.EMAIL)}, " +
                    $"CMND = {cls_Main.SQLString(KS_KhachHang.CCCD)}," +
                    $"QUOCTICH = {cls_Main.SQLString(KS_KhachHang.QUOCTICH)}, " +
                    $"FACE_PHOTO = {cls_Main.SQLString(KS_KhachHang.FACE_PHOTO)} " +
                $"WHERE MA_KH = {Id}";
            }

            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            cls_Main.ExecuteSQL(sSql, sConnectionString_live);

            return new JsonResult(new { ok = true });
        }
    }
}
