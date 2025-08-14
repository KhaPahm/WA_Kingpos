using FastReport.Export.PdfSimple;
using FastReport.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using WA_Kingpos.Data;
using WA_Kingpos.Models;
using static WA_Kingpos.Pages.Baocao.DSHanghoa.IndexModel;
using static WA_Kingpos.Pages.Nhacungcap.CreateModel;

namespace WA_Kingpos.Pages.Baocao.BCBanVeBanTungChuyen
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public ItemCheck itemCheck { get; set; }
        public List<Item> listitem_tuyen = new List<Item>();
        public List<Item> listitem = new List<Item>();
        public List<BAOCAO> listitemBC = new List<BAOCAO>();
        public List<ItemNV> listitem_NhanVien = new List<ItemNV>();
        public List<Item> listitem_DoiTuong = new List<Item>();
        public string THUCTHU = "0";
        public string PHAINOP = "0";
        public string TONG_CT_GIAMTRU = "0";
        public string TONG_CT_DATCOC = "0";
        public string TONG_CT_CUYENKHOAN = "0";
        public string TONG_CT_CONGNO = "0";
        public string TONG_CT_HOAHONG = "0";
        public string TONG_CT_DIEM = "0";
        public string TONG_CT_TIENGUI = "0";
        public string TONG_CT_DAILY = "0";
        public string TONG_CT_WEB = "0";
        public string TONG_PHUTHUHUYVE = "0";
        public string TONG_PHUTHUINLAI = "0";
        public bool CHECKNHOM { get; set; }
        public bool CHECKBSX { get; set; }
        [BindProperty]
        [Required(ErrorMessage = "Phải chọn tuyến")]
        public int MATUYEN { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Phải chọn chuyến tàu")]
        public int MALICHTRINH { get; set; }


        [BindProperty]
        [Required(ErrorMessage = "Phải chọn nhân viên")]
        public int MANHANVIEN { get; set; }

        [BindProperty]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public string sTungay { get; set; }
        string userName = string.Empty;
        string userID = string.Empty;
        public IActionResult OnGet(string id, string id1, string id2, string id3, bool CHECKNHOM, bool CHECKBSX)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowView("38", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                LoadData(id, id1, id2, id3, CHECKNHOM, CHECKBSX);
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "BCBanVeBanTungChuyen Get", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        public IActionResult OnPost(string submitButton, string id, string id1, string id2, string id3, bool CHECKNHOM, bool CHECKBSX)
        {
            try
            {
                if (string.IsNullOrEmpty(submitButton))
                {
                    submitButton = "btn1ReLoad";
                }
                if (submitButton == "btn1ReLoad" || submitButton == "view")
                {
                    return RedirectToPage("Index", new { id = MATUYEN, id1 = sTungay, id2 = MALICHTRINH, id3 = MANHANVIEN, CHECKNHOM , CHECKBSX });
                }
                else if (submitButton == "print")
                {
                    string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                    string sSQL = "";

                    if (User.Identity.IsAuthenticated)
                    {
                        userName = User.Identity.Name; // Get the current user's name
                    }

                    sSQL += "SELECT TOP 1 MaNV FROM SYS_USER WHERE UserID = " + cls_Main.SQLString(userName);
                    DataTable dtMaKH = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);

                    if (dtMaKH != null)
                    {
                        userID = dtMaKH.Rows[0][0].ToString();
                    }

                    // Khởi tạo báo cáo
                    WebReport webReport = new WebReport();

                    // Load mẫu báo cáo từ file .frx
                    string reportPath = Path.Combine(cls_Main.sFilePathReport, "rptBanVeMoiChuyen.frx");
                    webReport.Report.Load(reportPath);

                    // Gán Data Source vào báo cáo
                    string sTuyen = "";
                    string sTau = "";
                    string sNgay = "";
                    string sGio = "";
                    string sNhanVien = "";
                    sSQL = string.Empty;
                    if (CHECKBSX == false)
                        sSQL = "EXEC SP_W_BAOCAO_BANVE_BAN_TUNGCHUYEN_QL @MALICHTRINH=" + cls_Main.SQLString(MALICHTRINH.ToString()) + ",@TYPE=" + cls_Main.SQLBit(bool.Parse(CHECKNHOM.ToString())) + ",@USERNAME=" + cls_Main.SQLString(MANHANVIEN.ToString()) + "\n";
                    else
                        sSQL = "EXEC SP_W_BAOCAO_BANVE_BAN_TUNGCHUYEN_BS @MALICHTRINH=" + cls_Main.SQLString(MALICHTRINH.ToString()) + ",@TYPE=" + cls_Main.SQLBit(bool.Parse(CHECKNHOM.ToString())) + ",@USERNAME=" + cls_Main.SQLString(MANHANVIEN.ToString()) + "\n";
                    DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                    if (dt.Rows.Count > 0)
                    {
                        sTuyen = dt.Rows[0]["TENTUYEN"].ToString();
                        sTau = dt.Rows[0]["TENDOITUONG"].ToString();
                        sNgay = dt.Rows[0]["NGAYKHOIHANH"].ToString();
                        sGio = dt.Rows[0]["GIODI"].ToString();
                        sNhanVien = dt.Rows[0]["NHANVIENCOI"].ToString();
                        THUCTHU = decimal.Parse(dt.Rows[0]["THUCTHU"].ToString()).ToString("N0");
                        PHAINOP = decimal.Parse(dt.Rows[0]["PHAINOP"].ToString()).ToString("N0");
                        TONG_CT_GIAMTRU = decimal.Parse(dt.Rows[0]["TONG_CT_GIAMTRU"].ToString()).ToString("N0");
                        TONG_CT_DATCOC = decimal.Parse(dt.Rows[0]["TONG_CT_DATCOC"].ToString()).ToString("N0");
                        TONG_CT_CUYENKHOAN = decimal.Parse(dt.Rows[0]["TONG_CT_CUYENKHOAN"].ToString()).ToString("N0");
                        TONG_CT_CONGNO = decimal.Parse(dt.Rows[0]["TONG_CT_CONGNO"].ToString()).ToString("N0");
                        TONG_CT_HOAHONG = decimal.Parse(dt.Rows[0]["TONG_CT_HOAHONG"].ToString()).ToString("N0");
                        TONG_CT_DIEM = decimal.Parse(dt.Rows[0]["TONG_CT_DIEM"].ToString()).ToString("N0");
                        TONG_CT_TIENGUI = decimal.Parse(dt.Rows[0]["TONG_CT_TIENGUI"].ToString()).ToString("N0");
                        TONG_CT_DAILY = decimal.Parse(dt.Rows[0]["TONG_CT_DAILY"].ToString()).ToString("N0");
                        TONG_CT_WEB = decimal.Parse(dt.Rows[0]["TONG_CT_WEB"].ToString()).ToString("N0");
                        TONG_PHUTHUHUYVE = decimal.Parse(dt.Rows[0]["TONG_PHUTHUHUYVE"].ToString()).ToString("N0");
                        TONG_PHUTHUINLAI = decimal.Parse(dt.Rows[0]["TONG_PHUTHUINLAI"].ToString()).ToString("N0");
                        webReport.Report.RegisterData(dt, "TableMain");

                        // Gán Parameter vào báo cáo
                        webReport.Report.SetParameterValue("tencongty", cls_ConfigCashier.sTENCONGTY);
                        webReport.Report.SetParameterValue("diachi", cls_ConfigCashier.sDIACHICONGTY);
                        webReport.Report.SetParameterValue("dienthoai", cls_ConfigCashier.sSODIENTHOAICONGTY);

                        sSQL = "";
                        sSQL += "Select TENCONGTY,DIACHI,SODIENTHOAI,SOFAX,MAUSOPHIEUHD,KYHIEU,EMAIL,SOTAIKHOAN,MASOTHUE,HINHANH" + "\n";
                        sSQL += "From SYS_CONFIGREPORT" + "\n";
                        DataTable dtImage = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);

                        webReport.Report.SetParameterValue("LOGO", dtImage.Rows[0]["HINHANH"].ToString());
                        webReport.Report.SetParameterValue("TENTUYEN", sTuyen);
                        webReport.Report.SetParameterValue("TENDOITUONG", sTau);
                        webReport.Report.SetParameterValue("NGAYKHOIHANH", sNgay);
                        webReport.Report.SetParameterValue("GIODI", sGio);
                        webReport.Report.SetParameterValue("NHANVIEN", sNhanVien);
                        webReport.Report.SetParameterValue("THUCTHU", THUCTHU);
                        webReport.Report.SetParameterValue("PHAINOP", PHAINOP);
                        webReport.Report.SetParameterValue("TONG_CT_GIAMTRU", TONG_CT_GIAMTRU);
                        webReport.Report.SetParameterValue("TONG_CT_DATCOC", TONG_CT_DATCOC);
                        webReport.Report.SetParameterValue("TONG_CT_CUYENKHOAN", TONG_CT_CUYENKHOAN);
                        webReport.Report.SetParameterValue("TONG_CT_CONGNO", TONG_CT_CONGNO);
                        webReport.Report.SetParameterValue("TONG_CT_HOAHONG", TONG_CT_HOAHONG);
                        webReport.Report.SetParameterValue("TONG_CT_DIEM", TONG_CT_DIEM);
                        webReport.Report.SetParameterValue("TONG_CT_TIENGUI", TONG_CT_TIENGUI);
                        webReport.Report.SetParameterValue("TONG_CT_DAILY", TONG_CT_DAILY);
                        webReport.Report.SetParameterValue("TONG_CT_WEB", TONG_CT_WEB);
                        webReport.Report.SetParameterValue("TONG_PHUTHUHUYVE", TONG_PHUTHUHUYVE);
                        webReport.Report.SetParameterValue("TONG_PHUTHUINLAI", TONG_PHUTHUINLAI);
                        //Render report
                        webReport.Report.Prepare();

                        // Export báo cáo sang PDF và trả về kết quả
                        MemoryStream stream = new MemoryStream();
                        webReport.Report.Export(new PDFSimpleExport(), stream);
                        stream.Position = 0;
                        return File(stream, "application/pdf", "rptBanVeMoiChuyen.pdf");
                    }
                }
                else if (submitButton == "excel")
                {
                    // Tạo DataTable với dữ liệu của bảng bạn muốn xuất ra Excel
                    string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                    string sUsername = User.FindFirst("Username")?.Value;
                    string sSQL = "";
                    string sTuyen = "";
                    string sTau = "";
                    string sNgay = "";
                    string sGio = "";
                    string sNhanVien = "";
                    sSQL = string.Empty;
                    sSQL = "";
                    if (CHECKBSX == false)
                        sSQL = "EXEC SP_W_BAOCAO_BANVE_BAN_TUNGCHUYEN_QL @MALICHTRINH=" + cls_Main.SQLString(MALICHTRINH.ToString()) + ",@TYPE=" + cls_Main.SQLBit(bool.Parse(CHECKNHOM.ToString())) + ",@USERNAME=" + cls_Main.SQLString(MANHANVIEN.ToString()) + "\n";
                    else
                        sSQL = "EXEC SP_W_BAOCAO_BANVE_BAN_TUNGCHUYEN_BS @MALICHTRINH=" + cls_Main.SQLString(MALICHTRINH.ToString()) + ",@TYPE=" + cls_Main.SQLBit(bool.Parse(CHECKNHOM.ToString())) + ",@USERNAME=" + cls_Main.SQLString(MANHANVIEN.ToString()) + "\n";
                    DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                    if (dt.Rows.Count>0)
                    {
                        sTuyen = dt.Rows[0]["TENTUYEN"].ToString();
                        sTau = dt.Rows[0]["TENDOITUONG"].ToString();
                        sNgay = dt.Rows[0]["NGAYKHOIHANH"].ToString();
                        sGio = dt.Rows[0]["GIODI"].ToString();
                        sNhanVien = dt.Rows[0]["NHANVIENCOI"].ToString();
                        THUCTHU = decimal.Parse(dt.Rows[0]["THUCTHU"].ToString()).ToString("N0");
                        PHAINOP = decimal.Parse(dt.Rows[0]["PHAINOP"].ToString()).ToString("N0");
                        TONG_CT_GIAMTRU = decimal.Parse(dt.Rows[0]["TONG_CT_GIAMTRU"].ToString()).ToString("N0");
                        TONG_CT_DATCOC = decimal.Parse(dt.Rows[0]["TONG_CT_DATCOC"].ToString()).ToString("N0");
                        TONG_CT_CUYENKHOAN = decimal.Parse(dt.Rows[0]["TONG_CT_CUYENKHOAN"].ToString()).ToString("N0");
                        TONG_CT_CONGNO = decimal.Parse(dt.Rows[0]["TONG_CT_CONGNO"].ToString()).ToString("N0");
                        TONG_CT_HOAHONG = decimal.Parse(dt.Rows[0]["TONG_CT_HOAHONG"].ToString()).ToString("N0");
                        TONG_CT_DIEM = decimal.Parse(dt.Rows[0]["TONG_CT_DIEM"].ToString()).ToString("N0");
                        TONG_CT_TIENGUI = decimal.Parse(dt.Rows[0]["TONG_CT_TIENGUI"].ToString()).ToString("N0");
                        TONG_CT_DAILY = decimal.Parse(dt.Rows[0]["TONG_CT_DAILY"].ToString()).ToString("N0");
                        TONG_CT_WEB = decimal.Parse(dt.Rows[0]["TONG_CT_WEB"].ToString()).ToString("N0");
                        TONG_PHUTHUHUYVE = decimal.Parse(dt.Rows[0]["TONG_PHUTHUHUYVE"].ToString()).ToString("N0");
                        TONG_PHUTHUINLAI = decimal.Parse(dt.Rows[0]["TONG_PHUTHUINLAI"].ToString()).ToString("N0");
                        DataView view = new System.Data.DataView(dt);
                        DataTable dataTable = view.ToTable("MyDataTable", false,"STT", "LOAI", "TENKHUVUC", "SOLUONG", "DONGIA", "PHUTHU", "THANHTIEN", "PHUTHUHUYVE", "PHUTHUINLAI", "CT_GIAMTRU", "CT_HH", "CT_CONGNO", "CT_DATCOC", "CT_CUYENKHOAN", "CT_TIENGUI", "CT_DIEM", "CT_DAILY", "CT_WEB", "DIENGIAI");
                        dataTable.Columns["STT"].ColumnName = "STT";
                        dataTable.Columns["LOAI"].ColumnName = "Vé";
                        dataTable.Columns["TENKHUVUC"].ColumnName = "Loại";
                        dataTable.Columns["SOLUONG"].ColumnName = "Số lượng";
                        dataTable.Columns["DONGIA"].ColumnName = "Đơn giá";
                        dataTable.Columns["PHUTHU"].ColumnName = "Phụ thu";
                        dataTable.Columns["THANHTIEN"].ColumnName = "Thành tiền";
                        dataTable.Columns["PHUTHUHUYVE"].ColumnName = "Phụ thu trả vé";
                        dataTable.Columns["PHUTHUINLAI"].ColumnName = "Phụ thu in lại";
                        dataTable.Columns["CT_GIAMTRU"].ColumnName = "Giảm trừ";
                        dataTable.Columns["CT_HH"].ColumnName = "Hoa hồng";
                        dataTable.Columns["CT_CONGNO"].ColumnName = "Công nợ";
                        dataTable.Columns["CT_DATCOC"].ColumnName = "Đặt cọc";
                        dataTable.Columns["CT_CUYENKHOAN"].ColumnName = "Chuyển khoản";
                        dataTable.Columns["CT_TIENGUI"].ColumnName = "Tiền gửi";
                        dataTable.Columns["CT_DAILY"].ColumnName = "Điểm";
                        dataTable.Columns["CT_DIEM"].ColumnName = "Đại lý";
                        dataTable.Columns["CT_WEB"].ColumnName = "Online";
                        dataTable.Columns["DIENGIAI"].ColumnName = "Diễn giải";

                        //Thêm Parameter vào excel
                        string tencongty = cls_ConfigCashier.sTENCONGTY;
                        string diachi = "Địa chỉ : " + cls_ConfigCashier.sDIACHICONGTY;
                        string dienthoai = "Điện thoại : " + cls_ConfigCashier.sSODIENTHOAICONGTY;

                        string tieude = "BÁO CÁO CHI TIẾT BÁN VÉ MỖI CHUYẾN CỦA NHÂN VIÊN";

                        // Tạo một package ExcelPackage từ EPPlus
                        using (ExcelPackage package = new ExcelPackage())
                        {
                            // Tạo một worksheet trong package
                            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("rptBanVeMoiChuyen");
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
                            worksheet.Cells["A5"].Value = "Tuyến:";
                            worksheet.Cells["B5"].Style.Font.Bold = true;
                            worksheet.Cells["B5"].Value = sTuyen;
                            //
                            worksheet.Cells["A6"].Style.Font.Bold = true;
                            worksheet.Cells["A6"].Value = "Tàu:";
                            worksheet.Cells["B6"].Style.Font.Bold = true;
                            worksheet.Cells["B6"].Value = sTau;
                            //
                            worksheet.Cells["A7"].Style.Font.Bold = true;
                            worksheet.Cells["A7"].Value = "Ngày:";
                            worksheet.Cells["B7"].Style.Font.Bold = true;
                            worksheet.Cells["B7"].Value = sNgay;
                            //
                            worksheet.Cells["D7"].Style.Font.Bold = true;
                            worksheet.Cells["D7"].Value = "Giờ:";
                            worksheet.Cells["E7"].Style.Font.Bold = true;
                            worksheet.Cells["E7"].Value = sGio;
                            //
                            worksheet.Cells["A8"].Style.Font.Bold = true;
                            worksheet.Cells["A8"].Value = "Nhân viên:";
                            worksheet.Cells["B8"].Style.Font.Bold = true;
                            worksheet.Cells["B8"].Value = sNhanVien;
                            worksheet.Row(10).Style.Font.Bold = true;
                            //
                            worksheet.Cells["D8"].Style.Font.Bold = true;
                            worksheet.Cells["D8"].Value = "Hoa hồng:";
                            worksheet.Cells["E8"].Style.Font.Bold = true;
                            worksheet.Cells["E8"].Value = TONG_CT_HOAHONG;
                            worksheet.Row(10).Style.Font.Bold = true;
                            //
                            worksheet.Cells["G8"].Style.Font.Bold = true;
                            worksheet.Cells["G8"].Value = "Giảm giá:";
                            worksheet.Cells["H8"].Style.Font.Bold = true;
                            worksheet.Cells["H8"].Value = TONG_CT_GIAMTRU;
                            worksheet.Row(10).Style.Font.Bold = true;
                            //
                            worksheet.Cells["A9"].Style.Font.Bold = true;
                            worksheet.Cells["A9"].Value = "Tổng thành tiền:";
                            worksheet.Cells["B9"].Style.Font.Bold = true;
                            worksheet.Cells["B9"].Value = THUCTHU;
                            worksheet.Row(10).Style.Font.Bold = true;
                            //
                            worksheet.Cells["D9"].Style.Font.Bold = true;
                            worksheet.Cells["D9"].Value = "Đặc cọc:";
                            worksheet.Cells["E9"].Style.Font.Bold = true;
                            worksheet.Cells["E9"].Value = TONG_CT_DATCOC;
                            worksheet.Row(10).Style.Font.Bold = true;
                            //
                            worksheet.Cells["G9"].Style.Font.Bold = true;
                            worksheet.Cells["G9"].Value = "Công nợ:";
                            worksheet.Cells["H9"].Style.Font.Bold = true;
                            worksheet.Cells["H9"].Value = TONG_CT_CONGNO;
                            worksheet.Row(10).Style.Font.Bold = true;
                            //
                            worksheet.Cells["A10"].Style.Font.Bold = true;
                            worksheet.Cells["A10"].Value = "Phụ thu hủy vé:";
                            worksheet.Cells["B10"].Style.Font.Bold = true;
                            worksheet.Cells["B10"].Value = TONG_PHUTHUHUYVE;
                            worksheet.Row(10).Style.Font.Bold = true;
                            //
                            worksheet.Cells["D10"].Style.Font.Bold = true;
                            worksheet.Cells["D10"].Value = "Tền gửi:";
                            worksheet.Cells["E10"].Style.Font.Bold = true;
                            worksheet.Cells["E10"].Value = TONG_CT_TIENGUI;
                            worksheet.Row(10).Style.Font.Bold = true;
                            //
                            worksheet.Cells["G10"].Style.Font.Bold = true;
                            worksheet.Cells["G10"].Value = "Chuyển khoản:";
                            worksheet.Cells["H10"].Style.Font.Bold = true;
                            worksheet.Cells["H10"].Value = TONG_CT_CUYENKHOAN;
                            worksheet.Row(10).Style.Font.Bold = true;
                            //
                            worksheet.Cells["A11"].Style.Font.Bold = true;
                            worksheet.Cells["A11"].Value = "Phụ thu in lại";
                            worksheet.Cells["B11"].Style.Font.Bold = true;
                            worksheet.Cells["B11"].Value = TONG_PHUTHUINLAI;
                            worksheet.Row(10).Style.Font.Bold = true;
                            //
                            worksheet.Cells["D11"].Style.Font.Bold = true;
                            worksheet.Cells["D11"].Value = "Đại lý:";
                            worksheet.Cells["E11"].Style.Font.Bold = true;
                            worksheet.Cells["E11"].Value = TONG_CT_DAILY;
                            worksheet.Row(10).Style.Font.Bold = true;
                            //
                            worksheet.Cells["G11"].Style.Font.Bold = true;
                            worksheet.Cells["G11"].Value = "Điểm:";
                            worksheet.Cells["H11"].Style.Font.Bold = true;
                            worksheet.Cells["H11"].Value = TONG_CT_DIEM;
                            worksheet.Row(10).Style.Font.Bold = true;
                            //
                            worksheet.Cells["A12"].Style.Font.Bold = true;
                            worksheet.Cells["A12"].Value = "Thực thu:";
                            worksheet.Cells["B12"].Style.Font.Bold = true;
                            worksheet.Cells["B12"].Value = PHAINOP;
                            worksheet.Row(10).Style.Font.Bold = true;
                            //
                            worksheet.Cells["D12"].Style.Font.Bold = true;
                            worksheet.Cells["D12"].Value = "Online:";
                            worksheet.Cells["D12"].Style.Font.Bold = true;
                            worksheet.Cells["D12"].Value = TONG_CT_WEB;
                            worksheet.Row(10).Style.Font.Bold = true;
                            // Đưa dữ liệu từ DataTable vào worksheet
                            worksheet.Cells["A12"].LoadFromDataTable(dataTable, true);

                            // Tự động điều chỉnh kích thước của tất cả cột dựa trên nội dung
                            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                            // Lưu tệp Excel và trả về kết quả
                            MemoryStream stream = new MemoryStream();
                            package.SaveAs(stream);
                            stream.Position = 0;
                            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "rptBanVeMoiChuyen.xlsx");
                        }
                    } 
                        
                    
                }
                LoadData(id, id1, id2, id3, CHECKNHOM, CHECKBSX);
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "BCBanVeBanTungChuyen Post : ", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        private void LoadData(string id, string id1, string id2, string id3, bool id4, bool id5)
        {
            if (string.IsNullOrEmpty(id))
            {
                id = "0";
            }
            if (string.IsNullOrEmpty(id1))
            {
                id1 = DateTime.Now.ToString("dd/MM/yyyy");
            }
            if (string.IsNullOrEmpty(id2))
            {
                id2 = "0";
            }
            if (string.IsNullOrEmpty(id3))
            {
                id3 = "0";
            }

            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sUsername = User.FindFirst("Username")?.Value;
            string sUserID = User.FindFirst("UserID")?.Value;

            //ngay
            sTungay = id1;
            CHECKNHOM = id4;
            CHECKBSX = id5;
            //tuyen
            string sSQL = "EXEC SP_LOADLICHTRINH_THEOTUYEN  '', '', 1";
            DataTable dt_tuyen = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt_tuyen.Rows)
            {
                Item item = new Item();
                item.ID_Trip = dr["MATUYEN"].ToString();
                item.Name_Trip = dr["TENTUYEN"].ToString();
                listitem_tuyen.Add(item);
            }
            MATUYEN = int.Parse(id);
            //nhân viên
            sSQL = "EXEC SP_W_LOAD_COMBO_VETAU " + cls_Main.SQLString(sUserID) + " ,1";
            DataTable dt_NV = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt_NV.Rows)
            {
                ItemNV item = new ItemNV();
                item.UserID = dr["UserID"].ToString();
                item.MaNV = dr["MaNV"].ToString();
                item.MANHANVIEN = dr["MANHANVIEN"].ToString();
                item.TENNHANVIEN = dr["TENNHANVIEN"].ToString();
                listitem_NhanVien.Add(item);
            }
            if (dt_NV.Rows.Count >= 1 && id3 == "0")
            {
                id3 = dt_NV.Rows[0]["MANHANVIEN"].ToString();
            }
            MANHANVIEN = int.Parse(id3);
            //lich chay
            sSQL = "Exec SP_W_GetLichTauTheoNgay " + cls_Main.SQLString(id1) + "," + cls_Main.SQLString(id);
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                Item item = new Item();
                item.ID = dr["ID"].ToString();
                item.ID_Trip = dr["ID_Trip"].ToString();
                item.Name_Trip = dr["Name_Trip"].ToString();
                item.HourToGo = dr["HourToGo"].ToString();
                item.Vessel = dr["Vessel"].ToString();
                item.Note = dr["HourToGo"].ToString() + " - " + dr["Vessel"].ToString();
                if (item.ID_Trip == id)
                {
                    listitem.Add(item);
                }
            }
            MALICHTRINH = int.Parse(id2);
            //bao cao
            sSQL = "";
            if (CHECKBSX == false)
                sSQL = "EXEC SP_W_BAOCAO_BANVE_BAN_TUNGCHUYEN_QL @MALICHTRINH=" + cls_Main.SQLString(MALICHTRINH.ToString()) + ",@TYPE=" + cls_Main.SQLBit(bool.Parse(CHECKNHOM.ToString())) + ",@USERNAME=" + cls_Main.SQLString(MANHANVIEN.ToString()) + "\n";
            else
                sSQL = "EXEC SP_W_BAOCAO_BANVE_BAN_TUNGCHUYEN_BS @MALICHTRINH=" + cls_Main.SQLString(MALICHTRINH.ToString()) + ",@TYPE=" + cls_Main.SQLBit(bool.Parse(CHECKNHOM.ToString())) + ",@USERNAME=" + cls_Main.SQLString(MANHANVIEN.ToString()) + "\n";
            dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                BAOCAO item = new BAOCAO();
                item.STT = dr["STT2"].ToString();
                item.LOAI = dr["LOAI"].ToString();
                item.TENTUYEN = dr["TENTUYEN"].ToString();
                item.TENKHUVUC = dr["TENKHUVUC"].ToString();
                item.NGAYDI = String.Format("{0:dd/MM/yyyy}", dr["NGAYDI"]);
                item.NGAYKHOIHANH = String.Format("{0:dd/MM/yyyy}", dr["NGAYKHOIHANH"]);
                item.GIODI = dr["GIODI"].ToString();
                item.SOLUONG = decimal.Parse(dr["SOLUONG"].ToString());
                item.DONGIA = decimal.Parse(dr["DONGIA"].ToString());
                item.PHUTHU = decimal.Parse(dr["PHUTHU"].ToString());
                item.THANHTIEN = decimal.Parse(dr["THANHTIEN"].ToString());
                item.CT_GIAMTRU = decimal.Parse(dr["CT_GIAMTRU"].ToString());
                item.CT_DATCOC = decimal.Parse(dr["CT_DATCOC"].ToString());
                item.CT_CUYENKHOAN = decimal.Parse(dr["CT_CUYENKHOAN"].ToString());
                item.CT_TIENGUI = decimal.Parse(dr["CT_TIENGUI"].ToString());
                item.CT_CONGNO = decimal.Parse(dr["CT_CONGNO"].ToString());
                item.CT_DAILY = decimal.Parse(dr["CT_DAILY"].ToString());
                item.CT_WEB = decimal.Parse(dr["CT_WEB"].ToString());
                item.CT_HH = decimal.Parse(dr["CT_HH"].ToString());
                item.PHUTHUHUYVE = decimal.Parse(dr["PHUTHUHUYVE"].ToString());
                item.CT_DIEM = decimal.Parse(dr["CT_DIEM"].ToString());
                item.PHUTHUINLAI = decimal.Parse(dr["PHUTHUINLAI"].ToString());
                item.DIENGIAI = dr["DIENGIAI"].ToString();
                item.THUCTHU = decimal.Parse(dr["THUCTHU"].ToString());
                item.PHAINOP = decimal.Parse(dr["PHAINOP"].ToString());
                item.NHANVIEN = dr["Nhanvien1"].ToString();
                listitemBC.Add(item);
                THUCTHU = decimal.Parse(dt.Rows[0]["THUCTHU"].ToString()).ToString("N0");
                PHAINOP = decimal.Parse(dt.Rows[0]["PHAINOP"].ToString()).ToString("N0");
                TONG_CT_GIAMTRU = decimal.Parse(dt.Rows[0]["TONG_CT_GIAMTRU"].ToString()).ToString("N0");
                TONG_CT_DATCOC = decimal.Parse(dt.Rows[0]["TONG_CT_DATCOC"].ToString()).ToString("N0");
                TONG_CT_CUYENKHOAN = decimal.Parse(dt.Rows[0]["TONG_CT_CUYENKHOAN"].ToString()).ToString("N0");
                TONG_CT_CONGNO = decimal.Parse(dt.Rows[0]["TONG_CT_CONGNO"].ToString()).ToString("N0");
                TONG_CT_HOAHONG = decimal.Parse(dt.Rows[0]["TONG_CT_HOAHONG"].ToString()).ToString("N0");
                TONG_CT_DIEM = decimal.Parse(dt.Rows[0]["TONG_CT_DIEM"].ToString()).ToString("N0");
                TONG_CT_TIENGUI = decimal.Parse(dt.Rows[0]["TONG_CT_TIENGUI"].ToString()).ToString("N0");
                TONG_CT_DAILY = decimal.Parse(dt.Rows[0]["TONG_CT_DAILY"].ToString()).ToString("N0");
                TONG_CT_WEB = decimal.Parse(dt.Rows[0]["TONG_CT_WEB"].ToString()).ToString("N0");
                TONG_PHUTHUHUYVE = decimal.Parse(dt.Rows[0]["TONG_PHUTHUHUYVE"].ToString()).ToString("N0");
                TONG_PHUTHUINLAI = decimal.Parse(dt.Rows[0]["TONG_PHUTHUINLAI"].ToString()).ToString("N0");
            }
        }

        public class ItemCheck
        {
            public string? CheckNhanVien { get; set; }
            public string? CheckBSX { get; set; }
        }
        public class Item
        {
            public string? ID { get; set; }
            public string? DayToGo { get; set; }
            public string? DayOfArrival { get; set; }
            public string? HourToGo { get; set; }
            public string? HourOfArrival { get; set; }
            public string? ID_Trip { get; set; }
            public string? Name_Trip { get; set; }
            public string? Lock { get; set; }
            public string? Vessel { get; set; }
            public string? Note { get; set; }
            public string? sudung { get; set; }
        }

        public class ItemNV
        {
            public string? UserID { get; set; }
            public string? MaNV { get; set; }
            public string? MANHANVIEN { get; set; }
            public string? TENNHANVIEN { get; set; }
        }

        public class BAOCAO
        {
            public string? STT { get; set; }
            public string? TENTUYEN { get; set; }
            public string? NGAYDI { get; set; }
            public string? NGAYKHOIHANH { get; set; }
            public string? GIODI { get; set; }
            public string? LOAI { get; set; }
            public string? TENKHUVUC { get; set; }
            public decimal? SOLUONG { get; set; }
            public decimal? DONGIA { get; set; }
            public decimal? PHUTHU { get; set; }
            public decimal? THANHTIEN { get; set; }
            public decimal? CT_GIAMTRU { get; set; }
            public decimal? CT_CONGNO { get; set; }
            public decimal? CT_DATCOC { get; set; }
            public decimal? CT_DAILY { get; set; }
            public decimal? CT_WEB { get; set; }
            public decimal? CT_HH { get; set; }
            public decimal? CT_CUYENKHOAN { get; set; }
            public decimal? CT_TIENGUI { get; set; }
            public string? DIENGIAI { get; set; }
            public decimal? CT_HOAHONG { get; set; }
            public decimal? CT_DIEM { get; set; }
            public decimal? PHUTHUHUYVE { get; set; }
            public decimal? PHUTHUINLAI { get; set; }
            public string? GhiChuTM { get; set; }
            public decimal? THUCTHU { get; set; }
            public decimal? PHAINOP { get; set; }
            public string? NHANVIEN { get; set; }
        }

        public Bitmap stringToImage(string inputString)
        {
            byte[] imageBytes = Encoding.Unicode.GetBytes(inputString);
            using (MemoryStream ms = new MemoryStream(imageBytes))
            {
                return new Bitmap(ms);
            }
        }
    }
}
