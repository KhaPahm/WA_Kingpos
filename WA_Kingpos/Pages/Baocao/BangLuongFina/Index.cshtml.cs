using FastReport.Export.PdfSimple;
using FastReport.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeOpenXml;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.Baocao.BangLuongFina
{
    public class IndexModel : PageModel
    {
        public List<ThongTinBangLuong> listitem { set; get; } = new();
        DataTable TableMain = new();

        [BindProperty]
        public string MaNhanVien { get; set; } = "";

        public List<clsNhanvien> listNhanVien { get; set; } = new();

        public string strNhomNhanVien { get; set; } = "";

        public List<clsNhomNhanVien> listNhomNhanVien = new();


        [BindProperty]
        public string sNgay { get; set; } = "";

        public IndexModel()
        {
            //var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            //var GroupID = User.Claims.FirstOrDefault(c => c.Type == "GroupID")?.Value;
        }


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
                LoadData(id1 ?? MaNhanVien, id2 ?? sNgay, id4 ?? strNhomNhanVien);
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "BangLuongFina Get", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }

        public IActionResult OnPost(string submitButton, [FromForm(Name = "MaNhanVien")] List<string> id1, [FromForm(Name = "sNgay")] string id2, [FromForm(Name = "strNhomNhanVien")] string id4)
        {
            if (submitButton == "view")
            {
                return RedirectToPage("Index", new { id1 = string.Join(",", id1), id2 = id2 });
            }
            else if (submitButton == "print")
            {
                // Khởi tạo báo cáo
                WebReport webReport = new WebReport();

                // Load mẫu báo cáo từ file .frx
                string reportPath = Path.Combine(cls_Main.sFilePathReport, "rptBangLuong.frx");
                webReport.Report.Load(reportPath);
                LoadData(string.Join(",", id1), id2, id4);

                // Gán Data Source vào báo cáo
                webReport.Report.RegisterData(TableMain, "TableMain1");

                // Gán Parameter vào báo cáo
                webReport.Report.SetParameterValue("tencongty", cls_ConfigCashier.sTENCONGTY);
                webReport.Report.SetParameterValue("diachi", cls_ConfigCashier.sDIACHICONGTY);
                webReport.Report.SetParameterValue("dienthoai", cls_ConfigCashier.sSODIENTHOAICONGTY);
                webReport.Report.SetParameterValue("sNgay", sNgay);


                //Render report
                webReport.Report.Prepare();

                // Export báo cáo sang PDF và trả về kết quả
                MemoryStream stream = new MemoryStream();
                webReport.Report.Export(new PDFSimpleExport(), stream);
                stream.Position = 0;
                return File(stream, "application/pdf", "rptBangLuong.pdf");
            }
            else if (submitButton == "excel")
            {
                // Tạo DataTable với dữ liệu của bảng bạn muốn xuất ra Excel
                LoadData(string.Join(",", id1), id2, id4);

                //Thêm Parameter vào excel
                string tencongty = cls_ConfigCashier.sTENCONGTY;
                string diachi = "Địa chỉ : " + cls_ConfigCashier.sDIACHICONGTY;
                string dienthoai = "Điện thoại : " + cls_ConfigCashier.sSODIENTHOAICONGTY;

                string tieude = "BẢNG LƯƠNG NHÂN VIÊN";
                // Tạo một package ExcelPackage từ EPPlus
                using (ExcelPackage package = new ExcelPackage())
                {
                    // Tạo một worksheet trong package
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("rptBangLuong");
                    worksheet.Cells["A1:I1"].Merge = true;
                    worksheet.Cells["A1"].Style.Font.Bold = true;
                    worksheet.Cells["A1"].Value = tencongty;

                    worksheet.Cells["A2:I2"].Merge = true;
                    worksheet.Cells["A2"].Style.Font.Bold = true;
                    worksheet.Cells["A2"].Value = diachi;

                    worksheet.Cells["A3:I3"].Merge = true;
                    worksheet.Cells["A3"].Style.Font.Bold = true;
                    worksheet.Cells["A3"].Value = dienthoai;

                    worksheet.Cells["A4:L4"].Merge = true;
                    worksheet.Cells["A4"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells["A4"].Style.Font.Bold = true;
                    worksheet.Cells["A4"].Value = tieude;
                    //
                    worksheet.Cells["A5"].Style.Font.Bold = true;
                    worksheet.Cells["A5"].Value = "Thời gian:";
                    worksheet.Cells["B5"].Style.Font.Bold = true;
                    worksheet.Cells["B5"].Value = sNgay;

                    worksheet.Row(7).Style.Font.Bold = true;
                    // header table
                    worksheet.Cells["A7"].LoadFromArrays(new List<string[]>{
                        new[] { "Tên nhân viên", "Tổng Lương\r\n[1]+[4]+[5]+[6]", "Lương căn bản\r\n[1]","SL tiêu chuẩn (Kg)", "SL quy đổi (Kg)\r\n[2]", "Tỷ lệ %",
                    "Đơn vị HH (Đ/Kg)\r\n[3]", "Hoa hồng\r\n[4]=[2]*[3]", "Chênh lệch tiền hàng\r\n[5]", "HH quản lý\r\n[6]", "Hình thức", "Khu vực"} });

                    worksheet.Row(7).Style.WrapText = true;


                    // Đưa dữ liệu từ DataTable vào worksheet
                    worksheet.Cells["A8"].LoadFromArrays(listitem.Select(r => new object?[] { r.TENNHANVIEN, r.TONGLUONG, (r.LUONG_CANBAN ?? 0) + (r.LUONG_QUANLY ?? 0),
                        r.DOANHSO_TIEUCHUAN,r.DOANHSO_KPI,r.TYLE_HOANTHANH, r.DONVI_HH,r.HH_DOANHSO, r.CHENHLECH_BANHANG,r.HH_QUANLY,r.TRANGTHAI_NHANVIEN,r.KHUVUC_KPI }).ToList());

                    // Tự động điều chỉnh kích thước của tất cả cột dựa trên nội dung
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                    // Lưu tệp Excel và trả về kết quả
                    MemoryStream stream = new MemoryStream();
                    package.SaveAs(stream);
                    stream.Position = 0;
                    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "rptBangLuong.xlsx");
                }
            }
            return Page();
        }

        private void LoadData(string id1, string id2, string id4)
        {
            if (id1?.StartsWith("0,") == true)
            {
                id1 = "0";
            }
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

            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var sSQL = $"Select * From dbo.fn_View_NhomNhanVien() WHERE MA_NGUOIQUANLY = {userId}";
            listNhomNhanVien = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live).ToList<clsNhomNhanVien>();
            if (string.IsNullOrEmpty(strNhomNhanVien) && listNhomNhanVien.Count > 0)
            {
                // Chọn ngay nhóm đầu tiên để xem dữ liệu ngay khi load trang
                strNhomNhanVien = listNhomNhanVien[0].MANHOM.ToString();
            }

            listNhanVien.Clear();
            if (listNhomNhanVien.Count > 0)
            {
                // Có quản lý, load danh sách nhân viên cấp dưới
                sSQL = $"SELECT * FROM dbo.fn_DanhSachCapDuoi({userId}) ORDER BY TENNHANVIEN";
                var dtCapDuoi = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                listNhanVien.AddRange(dtCapDuoi.ToList<clsNhanvien>());
            }
            // chèn thêm 1 dòng là chính bản thân
            sSQL = $"SELECT * FROM dbo.DM_NHANVIEN WHERE MANHANVIEN = {ExtensionObject.toSqlPar(userId)}";
            var dtNhanVien = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            listNhanVien.Insert(0, dtNhanVien.ToList<clsNhanvien>().First());
            // filter lại danh sách nhân viên phải thuộc nhóm đang chọn
            if (!string.IsNullOrEmpty(strNhomNhanVien))
            {
                // nhóm đang chọn
                var nhom = listNhomNhanVien.FirstOrDefault(r => r.MANHOM.ToString() == strNhomNhanVien);
                listNhanVien.RemoveAll(r => nhom?.isThanhVien(r.manhanvien) != true);
            }
            if (string.IsNullOrEmpty(MaNhanVien))
            {
                // Chọn tất cả để xem dữ liệu ngay khi load trang
                MaNhanVien = string.Join(",", listNhanVien.Select(r => r.manhanvien));
            }


            if (string.IsNullOrEmpty(id2))
            {
                return;
            }
            var ngay_bd = id2.Split('-')[0].Trim();
            var ngay_kt = id2.Split('-')[1].Trim();
            sSQL = $"SELECT * FROM dbo.fn_BangLuongNhanVien({cls_Main.SQLString(MaNhanVien)} , 0, {cls_Main.SQLString(ngay_bd)}, {cls_Main.SQLString(ngay_kt)}) ORDER BY TENNHANVIEN";
            TableMain = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            listitem = TableMain.ToList<ThongTinBangLuong>();


        }
    }
}
