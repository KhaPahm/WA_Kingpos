using System.Data;
using System.Globalization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using WA_Kingpos.Data;
using WA_Kingpos.Helper;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.Nghiepvu.DangKyKhuonMatZk_V2
{
    public class _KS_KhachHangModel : PageModel
    {
        [BindProperty]public List<int> SelectedCongIds { get; set; } = new List<int>();
        [BindProperty]public cls_KS_KhachHang KS_KhachHang { get; set; } = new cls_KS_KhachHang();
        [BindProperty(SupportsGet = true)] public string Mode { get; set; } // view | edit | create
        [BindProperty(SupportsGet = true)] public int? Id { get; set; }

        [BindProperty] public string sTuNgay { get; set; } = "";
        [BindProperty] public string sDenNgay { get; set; } = "";

        public List<cls_DISPLAY_ORDER_QRCODE_SN> lsCong { get; set; }
        public string SelectedCongIdsString { get; set; }
        public bool IsReadOnly => string.Equals(Mode, "view", StringComparison.OrdinalIgnoreCase);
        public bool IsCreate => string.Equals(Mode, "create", StringComparison.OrdinalIgnoreCase);
        public string Title   => IsCreate ? "Thêm thông tin khách hàng" : IsReadOnly ? "Xem thông tin khách hàng" : "Sửa thông tin khách hàng";

        private readonly IRazorViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        public _KS_KhachHangModel(IRazorViewEngine viewEngine, ITempDataProvider tempDataProvider)
        {
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
        }

        public IActionResult OnGet()
        {
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL1 = $"EXEC SP_GET_DISPLAY_ORDER_QRCODE_SN @Type = 'FACE', @Active = 1";
            DataTable dt1 = cls_Main.ReturnDataTable_NoLock(sSQL1, sConnectionString_live);
            if (dt1.Rows.Count > 0)
            {
                lsCong = dt1.ToList<cls_DISPLAY_ORDER_QRCODE_SN>();
            }

            if (IsCreate) {
                KS_KhachHang = new();
                sTuNgay = KS_KhachHang.TUNGAY.ToString("dd/MM/yyyy HH:mm:ss");
                sDenNgay = KS_KhachHang.DENNGAY.ToString("dd/MM/yyyy HH:mm:ss");
                return Page();
            }

            if(Id == null)
            {
                return BadRequest();
            }

            string sSQL = $"EXEC SP_GET_DS_KS_KHACHHANG @MaKH = " + Id.ToString();
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            if (dt.Rows.Count > 0)
            {
                KS_KhachHang = dt.ToList<cls_KS_KhachHang>().FirstOrDefault();
            }

            SelectedCongIds = KS_KhachHang.GetCongSelected();
            sTuNgay = KS_KhachHang.TUNGAY.ToString("dd/MM/yyyy HH:mm:ss");
            sDenNgay = KS_KhachHang.DENNGAY.ToString("dd/MM/yyyy HH:mm:ss");
            return Page();
        }

        public async Task<IActionResult> OnPostSave()
        {
            string sSql = "";

            SelectedCongIdsString = string.Join(",", SelectedCongIds);
            DateTime dTuNgay;
            DateTime dDenNgay;
            DateTime.TryParseExact(sTuNgay, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dTuNgay);
            DateTime.TryParseExact(sDenNgay, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dDenNgay);
            KS_KhachHang.TUNGAY = dTuNgay;
            KS_KhachHang.DENNGAY = dDenNgay;

            if (string.Equals(Mode, "create", StringComparison.OrdinalIgnoreCase))
            {
                string manhanvien = User.FindFirst("UserId")?.Value;
                sSql = "INSERT INTO KS_KHACHHANG (TEN, NGAYSINH, GIOITINH, DIACHI, DIENTHOAI, EMAIL, CMND, QUOCTICH, NGAYTAO, NGUOITAO, FACE_PHOTO, TUNGAY, DENNGAY, CONG) " +
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
                    $"{cls_Main.SQLString(KS_KhachHang.FACE_PHOTO)}, " +
                    $"{cls_Main.SQLString(KS_KhachHang.TUNGAY.ToString("dd/MM/yyyy HH:mm:ss"))}, " +
                    $"{cls_Main.SQLString(KS_KhachHang.DENNGAY.ToString("dd/MM/yyyy HH:mm:ss"))}, " +
                    $"{cls_Main.SQLString(SelectedCongIdsString)})";

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
                    $"FACE_PHOTO = {cls_Main.SQLString(KS_KhachHang.FACE_PHOTO)}, " +
                    $"TUNGAY = {cls_Main.SQLString(KS_KhachHang.TUNGAY.ToString("dd/MM/yyyy HH:mm:ss"))}, " +
                    $"DENNGAY = {cls_Main.SQLString(KS_KhachHang.DENNGAY.ToString("dd/MM/yyyy HH:mm:ss"))}, " +
                    $"CONG = {cls_Main.SQLString(SelectedCongIdsString)} " +
                $"WHERE MA_KH = {Id}"; 
            }

            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            bool excueResult = cls_Main.ExecuteSQL(sSql, sConnectionString_live);
            //bool excueResult = false;
            if (excueResult == false)
                return BadRequest("Lỗi khi lưu dữ liệu. Vui lòng thử lại sau.");
            else {

                string sSQL = $"EXEC SP_GET_DS_KS_KHACHHANG @MaKH = {KS_KhachHang.MA_KH.ToString()}, @Ten = {cls_Main.SQLStringUnicode(KS_KhachHang.TEN)}, @Sdt = {cls_Main.SQLString(KS_KhachHang.DIENTHOAI)}";
                DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                if (dt.Rows.Count > 0)
                {
                    cls_KS_KhachHang KH_result = dt.ToList<cls_KS_KhachHang>().FirstOrDefault();
                    KH_result.CanEdit = cls_UserManagement.AllowEdit("2025081601", HttpContext.Session.GetString("Permission"));
                    KH_result.CanDelete = cls_UserManagement.AllowDelete("2025081601", HttpContext.Session.GetString("Permission"));
                    string htmlRow = await ViewRenderHelper.RenderPartialAsync(PageContext, _tempDataProvider, _viewEngine,
                                                                 "/Pages/Nghiepvu/DangKyKhuonMatZk_V2/_KS_KhachHang_Row.cshtml", KH_result);
                    string htmlCard = await ViewRenderHelper.RenderPartialAsync(PageContext, _tempDataProvider, _viewEngine,
                                                                 "/Pages/Nghiepvu/DangKyKhuonMatZk_V2/_KS_KhachHang_Card.cshtml", KH_result);
                    return new JsonResult(
                        new { 
                            ok = true, 
                            views = new {
                                row = htmlRow, // Trả về PartialView với dữ liệu đã cập nhật
                                card = htmlCard
                            } 
                        }); 
                }
            }
            return new JsonResult(new { ok = true });
        }

        public IActionResult OnPostDel(int id)
        {
            // delete...
            string sSql = $"UPDATE KS_KHACHHANG SET TRANGTHAI = 0 WHERE MA_KH = {id}";
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            cls_Main.ExecuteSQL(sSql, sConnectionString_live);
            return new JsonResult(new { ok = true });
        }
    }
}
