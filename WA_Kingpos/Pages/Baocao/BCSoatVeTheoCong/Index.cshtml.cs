using FastReport.Export.PdfSimple;
using FastReport.Web;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeOpenXml;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.Baocao.BCSoatVeTheoCong
{
    [Authorize]
    public class IndexModel : PageModel
    {
        [BindProperty]
        public List<NHANVIEN_CBO> ListNhanvien { set; get; } = new List<NHANVIEN_CBO>();
        public List<cls_SoatVeTheoCong> listHD { set; get; } = new List<cls_SoatVeTheoCong>();

        [BindProperty]
        public string selectedValue { get; set; } = "";
        [BindProperty]
        public string sTungay { get; set; } = "";
        [BindProperty]
        public string sNhomHang { get; set; } = "";
        [BindProperty]
        public string sCombo { get; set; } = "";

        string userName = string.Empty;
        string userID = string.Empty;
        string fullName = string.Empty;

        public decimal thanhTien = 0;
        public decimal tienVAT = 0;
        public decimal tienPhuThu = 0;
        public decimal tienChietKhau = 0;
        public decimal tongCong = 0;

        public IActionResult OnGet(string id, string id1, string id3)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowView("25081302", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                LoadData(id, id1, id3);
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "DSHangHoa Get", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        public IActionResult OnPost(string submitButton, string id, string id1, string id3)
        {
            try
            {
                if (submitButton == "view")
                {
                    string[] dateParts = sTungay.Split(new string[] { " - " }, StringSplitOptions.None);
                    return RedirectToPage("Index", new { id = dateParts[0], id1 = dateParts[1], id3 = sCombo });
                }
                else if (submitButton == "print")
                {
                    string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                    string sSQL = "";

                    if (User.Identity.IsAuthenticated)
                    {
                        userName = User.Identity.Name; // Get the current user's name
                    }

                    sSQL += "SELECT TOP 1 U.MaNV, NV.TENNHANVIEN FROM SYS_USER U JOIN DM_NHANVIEN NV ON U.MaNV = NV.MANHANVIEN WHERE UserID = " + cls_Main.SQLString(userName);
                    DataTable dtMaKH = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);

                    if (dtMaKH != null)
                    {
                        userID = dtMaKH.Rows[0][0].ToString();
                        fullName = dtMaKH.Rows[0]["TENNHANVIEN"].ToString();
                    }

                    // Khởi tạo báo cáo
                    WebReport webReport = new WebReport();

                    // Load mẫu báo cáo từ file .frx
                    string reportPath = Path.Combine(cls_Main.sFilePathReport, "rptBCBH_Daily.frx");
                    webReport.Report.Load(reportPath);

                    // Gán Data Source vào báo cáo
                    string selectedValue = "";
                    string scomboBox = "";
                    sSQL = string.Empty;

                    DateTime fromDate;
                    DateTime toDate;
                    var resultParseFromDate = DateTime.TryParseExact(id, "dd/MM/yyyy hh:mm tt",
                                    CultureInfo.InvariantCulture,
                                    DateTimeStyles.None,
                                    out fromDate);

                    var resultParseToDate = DateTime.TryParseExact(id1, "dd/MM/yyyy hh:mm tt",
                                                        CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                                                        out toDate);
                    if (resultParseToDate && resultParseFromDate)
                    {
                        sSQL += "EXEC SP_BAOCAOBANHANG @TUNGAY=" + cls_Main.SQLString(string.Format("{0:yyyyMMdd HH:mm}", fromDate))
                        + ", @DENNGAY=" + cls_Main.SQLString(string.Format("{0:yyyyMMdd HH:mm}", toDate))
                        + ",@DIEUKIEN=" + cls_Main.SQLString(id3) + ",@TYPE='NV'" + ",@USERLOGIN =" + cls_Main.SQLString(userID);
                    }
                    selectedValue = "ĐẠI LÝ: ";
                    scomboBox = fullName;

                    DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                    if (dt.Rows.Count == 0)
                    {
                        return BadRequest();
                    }
                    if (dt.Columns.Contains("TEN_CUAHANG") == false)
                        dt.Columns.Add("TEN_CUAHANG", typeof(string));
                    webReport.Report.RegisterData(dt, "TableMain");

                    // Gán Parameter vào báo cáo
                    webReport.Report.SetParameterValue("tencongty", cls_ConfigCashier.sTENCONGTY);
                    webReport.Report.SetParameterValue("diachi", cls_ConfigCashier.sDIACHICONGTY);
                    webReport.Report.SetParameterValue("dienthoai", cls_ConfigCashier.sSODIENTHOAICONGTY);
                    webReport.Report.SetParameterValue("tieude", "BÁO CÁO BÁN HÀNG ĐẠI LÝ");

                    webReport.Report.SetParameterValue("selectedValue", selectedValue);
                    webReport.Report.SetParameterValue("scomboBox", scomboBox);
                    webReport.Report.SetParameterValue("sTungay", id + " - " + id1);

                    //Render report
                    webReport.Report.Prepare();

                    // Export báo cáo sang PDF và trả về kết quả
                    MemoryStream stream = new MemoryStream();
                    webReport.Report.Export(new PDFSimpleExport(), stream);
                    stream.Position = 0;
                    return File(stream, "application/pdf", "rptBCBanHang.pdf");
                }
                else if (submitButton == "excel")
                {
                    // Tạo DataTable với dữ liệu của bảng bạn muốn xuất ra Excel
                    string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                    string sSQL = "";

                    if (User.Identity.IsAuthenticated)
                    {
                        userName = User.Identity.Name; // Get the current user's name
                    }

                    sSQL += "SELECT TOP 1 U.MaNV, NV.TENNHANVIEN FROM SYS_USER U JOIN DM_NHANVIEN NV ON U.MaNV = NV.MANHANVIEN WHERE UserID = " + cls_Main.SQLString(userName);
                    DataTable dtMaKH = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);

                    if (dtMaKH != null)
                    {
                        userID = dtMaKH.Rows[0][0].ToString();
                        fullName = dtMaKH.Rows[0]["TENNHANVIEN"].ToString();
                    }

                    string selectedValue = "";
                    string scomboBox = "";
                    sSQL = string.Empty;

                    DateTime fromDate;
                    DateTime toDate;
                    var resultParseFromDate = DateTime.TryParseExact(id, "dd/MM/yyyy hh:mm tt",
                                    CultureInfo.InvariantCulture,
                                    DateTimeStyles.None,
                                    out fromDate);

                    var resultParseToDate = DateTime.TryParseExact(id1, "dd/MM/yyyy hh:mm tt",
                                                        CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                                                        out toDate);

                    if (resultParseToDate && resultParseFromDate)
                    {
                        sSQL += "EXEC SP_BAOCAOBANHANG @TUNGAY=" + cls_Main.SQLString(string.Format("{0:yyyyMMdd HH:mm}", fromDate))
                        + ", @DENNGAY=" + cls_Main.SQLString(string.Format("{0:yyyyMMdd HH:mm}", toDate))
                        + ",@DIEUKIEN=" + cls_Main.SQLString(id3) + ",@TYPE='NV'" + ",@USERLOGIN =" + cls_Main.SQLString(userID);
                    }
                    selectedValue = "ĐẠI LÝ: ";
                    scomboBox = fullName;

                    DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                    //if (dt.Columns.Contains("TEN_CUAHANG") == false)
                    //    dt.Columns.Add("TEN_CUAHANG", typeof(string));
                    DataView view = new System.Data.DataView(dt);
                    DataTable dataTable = view.ToTable("MyDataTable", false, "MAVACH", "TEN_HANGHOA", "TEN_DONVITINH", "SOLUONG", "DONGIA", "THANHTIEN", "THUE", "TIENTHUE", "TIENPHUTHU", "TIENCHIETKHAU", "TONGCONG");
                    dataTable.Columns["MAVACH"].ColumnName = "Mã hàng hóa";
                    dataTable.Columns["TEN_HANGHOA"].ColumnName = "Tên hàng hóa";
                    dataTable.Columns["TEN_DONVITINH"].ColumnName = "Tên đơn vị tính";
                    dataTable.Columns["SOLUONG"].ColumnName = "Số lượng";
                    dataTable.Columns["DONGIA"].ColumnName = "Đơn giá";
                    dataTable.Columns["THANHTIEN"].ColumnName = "Thành tiền";
                    dataTable.Columns["THUE"].ColumnName = "Thuế";
                    dataTable.Columns["TIENTHUE"].ColumnName = "Tiền thuế";
                    dataTable.Columns["TIENPHUTHU"].ColumnName = "Tiền phụ thu";
                    dataTable.Columns["TIENCHIETKHAU"].ColumnName = "Tiền chiết khấu";
                    dataTable.Columns["TONGCONG"].ColumnName = "Tổng cộng";
                    //dataTable.Columns["TEN_CUAHANG"].ColumnName = "Tên cửa hàng";

                    //Thêm Parameter vào excel
                    string tencongty = cls_ConfigCashier.sTENCONGTY;
                    string diachi = "Địa chỉ : " + cls_ConfigCashier.sDIACHICONGTY;
                    string dienthoai = "Điện thoại : " + cls_ConfigCashier.sSODIENTHOAICONGTY;

                    string tieude = "BÁO CÁO BÁN HÀNG ĐẠI LÝ";

                    // Tạo một package ExcelPackage từ EPPlus
                    using (ExcelPackage package = new ExcelPackage())
                    {
                        // Tạo một worksheet trong package
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("rptDSHoaDon");
                        worksheet.Cells["A1:I1"].Merge = true;
                        worksheet.Cells["A1"].Style.Font.Bold = true;
                        worksheet.Cells["A1"].Value = tencongty;

                        worksheet.Cells["A2:I2"].Merge = true;
                        worksheet.Cells["A2"].Style.Font.Bold = true;
                        worksheet.Cells["A2"].Value = diachi;

                        worksheet.Cells["A3:I3"].Merge = true;
                        worksheet.Cells["A3"].Style.Font.Bold = true;
                        worksheet.Cells["A3"].Value = dienthoai;

                        worksheet.Cells["A4:K4"].Merge = true;
                        worksheet.Cells["A4"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells["A4"].Style.Font.Bold = true;
                        worksheet.Cells["A4"].Value = tieude;
                        //
                        worksheet.Cells["A5:K5"].Merge = true;
                        worksheet.Cells["A5"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells["A5"].Style.Font.Bold = false;
                        worksheet.Cells["A5"].Value = "Thời gian: " + id + " - " + id1;

                        worksheet.Cells["A6:K6"].Merge = true;
                        worksheet.Cells["A6"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells["A6"].Style.Font.Bold = false;
                        worksheet.Cells["A6"].Value = selectedValue + scomboBox;

                        //worksheet.Cells["A5"].Value = selectedValue;
                        //worksheet.Cells["B5"].Style.Font.Bold = true;
                        //worksheet.Cells["B5"].Value = scomboBox;
                        //
                        //worksheet.Cells["A6"].Style.Font.Bold = true;
                        //worksheet.Cells["A6"].Value = "Chi tiết:";
                        //worksheet.Cells["B6"].Style.Font.Bold = true;
                        //worksheet.Cells["B6"].Value = scomboBox;

                        worksheet.Row(8).Style.Font.Bold = true;
                        // Đưa dữ liệu từ DataTable vào worksheet
                        worksheet.Cells["A8"].LoadFromDataTable(dataTable, true);

                        // Tự động điều chỉnh kích thước của tất cả cột dựa trên nội dung
                        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                        // Lưu tệp Excel và trả về kết quả
                        MemoryStream stream = new MemoryStream();
                        package.SaveAs(stream);
                        stream.Position = 0;
                        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "rptDSHoaDon.xlsx");
                    }
                }
                LoadData(string.Empty, string.Empty, string.Empty);
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "DSHangHoa Post : ", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        private void LoadData(string id, string id1, string id3)
        {
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "";

            if (User.Identity.IsAuthenticated)
            {
                userName = User.Identity.Name; // Get the current user's name
            }

            sSQL += "SELECT TOP 1 U.MaNV, NV.TENNHANVIEN FROM SYS_USER U JOIN DM_NHANVIEN NV ON U.MaNV = NV.MANHANVIEN WHERE UserID = " + cls_Main.SQLString(userName);
            DataTable dtMaKH = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);

            if (dtMaKH != null)
            {
                userID = dtMaKH.Rows[0][0].ToString();
            }

            NHANVIEN_CBO nv = new NHANVIEN_CBO();
            nv.MA = dtMaKH.Rows[0]["MaNV"].ToString();
            nv.TEN = dtMaKH.Rows[0]["TENNHANVIEN"].ToString();
            ListNhanvien.Add(nv);

            //nhan vien
            //sSQL = "EXEC SP_GET_CUAHANG_KHO_QUAY_THEO_NHANVIEN " + cls_Main.SQLString(userID) + ", " + cls_Main.SQLString("5");
            //DataTable dtNhanvien = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);

            //foreach (DataRow dr in dtNhanvien.Rows)
            //{
            //    NHANVIEN_CBO nv = new NHANVIEN_CBO();
            //    nv.MA = dr["MA"].ToString();
            //    nv.TEN = dr["TEN"].ToString();
            //    ListNhanvien.Add(nv);
            //}

            sSQL = string.Empty;

            DateTime fromDate;
            DateTime toDate;
            var resultParseFromDate = DateTime.TryParseExact(id, "dd/MM/yyyy hh:mm tt",
                                    CultureInfo.InvariantCulture,
                                    DateTimeStyles.None,
                                    out fromDate);
            var resultParseToDate = DateTime.TryParseExact(id1, "dd/MM/yyyy hh:mm tt",
                                                CultureInfo.InvariantCulture,
                                                DateTimeStyles.None,
                                                out toDate);


            if (resultParseFromDate && resultParseToDate)
            {
                sSQL += "EXEC SP_BAOCAOBANHANG_DAILY @TUNGAY=" + cls_Main.SQLString(string.Format("{0:yyyyMMdd HH:mm}", fromDate))
                    + ", @DENNGAY=" + cls_Main.SQLString(string.Format("{0:yyyyMMdd HH:mm}", toDate))
                    + ",@DIEUKIEN=" + cls_Main.SQLString(id3) + ",@TYPE='NV'" + ",@USERLOGIN =" + cls_Main.SQLString(userID);
            }

            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            int i = 0;

            foreach (DataRow dr in dt.Rows)
            {
                BaoCaoBanHang item = new BaoCaoBanHang();
                item.STT = ++i;
                item.MA_HOADON = dr["MA_HOADON"].ToString();
                item.NGAYTAO = ((DateTime)dr["NGAYTAO"]).ToString("dd/MM/yyyy");
                item.MAHANGHOA = dr["MAVACH"].ToString();
                item.TENHANGHOA = dr["TEN_HANGHOA"].ToString();
                item.DVT = dr["TEN_DONVITINH"].ToString();
                item.SOLUONG = float.Parse(dr["SOLUONG"].ToString());
                item.DONGIA = decimal.Parse(dr["DONGIA"].ToString());
                item.THANHTIEN = decimal.Parse(dr["THANHTIEN"].ToString());
                item.VAT = decimal.Parse(dr["THUE"].ToString());
                item.TIENVAT = decimal.Parse(dr["TIENTHUE"].ToString());
                item.TIENPHUTHU = decimal.Parse(dr["TIENPHUTHU"].ToString());
                item.TIENCHIETKHAU = decimal.Parse(dr["TIENCHIETKHAU"].ToString());
                item.TONGCONG = decimal.Parse(dr["TONGCONG"].ToString());
                //item.NHOMHANG = dr["TEN_NHOMHANG"].ToString();
                //item.CUAHANG = dr["TEN_CUAHANG"].ToString();
                //item.GHICHUCHIETKHAU = dr["GHICHU_CHIETKHAU"].ToString();
                //item.QUAY = dr["TEN_QUAY"].ToString();;
                listHD.Add(item);

                thanhTien += item.THANHTIEN;
                tienVAT += item.TIENVAT;
                tienPhuThu += item.TIENPHUTHU;
                tienChietKhau += item.TIENCHIETKHAU;
                tongCong += item.TONGCONG;
            }

            if (!string.IsNullOrEmpty(id))
            {
                sTungay = id + " - " + id1;
            }
        }
    }
}
