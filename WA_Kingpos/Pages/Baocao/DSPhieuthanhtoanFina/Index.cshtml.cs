using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FastReport.Export.PdfSimple;
using FastReport.Web;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using System.Data;
using System.Globalization;
using WA_Kingpos.Data;
using WA_Kingpos.Models;
using static WA_Kingpos.Pages.Baocao.DSHanghoa.IndexModel;
using FastReport;

namespace WA_Kingpos.Pages.Baocao.DSPhieuthanhtoanFina
{
    [Authorize]
    public class IndexModel : PageModel
    {
        [BindProperty]
        public List<TAIXE> ListTaiXe { get; set; } = new List<TAIXE>();
        [BindProperty]
        public List<NHANVIEN> Listnhanvien { set; get; } = new List<NHANVIEN>();

        [BindProperty]
        public List<KHO> Listkho { set; get; } = new List<KHO>();

        [BindProperty]
        public List<KHACHHANG> Listkhachhang { set; get; } = new List<KHACHHANG>();

        [BindProperty]
        public List<HANGHOA> Listhanghoa { set; get; } = new List<HANGHOA>();

        [BindProperty]
        public List<TRANGTHAI> Listtrangthai { set; get; } = new List<TRANGTHAI>();
        [BindProperty]
        public List<VANCHUYEN> Listvanchuyen { set; get; } = new List<VANCHUYEN>();
        [BindProperty]
        public List<THANHTOAN> Listthanhtoan { set; get; } = new List<THANHTOAN>();
        [BindProperty]
        public List<DONHANG> Listdonhang { set; get; } = new List<DONHANG>();

        public List<clsDathang> listitem { set; get; } = new List<clsDathang>();
        public List<BIENSOXE> listBiensoxe { set; get; } = new List<BIENSOXE>();

        [BindProperty]
        public string sNhanvien { get; set; } = "";
        [BindProperty]
        public string sKho { get; set; } = "";
        [BindProperty]
        public string sKhachhang { get; set; } = "";
        [BindProperty]
        public string sHanghoa { get; set; } = "";
        [BindProperty]
        public string sTrangthai { get; set; } = "";
        [BindProperty]
        public string sVanchuyen { get; set; } = "";
        [BindProperty]
        public string sThanhtoan { get; set; } = "";
        [BindProperty]
        public string sTungayDat { get; set; } = "";
        [BindProperty]
        public string sDenngayDat { get; set; } = "";
        [BindProperty]
        public string sTaixe { get; set; } = "";
        [BindProperty]
        public string sBiensoxe { get; set; } = "";
        [BindProperty]
        public string sDonhang { get; set; } = "";
        public IActionResult OnGet(string id, string id1, string id2, string id3, string id4)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowView("23120902", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                LoadData(id, id1, id2, id3, id4);
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "DSPhieuthanhtoanFina Get", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        public IActionResult OnPost(string submitButton, string id, string id1, string id2, string id3, string id4)
        {
            try
            {
                if (submitButton == "view")
                {
                    return RedirectToPage("Index", new { id = sTungayDat.Substring(0, 10), id1 = sTungayDat.Substring(13, 10), id2 = sNhanvien, id3 = sDonhang, id4 = sTrangthai});
                }
                else if (submitButton == "print")
                {
                    // Khởi tạo báo cáo
                    WebReport webReport = new WebReport();

                    // Load mẫu báo cáo từ file .frx
                    //string reportPath = Path.Combine(cls_Main.sFilePathReport, "rptDSPhieuthanhtoan.frx");
                    string reportPath = Path.Combine(cls_Main.sFilePathReport, "rptDSPhieuTT.frx");
                    webReport.Report.Load(reportPath);

                    // Gán Data Source vào báo cáo
                    string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                    string sSQL = "";
                    sSQL += "EXEC SP_W_BAOCAO_DSPHIEUTHANHTOAN2 " + cls_Main.SQLString(id) + "," + cls_Main.SQLString(id1) + "," + cls_Main.SQLString(id2) + "," + cls_Main.SQLString(id3) + "," + cls_Main.SQLString(id4) + "\n";
                    DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                    webReport.Report.RegisterData(dt, "TableMain");
                    webReport.Report.GetDataSource("TableMain").Enabled = true;

                    //DataBand dtBand = webReport.Report.FindObject("")

                    // Gán Parameter vào báo cáo
                    webReport.Report.SetParameterValue("tencongty", cls_ConfigCashier.sTENCONGTY);
                    webReport.Report.SetParameterValue("diachi", cls_ConfigCashier.sDIACHICONGTY);
                    webReport.Report.SetParameterValue("dienthoai", cls_ConfigCashier.sSODIENTHOAICONGTY);

                    string tieude = "DANH SÁCH PHIẾU THANH TOÁN";
                    string nhanvien = "";
                    string donhang = "";
                    string trangthai = "";
                    string sNgay = DateTime.ParseExact(id, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy") + " - " + DateTime.ParseExact(id1, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy");

                    sSQL = "";
                    sSQL += "SELECT '0' as MA, N'Tất cả' as TEN" + "\n";
                    sSQL += "union" + "\n";
                    sSQL += "SELECT DISTINCT MAPDNDC as MA, MAPDNDC as TEN from PHIEUDATHANG";
                    Listdonhang.Clear();
                    DataTable dtDathang = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                    foreach (DataRow dr in dtDathang.Rows)
                    {
                        DONHANG dh = new DONHANG();
                        dh.MA = dr["MA"].ToString();
                        dh.TEN = dr["TEN"].ToString();
                        Listdonhang.Add(dh);
                    }

                    NHANVIEN nv = Listnhanvien.Find(item => item.MA == id2);
                    if (nv != null)
                    {
                        nhanvien = nv.TEN;
                    }
                    DONHANG hh = Listdonhang.Find(item => item.MA == id3);
                    if (hh != null)
                    {
                        donhang = hh.TEN;
                    }
                    TRANGTHAI k = Listtrangthai.Find(item => item.MA == id4);
                    if (k != null)
                    {
                        trangthai = k.TEN;
                    }
                    
                    webReport.Report.SetParameterValue("thoigian", sNgay);
                    webReport.Report.SetParameterValue("nhanvien", nhanvien);
                    webReport.Report.SetParameterValue("donhang", donhang);
                    webReport.Report.SetParameterValue("trangthai", trangthai);
                    //Render report
                    webReport.Report.Prepare();

                    // Export báo cáo sang PDF và trả về kết quả
                    MemoryStream stream = new MemoryStream();
                    webReport.Report.Export(new PDFSimpleExport(), stream);
                    stream.Position = 0;
                    return File(stream, "application/pdf", "rptDSPhieuthanhtoan.pdf");
                }
                else if (submitButton == "excel")
                {
                    //Tạo DataTable với dữ liệu của bảng bạn muốn xuất ra Excel
                    string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                    string sSQL = "";
                    sSQL += "EXEC SP_W_BAOCAO_DSPHIEUTHANHTOAN2 " + cls_Main.SQLString(id) + "," + cls_Main.SQLString(id1) + "," + cls_Main.SQLString(id2) + "," + cls_Main.SQLString(id3) + "," + cls_Main.SQLString(id4) + "\n";
                    DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                    DataView view = new System.Data.DataView(dt);
                    DataTable dataTable = view.ToTable("MyDataTable", false, "MAPDNDT", "NGUOILAP", "NGAYLAPPHIEU", "TONGTIEN", "LYDO", "TRANGTHAI", "NGUOIDUYET");
                    dataTable.Columns["MAPDNDT"].ColumnName = "Số phiếu";
                    dataTable.Columns["NGUOILAP"].ColumnName = "Người lập phiếu";
                    dataTable.Columns["NGAYLAPPHIEU"].ColumnName = "Ngày lập phiếu";
                    dataTable.Columns["TONGTIEN"].ColumnName = "Tổng tiền";
                    dataTable.Columns["LYDO"].ColumnName = "Lý do";
                    dataTable.Columns["TRANGTHAI"].ColumnName = "Trạng thái";
                    dataTable.Columns["NGUOIDUYET"].ColumnName = "Người duyệt";

                    //Thêm Parameter vào excel
                    string tencongty = cls_ConfigCashier.sTENCONGTY;
                    string diachi = "Địa chỉ : " + cls_ConfigCashier.sDIACHICONGTY;
                    string dienthoai = "Điện thoại : " + cls_ConfigCashier.sSODIENTHOAICONGTY;

                    string tieude = "DANH SÁCH PHIẾU THANH TOÁN";
                    string kho = "";
                    string nhanvien = "";
                    string khachhang = "";
                    string hanghoa = "";
                    string donhang = "";
                    string trangthai = "";
                    string taixe = "";
                    string biensoxe = "";
                    string sNgay = DateTime.ParseExact(id, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy") + " - " + DateTime.ParseExact(id1, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy");

                    sSQL = "";
                    sSQL += "SELECT '0' as MA, N'Tất cả' as TEN" + "\n";
                    sSQL += "union" + "\n";
                    sSQL += "SELECT DISTINCT MAPDNDC as MA, MAPDNDC as TEN from PHIEUDATHANG";
                    Listdonhang.Clear();
                    DataTable dtDathang = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                    foreach (DataRow dr in dtDathang.Rows)
                    {
                        DONHANG dh = new DONHANG();
                        dh.MA = dr["MA"].ToString();
                        dh.TEN = dr["TEN"].ToString();
                        Listdonhang.Add(dh);
                    }

                    NHANVIEN nv = Listnhanvien.Find(item => item.MA == id2);
                    if (nv != null)
                    {
                        nhanvien = nv.TEN;
                    }
                    DONHANG hh = Listdonhang.Find(item => item.MA == id3);
                    if (hh != null)
                    {
                        donhang = hh.TEN;
                    }
                    TRANGTHAI k = Listtrangthai.Find(item => item.MA == id4);
                    if (k != null)
                    {
                        trangthai = k.TEN;
                    }

                    // Tạo một package ExcelPackage từ EPPlus
                    using (ExcelPackage package = new ExcelPackage())
                    {
                        // Tạo một worksheet trong package
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("rptDSPhieuthanhtoan");
                        worksheet.Cells["A1:I1"].Merge = true;
                        worksheet.Cells["A1"].Style.Font.Bold = true;
                        worksheet.Cells["A1"].Value = tencongty;

                        worksheet.Cells["A2:I2"].Merge = true;
                        worksheet.Cells["A2"].Style.Font.Bold = true;
                        worksheet.Cells["A2"].Value = diachi;

                        worksheet.Cells["A3:I3"].Merge = true;
                        worksheet.Cells["A3"].Style.Font.Bold = true;
                        worksheet.Cells["A3"].Value = dienthoai;

                        worksheet.Cells["A4:I4"].Merge = true;
                        worksheet.Cells["A4"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells["A4"].Style.Font.Bold = true;
                        worksheet.Cells["A4"].Value = tieude;
                        //
                        worksheet.Cells["A5"].Style.Font.Bold = true;
                        worksheet.Cells["A5"].Value = "Ngày:";
                        worksheet.Cells["B5"].Style.Font.Bold = true;
                        worksheet.Cells["B5"].Value = sNgay;
                        //
                        worksheet.Cells["A6"].Style.Font.Bold = true;
                        worksheet.Cells["A6"].Value = "Nhân viên:";
                        worksheet.Cells["B6"].Style.Font.Bold = true;
                        worksheet.Cells["B6"].Value = nhanvien;
                        //
                        worksheet.Cells["A7"].Style.Font.Bold = true;
                        worksheet.Cells["A7"].Value = "Đơn hàng:";
                        worksheet.Cells["B7"].Style.Font.Bold = true;
                        worksheet.Cells["B7"].Value = donhang;
                        //
                        worksheet.Cells["A8"].Style.Font.Bold = true;
                        worksheet.Cells["A8"].Value = "Trạng thái:";
                        worksheet.Cells["B8"].Style.Font.Bold = true;
                        worksheet.Cells["B8"].Value = trangthai;
                        
                        worksheet.Row(9).Style.Font.Bold = true;
                        // Đưa dữ liệu từ DataTable vào worksheet
                        worksheet.Cells["A9"].LoadFromDataTable(dataTable, true);

                        // Tự động điều chỉnh kích thước của tất cả cột dựa trên nội dung
                        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                        // Lưu tệp Excel và trả về kết quả
                        MemoryStream stream = new MemoryStream();
                        package.SaveAs(stream);
                        stream.Position = 0;
                        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "rptDSPhieuthanhtoan.xlsx");
                    }
                }
                LoadData(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "PDFPhieuthanhtoan Post : ", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        private void LoadData(string id, string id1, string id2, string id3, string id4)
        {
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "";
            //nhanvien
            sSQL = "select 0 as MANHANVIEN, N'Tất cả' as TENNHANVIEN" + "\n";
            sSQL += "union" + "\n";
            sSQL += "SELECT MANHANVIEN, TENNHANVIEN from DM_NHANVIEN where SUDUNG=1";
            DataTable dtNhanvien = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dtNhanvien.Rows)
            {
                NHANVIEN Nhanvien = new NHANVIEN();
                Nhanvien.MA = dr["MANHANVIEN"].ToString();
                Nhanvien.TEN = dr["TENNHANVIEN"].ToString();
                Listnhanvien.Add(Nhanvien);
            }
            //don hang
            sSQL = "";
            sSQL += "SELECT '0' as MA, N'Tất cả' as TEN" + "\n";
            sSQL += "union" + "\n";
            sSQL += "SELECT DISTINCT MAPDNDC as MA, MAPDNDC as TEN from PHIEUDATHANG";
            DataTable dtDathang = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dtDathang.Rows)
            {
                DONHANG dh = new DONHANG();
                dh.MA = dr["MA"].ToString();
                dh.TEN = dr["TEN"].ToString();
                Listdonhang.Add(dh);
            }
            //kho
            sSQL = "";
            sSQL = "select 0 as MA_KHO, N'Tất cả' as TEN_KHO" + "\n";
            sSQL += "union" + "\n";
            sSQL += "select MA_KHO, TEN_KHO from kho where SUDUNG=1";
            DataTable dtKho = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dtKho.Rows)
            {
                KHO Kho = new KHO();
                Kho.MA = dr["MA_KHO"].ToString();
                Kho.TEN = dr["TEN_KHO"].ToString();
                Listkho.Add(Kho);
            }
            //trang thai
            DataTable dt = new DataTable();
            dt.Columns.Add("MA", typeof(int));
            dt.Columns.Add("TEN", typeof(string));
            dt.Rows.Add("0", "Tất cả");
            dt.Rows.Add("1", "Chưa duyệt");
            dt.Rows.Add("2", "Đã duyệt");
            dt.Rows.Add("3", "Không duyệt");
            foreach (DataRow dr in dt.Rows)
            {
                TRANGTHAI Trangthai = new TRANGTHAI();
                Trangthai.MA = dr["MA"].ToString();
                Trangthai.TEN = dr["TEN"].ToString();
                Listtrangthai.Add(Trangthai);
            }
            //danh sach don dat hang
            sSQL = "";
            sSQL += "EXEC SP_W_BAOCAO_DSPHIEUTHANHTOAN2 " + cls_Main.SQLString(id) + "," + cls_Main.SQLString(id1) + "," + cls_Main.SQLString(id2) + "," + cls_Main.SQLString(id3) + "," + cls_Main.SQLString(id4) + "\n";
            dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                clsDathang item = new clsDathang();
                item.maphieu = dr["MAPDNDT"].ToString();
                item.nhanvien = dr["NGUOILAP"].ToString();
                item.ngaylap = dr["NGAYLAPPHIEU"].ToString();
                item.thanhtien = decimal.Parse(dr["TONGTIEN"].ToString()).ToString("N0");
                item.trangthai1 = dr["LYDO"].ToString();
                item.trangthai = dr["TRANGTHAI"].ToString();
                item.khachhang = dr["NGUOIDUYET"].ToString();
                listitem.Add(item);
            }

            if (!string.IsNullOrEmpty(id))
            {
                sTungayDat = id;
            }
            if (!string.IsNullOrEmpty(id1))
            {
                sDenngayDat = id1;
            }
            if (!string.IsNullOrEmpty(id2))
            {
                sNhanvien = id2;
            }
            if (!string.IsNullOrEmpty(id3))
            {
                sDonhang = id3;
            }
            if (!string.IsNullOrEmpty(id4))
            {
                sTrangthai = id4;
            }
            // khong reset ngay khi search
            if (!string.IsNullOrEmpty(id))
            {
                sTungayDat = sTungayDat + " - " + sDenngayDat;
            }
        }

        public class NHANVIEN
        {
            public string MA { get; set; }
            public string TEN { get; set; }
        }
        public class KHO
        {
            public string MA { get; set; }
            public string TEN { get; set; }
        }
        public class KHACHHANG
        {
            public string MA { get; set; }
            public string TEN { get; set; }
        }
        public class HANGHOA
        {
            public string MA { get; set; }
            public string TEN { get; set; }
        }
        public class TRANGTHAI
        {
            public string MA { get; set; }
            public string TEN { get; set; }
        }
        public class VANCHUYEN
        {
            public string MA { get; set; }
            public string TEN { get; set; }
        }
        public class THANHTOAN
        {
            public string MA { get; set; }
            public string TEN { get; set; }
        }

        public class TAIXE
        {
            public string MA { get; set; }
            public string TEN { get; set; }
        }

        public class BIENSOXE
        {
            public string MA { get; set; }
            public string TEN { get; set; }
        }

        public class DONHANG
        {
            public string MA { get; set; }
            public string TEN { get; set; }
        }
    }
}
