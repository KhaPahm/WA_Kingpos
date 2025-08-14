using FastReport.Export.PdfSimple;
using FastReport.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using System.Data;
using System.Globalization;
using WA_Kingpos.Data;
using WA_Kingpos.Models;
using static WA_Kingpos.Pages.Baocao.DSHanghoa.IndexModel;

namespace WA_Kingpos.Pages.Baocao.BCDoanhSoNhanVien
{
    [Authorize]
    public class IndexModel : PageModel
    {
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

        public List<clsDoanhSoNV> listitem { set; get; } = new List<clsDoanhSoNV>();

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
        public string sTungay { get; set; } = "";
        [BindProperty]
        public string sDenngay { get; set; } = "";
        public IActionResult OnGet(string id, string id1, string id2, string id3, string id4, string id5, string id6, string id7, string id8)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowView("25030701", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                LoadData(id, id1, id2, id3, id4, id5, id6, id7, id8);
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "DSDondathangFina Get", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        public IActionResult OnPost(string submitButton, string id, string id1, string id2, string id3, string id4, string id5, string id6, string id7, string id8)
        {
            try
            {
                if (submitButton == "view")
                {
                    return RedirectToPage("Index", new { id = sTungay.Substring(0,10), id1 = sTungay.Substring(13, 10), id2 = sNhanvien, id3 = sKho, id4 = sKhachhang,id5=sHanghoa,id6=sTrangthai,id7=sVanchuyen,id8=sThanhtoan });
                }
                else if (submitButton == "print")
                {
                    // Khởi tạo báo cáo
                    WebReport webReport = new WebReport();

                    // Load mẫu báo cáo từ file .frx
                    string reportPath = Path.Combine(cls_Main.sFilePathReport, "rptDSDDH.frx");
                    webReport.Report.Load(reportPath);

                    // Gán Data Source vào báo cáo
                    string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                    string sSQL = "";
                    sSQL += "EXEC SP_W_BAOCAO_DSDONHANG " + cls_Main.SQLString(id) + "," + cls_Main.SQLString(id1) + "," + cls_Main.SQLString(id2) + "," + cls_Main.SQLString(id3) + "," + cls_Main.SQLString(id4) + "," + cls_Main.SQLString(id5) + "," + cls_Main.SQLString(id6) + "," + cls_Main.SQLString(id7) + "," + cls_Main.SQLString(id8) + "\n";
                    DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                    webReport.Report.RegisterData(dt, "TableMain");

                    // Gán Parameter vào báo cáo
                    webReport.Report.SetParameterValue("tencongty", cls_ConfigCashier.sTENCONGTY);
                    webReport.Report.SetParameterValue("diachi", cls_ConfigCashier.sDIACHICONGTY);
                    webReport.Report.SetParameterValue("dienthoai", cls_ConfigCashier.sSODIENTHOAICONGTY);

                    string tieude = "DANH SÁCH ĐƠN ĐẶT HÀNG";
                    string kho = "";
                    string nhanvien = "";
                    string khachhang = "";
                    string hanghoa = "";
                    string trangthai = "";
                    string vanchuyen = "";
                    string thanhtoan = "";
                    string sNgay = DateTime.ParseExact(id, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy") + " - " + DateTime.ParseExact(id1, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy");


                    NHANVIEN nv = Listnhanvien.Find(item => item.MA == id2);
                    if (nv != null)
                    {
                        nhanvien = nv.TEN;
                    }
                    KHO k = Listkho.Find(item => item.MA == id3);
                    if (k != null)
                    {
                        kho = k.TEN;
                    }
                    KHACHHANG kh = Listkhachhang.Find(item => item.MA == id4);
                    if (kh != null)
                    {
                        khachhang = kh.TEN;
                    }
                    HANGHOA hh = Listhanghoa.Find(item => item.MA == id5);
                    if (hh != null)
                    {
                        hanghoa = hh.TEN;
                    }
                    TRANGTHAI tt = Listtrangthai.Find(item => item.MA == id6);
                    if (tt != null)
                    {
                        trangthai = tt.TEN;
                    }
                    VANCHUYEN vc = Listvanchuyen.Find(item => item.MA == id7);
                    if (vc != null)
                    {
                        vanchuyen = vc.TEN;
                    }
                    THANHTOAN ttoan = Listthanhtoan.Find(item => item.MA == id8);
                    if (ttoan != null)
                    {
                        thanhtoan = ttoan.TEN;
                    }
                    webReport.Report.SetParameterValue("thoigian", sNgay);
                    webReport.Report.SetParameterValue("nhanvien", nhanvien);
                    webReport.Report.SetParameterValue("kho", kho);
                    webReport.Report.SetParameterValue("khachhang", khachhang);
                    webReport.Report.SetParameterValue("hanghoa", hanghoa);
                    webReport.Report.SetParameterValue("trangthai", trangthai);
                    webReport.Report.SetParameterValue("vanchuyen", vanchuyen);
                    webReport.Report.SetParameterValue("thanhtoan", thanhtoan);
                    //Render report
                    webReport.Report.Prepare();

                    // Export báo cáo sang PDF và trả về kết quả
                    MemoryStream stream = new MemoryStream();
                    webReport.Report.Export(new PDFSimpleExport(), stream);
                    stream.Position = 0;
                    return File(stream, "application/pdf", "rptDSDondathang.pdf");
                }
                else if (submitButton == "excel")
                {
                    //Tạo DataTable với dữ liệu của bảng bạn muốn xuất ra Excel
                    string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                    string sSQL = "";
                    sSQL += "EXEC SP_W_BAOCAO_DSDONHANG " + cls_Main.SQLString(id) + "," + cls_Main.SQLString(id1) + "," + cls_Main.SQLString(id2) + "," + cls_Main.SQLString(id3) + "," + cls_Main.SQLString(id4) + "," + cls_Main.SQLString(id5) + "," + cls_Main.SQLString(id6) + "," + cls_Main.SQLString(id7) + "," + cls_Main.SQLString(id8) + "\n";
                    DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                    DataView view = new System.Data.DataView(dt);
                    DataTable dataTable = view.ToTable("MyDataTable", false, "MAPDNDC", "NGAYLAPPHIEU", "TEN_KHO", "TENNHANVIEN", "TEN", "TONGTIEN", "TRANGTHAI", "NGUOIDUYET", "VANCHUYEN", "THANHTOAN");
                    dataTable.Columns["MAPDNDC"].ColumnName = "Số phiếu";
                    dataTable.Columns["NGAYLAPPHIEU"].ColumnName = "Ngày lập phiếu";
                    dataTable.Columns["TEN_KHO"].ColumnName = "Kho";
                    dataTable.Columns["TENNHANVIEN"].ColumnName = "Nhân viên";
                    dataTable.Columns["TEN"].ColumnName = "Khách hàng";
                    dataTable.Columns["TONGTIEN"].ColumnName = "Tổng tiền";
                    dataTable.Columns["TRANGTHAI"].ColumnName = "Trạng thái";
                    dataTable.Columns["NGUOIDUYET"].ColumnName = "Người duyệt";
                    dataTable.Columns["VANCHUYEN"].ColumnName = "Vận chuyển";
                    dataTable.Columns["THANHTOAN"].ColumnName = "Thanh toán";

                    //Thêm Parameter vào excel
                    string tencongty = cls_ConfigCashier.sTENCONGTY;
                    string diachi = "Địa chỉ : " + cls_ConfigCashier.sDIACHICONGTY;
                    string dienthoai = "Điện thoại : " + cls_ConfigCashier.sSODIENTHOAICONGTY;

                    string tieude = "DANH SÁCH ĐƠN ĐẶT HÀNG";
                    string kho = "";
                    string nhanvien = "";
                    string khachhang = "";
                    string hanghoa = "";
                    string trangthai = "";
                    string vanchuyen = "";
                    string thanhtoan = "";
                    string sNgay = DateTime.ParseExact(id, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy")+" - "+ DateTime.ParseExact(id1, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy");


                    NHANVIEN nv = Listnhanvien.Find(item => item.MA == id2);
                    if (nv != null)
                    {
                        nhanvien = nv.TEN;
                    }
                    KHO k = Listkho.Find(item => item.MA == id3);
                    if (k != null)
                    {
                        kho = k.TEN;
                    }
                    KHACHHANG kh = Listkhachhang.Find(item => item.MA == id4);
                    if (kh != null)
                    {
                        khachhang = kh.TEN;
                    }
                    HANGHOA hh = Listhanghoa.Find(item => item.MA == id5);
                    if (hh != null)
                    {
                        hanghoa = hh.TEN;
                    }
                    TRANGTHAI tt = Listtrangthai.Find(item => item.MA == id6);
                    if (tt != null)
                    {
                        trangthai = tt.TEN;
                    }
                    VANCHUYEN vc = Listvanchuyen.Find(item => item.MA == id7);
                    if (vc != null)
                    {
                        vanchuyen = vc.TEN;
                    }
                    THANHTOAN ttoan = Listthanhtoan.Find(item => item.MA == id8);
                    if (ttoan != null)
                    {
                        thanhtoan = ttoan.TEN;
                    }
                    // Tạo một package ExcelPackage từ EPPlus
                    using (ExcelPackage package = new ExcelPackage())
                    {
                        // Tạo một worksheet trong package
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("rptDSDondathang");
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
                        worksheet.Cells["A5"].Value = "Thời gian:";
                        worksheet.Cells["B5"].Style.Font.Bold = true;
                        worksheet.Cells["B5"].Value = sNgay;
                        //
                        worksheet.Cells["A6"].Style.Font.Bold = true;
                        worksheet.Cells["A6"].Value = "Nhân viên:";
                        worksheet.Cells["B6"].Style.Font.Bold = true;
                        worksheet.Cells["B6"].Value = nhanvien;
                        //
                        worksheet.Cells["A7"].Style.Font.Bold = true;
                        worksheet.Cells["A7"].Value = "Kho:";
                        worksheet.Cells["B7"].Style.Font.Bold = true;
                        worksheet.Cells["B7"].Value = kho;
                        //
                        worksheet.Cells["A8"].Style.Font.Bold = true;
                        worksheet.Cells["A8"].Value = "Khách hàng:";
                        worksheet.Cells["B8"].Style.Font.Bold = true;
                        worksheet.Cells["B8"].Value = khachhang;
                        //
                        worksheet.Cells["A9"].Style.Font.Bold = true;
                        worksheet.Cells["A9"].Value = "Hàng hóa:";
                        worksheet.Cells["B9"].Style.Font.Bold = true;
                        worksheet.Cells["B9"].Value = hanghoa;
                        //
                        worksheet.Cells["A10"].Style.Font.Bold = true;
                        worksheet.Cells["A10"].Value = "Trạng thái:";
                        worksheet.Cells["B10"].Style.Font.Bold = true;
                        worksheet.Cells["B10"].Value = trangthai;
                        //
                        worksheet.Cells["A11"].Style.Font.Bold = true;
                        worksheet.Cells["A11"].Value = "Vận chuyển:";
                        worksheet.Cells["B11"].Style.Font.Bold = true;
                        worksheet.Cells["B11"].Value = vanchuyen;
                        //
                        worksheet.Cells["A12"].Style.Font.Bold = true;
                        worksheet.Cells["A12"].Value = "Thanh toán:";
                        worksheet.Cells["B12"].Style.Font.Bold = true;
                        worksheet.Cells["B12"].Value = thanhtoan;

                        worksheet.Row(13).Style.Font.Bold = true;
                        // Đưa dữ liệu từ DataTable vào worksheet
                        worksheet.Cells["A13"].LoadFromDataTable(dataTable, true);

                        // Tự động điều chỉnh kích thước của tất cả cột dựa trên nội dung
                        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                        // Lưu tệp Excel và trả về kết quả
                        MemoryStream stream = new MemoryStream();
                        package.SaveAs(stream);
                        stream.Position = 0;
                        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "rptDSDondathang.xlsx");
                    }
                }
                LoadData(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "DSDondathang Post : ", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        private void LoadData(string id, string id1, string id2, string id3, string id4, string id5, string id6, string id7, string id8)
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
            //kho
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
            //khachhang
            sSQL = "select 0 as MA, N'Tất cả' as TEN" + "\n";
            sSQL += "union" + "\n";
            sSQL += "select MA, TEN from nhacungcap where SUDUNG=1 and LOAI=0";
            DataTable dtKhachhang = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dtKhachhang.Rows)
            {
                KHACHHANG Khachhang = new KHACHHANG();
                Khachhang.MA = dr["MA"].ToString();
                Khachhang.TEN = dr["TEN"].ToString();
                Listkhachhang.Add(Khachhang);
            }
            //Hanghoa
            sSQL = "select 0 as MA_HANGHOA, N'Tất cả' as TEN_HANGHOA" + "\n";
            sSQL += "union" + "\n";
            sSQL += "select MA_HANGHOA, TEN_HANGHOA from hanghoa where sudung=1";
            DataTable dtHanghoa = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dtHanghoa.Rows)
            {
                HANGHOA Hanghoa = new HANGHOA();
                Hanghoa.MA = dr["MA_HANGHOA"].ToString();
                Hanghoa.TEN = dr["TEN_HANGHOA"].ToString();
                Listhanghoa.Add(Hanghoa);
            }
            //trangthai
            DataTable dt = new DataTable();
            dt.Columns.Add("MA", typeof(int));
            dt.Columns.Add("TEN", typeof(string));
            dt.Rows.Add("0", "Tất cả");
            dt.Rows.Add("1", "Chờ duyệt");
            dt.Rows.Add("2", "Duyệt");
            dt.Rows.Add("3", "Không duyệt");
            foreach (DataRow dr in dt.Rows)
            {
                TRANGTHAI Trangthai = new TRANGTHAI();
                Trangthai.MA = dr["MA"].ToString();
                Trangthai.TEN = dr["TEN"].ToString();
                Listtrangthai.Add(Trangthai);
            }
            //vanchuyen
            dt.Rows.Clear();
            dt.Rows.Add("0", "Tất cả");
            dt.Rows.Add("1", "Đã bốc hàng");
            dt.Rows.Add("2", "Chưa bốc hàng");
            foreach (DataRow dr in dt.Rows)
            {
                VANCHUYEN Vanchuyen = new VANCHUYEN();
                Vanchuyen.MA = dr["MA"].ToString();
                Vanchuyen.TEN = dr["TEN"].ToString();
                Listvanchuyen.Add(Vanchuyen);
            }
            //Thanhtoan
            dt.Rows.Clear();
            dt.Rows.Add("0", "Tất cả");
            dt.Rows.Add("1", "Đã thanh toán");
            dt.Rows.Add("2", "Chưa thanh toán");
            foreach (DataRow dr in dt.Rows)
            {
                THANHTOAN Thanhtoan = new THANHTOAN();
                Thanhtoan.MA = dr["MA"].ToString();
                Thanhtoan.TEN = dr["TEN"].ToString();
                Listthanhtoan.Add(Thanhtoan);
            }
            //danh sach don dat hang
            sSQL = "";
            sSQL += "EXEC SP_W_BANGDOANHSO_FINA " + cls_Main.SQLString(id) + "," + cls_Main.SQLString(id1) + "," + cls_Main.SQLString(id2) + "," + cls_Main.SQLString(id3) + "," + cls_Main.SQLString(id4) + "," + cls_Main.SQLString(id5) + "," + cls_Main.SQLString(id6) + "," + cls_Main.SQLString(id7) + "," + cls_Main.SQLString(id8) + "\n";
            dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                clsDoanhSoNV item = new clsDoanhSoNV();
                item.TENKHACHHANG = dr["TENKHACHHANG"].ToString();
                item.TEN_KHO = dr["TEN_KHO"].ToString();
                item.NGAYLAP = String.Format("{0:dd-MM-yyyy HH:mm:ss}", dr["NGAYLAP"]);
                item.NGAYXUAT = String.Format("{0:dd-MM-yyyy HH:mm:ss}", dr["NGAYXUAT"]);
                item.TEN_HANGHOA = dr["TEN_HANGHOA"].ToString();
                item.KICHCO = decimal.Parse(dr["KICHCO"].ToString()).ToString("N3");
                item.HESO_QUYDOI = decimal.Parse(dr["HESO_QUYDOI"].ToString()).ToString("N3");
                item.SANLUONG_TRUOC_QUYDOI_THUNG = decimal.Parse(dr["SANLUONG_TRUOC_QUYDOI_THUNG"].ToString()).ToString("N3");
                item.SANLUONG_TRUOC_QUYDOI_TAN = decimal.Parse(dr["SANLUONG_TRUOC_QUYDOI_TAN"].ToString()).ToString("N3");
                item.QUYDOI_PBL = decimal.Parse(dr["QUYDOI_PBL"].ToString()).ToString("N3");
                item.QUYDOI_BONGOC = decimal.Parse(dr["QUYDOI_BONGOC"].ToString()).ToString("N3");
                item.TONGSANLUONG_SAU_QUYDOI = decimal.Parse(dr["TONGSANLUONG_SAU_QUYDOI"].ToString()).ToString("N3");
                item.DONGIA = decimal.Parse(dr["DONGIA"].ToString()).ToString("N3");
                item.THANHTIEN_PBL = decimal.Parse(dr["THANHTIEN_PBL"].ToString()).ToString("N3");
                item.THANHTIEN_GOC = decimal.Parse(dr["THANHTIEN_GOC"].ToString()).ToString("N3");
                item.TONGTHANHTIEN = decimal.Parse(dr["TONGTHANHTIEN"].ToString()).ToString("N3");
                //item.NODAUTHANG = decimal.Parse(dr["NODAUTHANG"].ToString()).ToString("N0");
                //item.THANHTOAN = decimal.Parse(dr["THANHTOAN"].ToString()).ToString("N0");
                //item.HOTRO = decimal.Parse(dr["HOTRO"].ToString()).ToString("N0");
                //item.CONLAI = decimal.Parse(dr["CONLAI"].ToString()).ToString("N0");
                //item.ICHECK = dr["ICHECK"].ToString();
                //item.HOAHONG = decimal.Parse(dr["HOAHONG"].ToString()).ToString("N0");
                //item.NGAYCK = dr["NGAYCK"].ToString();
                //item.NOINHANTIEN = dr["NOINHANTIEN"].ToString();
                //item.NGAYTRAHANG = dr["NGAYTRAHANG"].ToString();
                //item.GHICHU = dr["GHICHU"].ToString();
                listitem.Add(item);
            }
            if (!string.IsNullOrEmpty(id))
            {
                sTungay = id;

            }
            if (!string.IsNullOrEmpty(id1))
            {
                sDenngay = id1;
            }
            if (!string.IsNullOrEmpty(id2))
            {
                sNhanvien = id2;
            }
            if (!string.IsNullOrEmpty(id3))
            {
                sKho = id3;
            }
            if (!string.IsNullOrEmpty(id4))
            {
                sKhachhang = id4;
            }
            if (!string.IsNullOrEmpty(id5))
            {
                sHanghoa = id5;
            }
            if (!string.IsNullOrEmpty(id6))
            {
                sTrangthai = id6;
            }
            if (!string.IsNullOrEmpty(id7))
            {
                sVanchuyen = id7;
            }
            if (!string.IsNullOrEmpty(id8))
            {
                sThanhtoan = id8;
            }
            // khong reset ngay khi search
            if (!string.IsNullOrEmpty(id))
            {
                sTungay = sTungay + " - " + sDenngay;
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
    }
}
