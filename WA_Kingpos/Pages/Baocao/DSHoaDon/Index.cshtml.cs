using FastReport.Export.PdfSimple;
using FastReport.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeOpenXml;
using System.Data;
using System.Text.Json;
using WA_Kingpos.Data;
using WA_Kingpos.Models;
using static WA_Kingpos.Pages.Index2Model;

namespace WA_Kingpos.Pages.Baocao.DSHoaDon
{
    [Authorize]
    public class IndexModel : PageModel
    {
        [BindProperty]
        public List<CUAHANG_CBO> ListCuaHang { set; get; } = new List<CUAHANG_CBO>();
        [BindProperty]
        public List<KHO_CBO> ListKho { set; get; } = new List<KHO_CBO>();
        [BindProperty]
        public List<QUAY_CBO> ListQuay { set; get; } = new List<QUAY_CBO>();
        [BindProperty]
        public List<NHOMHANG_CBO> Listnhomhang { set; get; } = new List<NHOMHANG_CBO>();
        [BindProperty]
        public List<HANGHOA_CBO> ListHanghoa { set; get; } = new List<HANGHOA_CBO>();
        [BindProperty]
        public List<NHANVIEN_CBO> ListNhanvien { set; get; } = new List<NHANVIEN_CBO>();
        [BindProperty]
        public List<HOADONGIAMGIA_CBO> listHoadonGiamgia { set; get; } = new List<HOADONGIAMGIA_CBO>();
        [BindProperty]
        public List<NGANHHANG_CBO> listNganhhang { set; get; } = new List<NGANHHANG_CBO>();
        public List<DanhSachHoaDon> listHD { set; get; } = new List<DanhSachHoaDon>();

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

        public decimal tongTien = 0;
        public decimal tienMat = 0;
        public decimal tienVoucher = 0;
        public decimal tienVisa = 0;
        public decimal tienChuyenKhoan = 0;
        public decimal tienTheTTNB = 0;
        public decimal viMOCA = 0;
        public decimal viVNPAY = 0;
        public decimal viZALO = 0;
        public decimal viMOMO = 0;
        public decimal viVIETTEL = 0;
        public decimal viSHOPEE = 0;
        public IActionResult OnGet(string id, string id1, string id2, string id3)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowView("48", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                LoadData(id, id1, id2, id3);
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "DSHangHoa Get", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        public IActionResult OnPost(string submitButton, string id, string id1, string id2, string id3)
        {
            try
            {
                if (submitButton == "view")
                {
                    string[] dateParts = sTungay.Split(new string[] { " - " }, StringSplitOptions.None);
                    return RedirectToPage("Index", new { id = dateParts[0], id1 = dateParts[1], id2, id3 = sCombo});
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
                    string reportPath = Path.Combine(cls_Main.sFilePathReport, "rptDSHD.frx");
                    webReport.Report.Load(reportPath);

                    // Gán Data Source vào báo cáo
                    string selectedValue = "";
                    string scomboBox = "";
                    sSQL = string.Empty;
                    if (id2 != null && id2.Equals("cuahang"))
                    {
                        sSQL += "EXEC SP_BAOCAO_HOADON @TUNGAY=" + cls_Main.SQLString(id) + ", @DENNGAY=" + cls_Main.SQLString(id1) + ",@DIEUKIEN=" + cls_Main.SQLString(id3) + ",@TYPE=" + cls_Main.SQLString("CH") + ", @USERLOGIN=" + cls_Main.SQLString(userID);
                        selectedValue = "Theo cửa hàng:";
                        CUAHANG_CBO val = ListCuaHang.Find(item => item.MA == id3);
                        if (val != null)
                        {
                            scomboBox = val.TEN;
                        }
                    }
                    else if (id2 != null && id2.Equals("kho"))
                    {
                        sSQL += "EXEC SP_BAOCAO_HOADON @TUNGAY=" + cls_Main.SQLString(id) + ", @DENNGAY=" + cls_Main.SQLString(id1) + ",@DIEUKIEN=" + cls_Main.SQLString(id3) + ",@TYPE=" + cls_Main.SQLString("KO") + ", @USERLOGIN=" + cls_Main.SQLString(userID);
                        selectedValue = "Theo kho:";
                        KHO_CBO val = ListKho.Find(item => item.MA == id3);
                        if (val != null)
                        {
                            scomboBox = val.TEN;
                        }
                    }
                    else if (id2 != null && id2.Equals("quay"))
                    {
                        sSQL += "EXEC SP_BAOCAO_HOADON @TUNGAY=" + cls_Main.SQLString(id) + ", @DENNGAY=" + cls_Main.SQLString(id1) + ",@DIEUKIEN=" + cls_Main.SQLString(id3) + ",@TYPE=" + cls_Main.SQLString("QA") + ", @USERLOGIN=" + cls_Main.SQLString(userID);
                        selectedValue = "Theo quầy:";
                        QUAY_CBO val = ListQuay.Find(item => item.MA == id3);
                        if (val != null)
                        {
                            scomboBox = val.TEN;
                        }
                    }
                    else if (id2 != null && id2.Equals("nhomhang"))
                    {
                        sSQL += "EXEC SP_BAOCAO_HOADON @TUNGAY=" + cls_Main.SQLString(id) + ", @DENNGAY=" + cls_Main.SQLString(id1) + ",@DIEUKIEN=" + cls_Main.SQLString(id3) + ",@TYPE=" + cls_Main.SQLString("NH") + ", @USERLOGIN=" + cls_Main.SQLString(userID);
                        selectedValue = "Theo nhóm hàng:";
                        NHOMHANG_CBO val = Listnhomhang.Find(item => item.MA == id3);
                        if (val != null)
                        {
                            scomboBox = val.TEN;
                        }
                    }
                    else if (id2 != null && id2.Equals("hanghoa"))
                    {
                        sSQL += "EXEC SP_BAOCAO_HOADON @TUNGAY=" + cls_Main.SQLString(id) + ", @DENNGAY=" + cls_Main.SQLString(id1) + ",@DIEUKIEN=" + cls_Main.SQLString(id3) + ",@TYPE=" + cls_Main.SQLString("HH") + ", @USERLOGIN=" + cls_Main.SQLString(userID);
                        selectedValue = "Theo hàng hóa:";
                        HANGHOA_CBO val = ListHanghoa.Find(item => item.MA == id3);
                        if (val != null)
                        {
                            scomboBox = val.TEN;
                        }
                    }
                    else if (id2 != null && id2.Equals("nhanvien"))
                    {
                        sSQL += "EXEC SP_BAOCAO_HOADON @TUNGAY=" + cls_Main.SQLString(id) + ", @DENNGAY=" + cls_Main.SQLString(id1) + ",@DIEUKIEN=" + cls_Main.SQLString(id3) + ",@TYPE=" + cls_Main.SQLString("NV") + ", @USERLOGIN=" + cls_Main.SQLString(userID);
                        selectedValue = "Theo nhân viên:";
                        NHANVIEN_CBO val = ListNhanvien.Find(item => item.MA == id3);
                        if (val != null)
                        {
                            scomboBox = val.TEN;
                        }
                    }
                    else if (id2 != null && id2.Equals("hdgiamgia"))
                    {
                        sSQL += "EXEC SP_BAOCAO_HOADON @TUNGAY=" + cls_Main.SQLString(id) + ", @DENNGAY=" + cls_Main.SQLString(id1) + ",@DIEUKIEN=" + cls_Main.SQLString(id3) + ",@TYPE=" + cls_Main.SQLString("GG") + ", @USERLOGIN=" + cls_Main.SQLString(userID);
                        selectedValue = "Theo hđ giảm giá:";
                        
                    }
                    else if (id2 != null && id2.Equals("nganhhang"))
                    {
                        sSQL += "EXEC SP_BAOCAO_HOADON @TUNGAY=" + cls_Main.SQLString(id) + ", @DENNGAY=" + cls_Main.SQLString(id1) + ",@DIEUKIEN=" + cls_Main.SQLString(id3) + ",@TYPE=" + cls_Main.SQLString("NGH") + ", @USERLOGIN=" + cls_Main.SQLString(userID);
                        selectedValue = "Theo ngành hàng:";
                        NGANHHANG_CBO val = listNganhhang.Find(item => item.MA == id3);
                        if (val != null)
                        {
                            scomboBox = val.TEN;
                        }
                    }

                    DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                    webReport.Report.RegisterData(dt, "TableMain");

                    // Gán Parameter vào báo cáo
                    webReport.Report.SetParameterValue("tencongty", cls_ConfigCashier.sTENCONGTY);
                    webReport.Report.SetParameterValue("diachi", cls_ConfigCashier.sDIACHICONGTY);
                    webReport.Report.SetParameterValue("dienthoai", cls_ConfigCashier.sSODIENTHOAICONGTY);

                    webReport.Report.SetParameterValue("selectedValue", selectedValue + ":");
                    webReport.Report.SetParameterValue("scomboBox", scomboBox);
                    webReport.Report.SetParameterValue("sTungay", id + " - " + id1);

                    //Render report
                    webReport.Report.Prepare();

                    // Export báo cáo sang PDF và trả về kết quả
                    MemoryStream stream = new MemoryStream();
                    webReport.Report.Export(new PDFSimpleExport(), stream);
                    stream.Position = 0;
                    return File(stream, "application/pdf", "rptDSHD.pdf");
                }
                else if (submitButton == "excel")
                {
                    // Tạo DataTable với dữ liệu của bảng bạn muốn xuất ra Excel
                    string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                    string sSQL = "";
                    string selectedValue = "";
                    string scomboBox = "";
                    sSQL = string.Empty;
                    if (id2 != null && id2.Equals("cuahang"))
                    {
                        sSQL += "EXEC SP_BAOCAO_HOADON @TUNGAY=" + cls_Main.SQLString(id) + ", @DENNGAY=" + cls_Main.SQLString(id1) + ",@DIEUKIEN=" + cls_Main.SQLString(id3) + ",@TYPE=" + cls_Main.SQLString("CH") + ", @USERLOGIN=" + cls_Main.SQLString(userID);
                        selectedValue = "Theo cửa hàng:";
                        CUAHANG_CBO val = ListCuaHang.Find(item => item.MA == id3);
                        if (val != null)
                        {
                            scomboBox = val.TEN;
                        }
                    }
                    else if (id2 != null && id2.Equals("kho"))
                    {
                        sSQL += "EXEC SP_BAOCAO_HOADON @TUNGAY=" + cls_Main.SQLString(id) + ", @DENNGAY=" + cls_Main.SQLString(id1) + ",@DIEUKIEN=" + cls_Main.SQLString(id3) + ",@TYPE=" + cls_Main.SQLString("KO") + ", @USERLOGIN=" + cls_Main.SQLString(userID);
                        selectedValue = "Theo kho:";
                        KHO_CBO val = ListKho.Find(item => item.MA == id3);
                        if (val != null)
                        {
                            scomboBox = val.TEN;
                        }
                    }
                    else if (id2 != null && id2.Equals("quay"))
                    {
                        sSQL += "EXEC SP_BAOCAO_HOADON @TUNGAY=" + cls_Main.SQLString(id) + ", @DENNGAY=" + cls_Main.SQLString(id1) + ",@DIEUKIEN=" + cls_Main.SQLString(id3) + ",@TYPE=" + cls_Main.SQLString("QA") + ", @USERLOGIN=" + cls_Main.SQLString(userID);
                        selectedValue = "Theo quầy:";
                        QUAY_CBO val = ListQuay.Find(item => item.MA == id3);
                        if (val != null)
                        {
                            scomboBox = val.TEN;
                        }
                    }
                    else if (id2 != null && id2.Equals("nhomhang"))
                    {
                        sSQL += "EXEC SP_BAOCAO_HOADON @TUNGAY=" + cls_Main.SQLString(id) + ", @DENNGAY=" + cls_Main.SQLString(id1) + ",@DIEUKIEN=" + cls_Main.SQLString(id3) + ",@TYPE=" + cls_Main.SQLString("NH") + ", @USERLOGIN=" + cls_Main.SQLString(userID);
                        selectedValue = "Theo nhóm hàng:";
                        NHOMHANG_CBO val = Listnhomhang.Find(item => item.MA == id3);
                        if (val != null)
                        {
                            scomboBox = val.TEN;
                        }
                    }
                    else if (id2 != null && id2.Equals("hanghoa"))
                    {
                        sSQL += "EXEC SP_BAOCAO_HOADON @TUNGAY=" + cls_Main.SQLString(id) + ", @DENNGAY=" + cls_Main.SQLString(id1) + ",@DIEUKIEN=" + cls_Main.SQLString(id3) + ",@TYPE=" + cls_Main.SQLString("HH") + ", @USERLOGIN=" + cls_Main.SQLString(userID);
                        selectedValue = "Theo hàng hóa:";
                        HANGHOA_CBO val = ListHanghoa.Find(item => item.MA == id3);
                        if (val != null)
                        {
                            scomboBox = val.TEN;
                        }
                    }
                    else if (id2 != null && id2.Equals("nhanvien"))
                    {
                        sSQL += "EXEC SP_BAOCAO_HOADON @TUNGAY=" + cls_Main.SQLString(id) + ", @DENNGAY=" + cls_Main.SQLString(id1) + ",@DIEUKIEN=" + cls_Main.SQLString(id3) + ",@TYPE=" + cls_Main.SQLString("NV") + ", @USERLOGIN=" + cls_Main.SQLString(userID);
                        selectedValue = "Theo nhân viên:";
                        NHANVIEN_CBO val = ListNhanvien.Find(item => item.MA == id3);
                        if (val != null)
                        {
                            scomboBox = val.TEN;
                        }
                    }
                    else if (id2 != null && id2.Equals("hdgiamgia"))
                    {
                        sSQL += "EXEC SP_BAOCAO_HOADON @TUNGAY=" + cls_Main.SQLString(id) + ", @DENNGAY=" + cls_Main.SQLString(id1) + ",@DIEUKIEN=" + cls_Main.SQLString(id3) + ",@TYPE=" + cls_Main.SQLString("GG") + ", @USERLOGIN=" + cls_Main.SQLString(userID);
                        selectedValue = "Theo hđ giảm giá:";

                    }
                    else if (id2 != null && id2.Equals("nganhhang"))
                    {
                        sSQL += "EXEC SP_BAOCAO_HOADON @TUNGAY=" + cls_Main.SQLString(id) + ", @DENNGAY=" + cls_Main.SQLString(id1) + ",@DIEUKIEN=" + cls_Main.SQLString(id3) + ",@TYPE=" + cls_Main.SQLString("NGH") + ", @USERLOGIN=" + cls_Main.SQLString(userID);
                        selectedValue = "Theo ngành hàng:";
                        NGANHHANG_CBO val = listNganhhang.Find(item => item.MA == id3);
                        if (val != null)
                        {
                            scomboBox = val.TEN;
                        }
                    }
                    DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                    DataView view = new System.Data.DataView(dt);
                    DataTable dataTable = view.ToTable("MyDataTable", false, "MA_HOADON", "TenNV", "NGAYTAO", "TONGTIEN", "MONEYCASH", "MONEYVOUCHER", "MONEYVISA", "MONEYCHUYENKHOAN", "MONEYINNERCARD", "MONEYMOCA", "MONEYVNPAY", "MONEYZALO", "MONEYMOMO", "MONEYVIETTEL", "MONEYSHOPEEPAY", "KHACHHANG", "GHICHU1");
                    dataTable.Columns["MA_HOADON"].ColumnName = "Mã hóa đơn";
                    dataTable.Columns["TenNV"].ColumnName = "Tên nhân viên";
                    dataTable.Columns["NGAYTAO"].ColumnName = "Ngày tạo";
                    dataTable.Columns["TONGTIEN"].ColumnName = "Tổng tiền";
                    dataTable.Columns["MONEYCASH"].ColumnName = "Tiền mặt";
                    dataTable.Columns["MONEYVOUCHER"].ColumnName = "Tiền voucher";
                    dataTable.Columns["MONEYVISA"].ColumnName = "Tiền VISA";
                    dataTable.Columns["MONEYCHUYENKHOAN"].ColumnName = "Tiền chuyển khoản";
                    dataTable.Columns["MONEYINNERCARD"].ColumnName = "Tiền thẻ TTNB";
                    dataTable.Columns["MONEYMOCA"].ColumnName = "Ví MOCA";
                    dataTable.Columns["MONEYVNPAY"].ColumnName = "Ví VNPAY";
                    dataTable.Columns["MONEYZALO"].ColumnName = "Ví ZALOPAY";
                    dataTable.Columns["MONEYMOMO"].ColumnName = "Ví MOMO";
                    dataTable.Columns["MONEYVIETTEL"].ColumnName = "Ví VIETTEL";
                    dataTable.Columns["MONEYSHOPEEPAY"].ColumnName = "Ví SHOPEEPAY";
                    dataTable.Columns["KHACHHANG"].ColumnName = "Khách hàng";
                    dataTable.Columns["GHICHU1"].ColumnName = "Ghi chú";

                    //Thêm Parameter vào excel
                    string tencongty = cls_ConfigCashier.sTENCONGTY;
                    string diachi = "Địa chỉ : " + cls_ConfigCashier.sDIACHICONGTY;
                    string dienthoai = "Điện thoại : " + cls_ConfigCashier.sSODIENTHOAICONGTY;

                    string tieude = "DANH SÁCH HÓA ĐƠN";

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

                        worksheet.Cells["A4:L4"].Merge = true;
                        worksheet.Cells["A4"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells["A4"].Style.Font.Bold = true;
                        worksheet.Cells["A4"].Value = tieude;
                        //
                        worksheet.Cells["A5:L5"].Style.Font.Bold = true;
                        worksheet.Cells["A5"].Value = "Ngày:";
                        worksheet.Cells["B5"].Style.Font.Bold = true;
                        worksheet.Cells["B5"].Value = id + " - " + id1;
                        //
                        worksheet.Cells["A6"].Style.Font.Bold = true;
                        worksheet.Cells["A6"].Value = "Tìm kiếm:";
                        worksheet.Cells["B6"].Style.Font.Bold = true;
                        worksheet.Cells["B6"].Value = selectedValue;
                        //
                        worksheet.Cells["A7"].Style.Font.Bold = true;
                        worksheet.Cells["A7"].Value = "Chi tiết:";
                        worksheet.Cells["B7"].Style.Font.Bold = true;
                        worksheet.Cells["B7"].Value = scomboBox; 

                        worksheet.Row(10).Style.Font.Bold = true;
                        // Đưa dữ liệu từ DataTable vào worksheet
                        worksheet.Cells["A10"].LoadFromDataTable(dataTable, true);

                        // Tự động điều chỉnh kích thước của tất cả cột dựa trên nội dung
                        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                        // Lưu tệp Excel và trả về kết quả
                        MemoryStream stream = new MemoryStream();
                        package.SaveAs(stream);
                        stream.Position = 0;
                        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "rptDSHoaDon.xlsx");
                    }
                }
                LoadData(string.Empty, string.Empty, string.Empty, string.Empty);
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "DSHangHoa Post : ", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        private void LoadData(string id, string id1, string id2, string id3)
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

            //cuahang
            sSQL = "EXEC SP_GET_CUAHANG_THEO_TAIKHOAN " + cls_Main.SQLString(userID) + ", " + cls_Main.SQLString("2");
            DataTable dtCuahang = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);

            foreach (DataRow dr in dtCuahang.Rows)
            {
                CUAHANG_CBO Cuahang = new CUAHANG_CBO();
                Cuahang.MA = dr["MA"].ToString();
                Cuahang.TEN = dr["TEN"].ToString();
                ListCuaHang.Add(Cuahang);
            }

            //kho
            sSQL = "EXEC SP_GET_CUAHANG_KHO_QUAY_THEO_NHANVIEN " + cls_Main.SQLString(userID) + ", " + cls_Main.SQLString("1");
            DataTable dtKho = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);

            foreach (DataRow dr in dtKho.Rows)
            {
                KHO_CBO kho = new KHO_CBO();
                kho.MA = dr["MA"].ToString();
                kho.TEN = dr["TEN"].ToString();
                ListKho.Add(kho);
            }

            //quay
            sSQL = "EXEC SP_GET_CUAHANG_KHO_QUAY_THEO_NHANVIEN " + cls_Main.SQLString(userID) + ", " + cls_Main.SQLString("2");
            DataTable dtQuay = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);

            foreach (DataRow dr in dtQuay.Rows)
            {
                QUAY_CBO quay = new QUAY_CBO();
                quay.MA = dr["MA"].ToString();
                quay.TEN = dr["TEN"].ToString();
                ListQuay.Add(quay);
            }

            //nhom hang
            sSQL = "EXEC SP_GET_CUAHANG_KHO_QUAY_THEO_NHANVIEN " + cls_Main.SQLString(userID) + ", " + cls_Main.SQLString("3");
            DataTable dtNhomhang = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);

            foreach (DataRow dr in dtNhomhang.Rows)
            {
                NHOMHANG_CBO nh = new NHOMHANG_CBO();
                nh.MA = dr["MA"].ToString();
                nh.TEN = dr["TEN"].ToString();
                Listnhomhang.Add(nh);
            }

            //hang hoa
            sSQL = "EXEC SP_GET_CUAHANG_KHO_QUAY_THEO_NHANVIEN " + cls_Main.SQLString(userID) + ", " + cls_Main.SQLString("4");
            DataTable dtHanghoa = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);

            foreach (DataRow dr in dtHanghoa.Rows)
            {
                HANGHOA_CBO hh = new HANGHOA_CBO();
                hh.MA = dr["MA"].ToString();
                hh.TEN = dr["TEN"].ToString();
                ListHanghoa.Add(hh);
            }

            //nhan vien
            sSQL = "EXEC SP_GET_CUAHANG_KHO_QUAY_THEO_NHANVIEN " + cls_Main.SQLString(userID) + ", " + cls_Main.SQLString("5");
            DataTable dtNhanvien = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);

            foreach (DataRow dr in dtNhanvien.Rows)
            {
                NHANVIEN_CBO nv = new NHANVIEN_CBO();
                nv.MA = dr["MA"].ToString();
                nv.TEN = dr["TEN"].ToString();
                ListNhanvien.Add(nv);
            }

            //nganh hang
            sSQL = "EXEC SP_GET_CUAHANG_KHO_QUAY_THEO_NHANVIEN " + cls_Main.SQLString(userID) + ", " + cls_Main.SQLString("7");
            DataTable dtNganhhang = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);

            foreach (DataRow dr in dtNganhhang.Rows)
            {
                NGANHHANG_CBO nh = new NGANHHANG_CBO();
                nh.MA = dr["MA"].ToString();
                nh.TEN = dr["TEN"].ToString();
                listNganhhang.Add(nh);
            }


            sSQL = string.Empty;
            if (id2 != null && id2.Equals("cuahang"))
            {
                sSQL += "EXEC SP_BAOCAO_HOADON @TUNGAY=" + cls_Main.SQLString(id) + ", @DENNGAY=" + cls_Main.SQLString(id1) + ",@DIEUKIEN=" + cls_Main.SQLString(id3) + ",@TYPE=" + cls_Main.SQLString("CH") + ", @USERLOGIN=" + cls_Main.SQLString(userID);

            }
            else if (id2 != null && id2.Equals("kho"))
            {
                sSQL += "EXEC SP_BAOCAO_HOADON @TUNGAY=" + cls_Main.SQLString(id) + ", @DENNGAY=" + cls_Main.SQLString(id1) + ",@DIEUKIEN=" + cls_Main.SQLString(id3) + ",@TYPE=" + cls_Main.SQLString("KO") + ", @USERLOGIN=" + cls_Main.SQLString(userID);

            }
            else if (id2 != null && id2.Equals("quay"))
            {
                sSQL += "EXEC SP_BAOCAO_HOADON @TUNGAY=" + cls_Main.SQLString(id) + ", @DENNGAY=" + cls_Main.SQLString(id1) + ",@DIEUKIEN=" + cls_Main.SQLString(id3) + ",@TYPE=" + cls_Main.SQLString("QA") + ", @USERLOGIN=" + cls_Main.SQLString(userID);

            }
            else if (id2 != null && id2.Equals("nhomhang"))
            {
                sSQL += "EXEC SP_BAOCAO_HOADON @TUNGAY=" + cls_Main.SQLString(id) + ", @DENNGAY=" + cls_Main.SQLString(id1) + ",@DIEUKIEN=" + cls_Main.SQLString(id3) + ",@TYPE=" + cls_Main.SQLString("NH") + ", @USERLOGIN=" + cls_Main.SQLString(userID);

            }
            else if (id2 != null && id2.Equals("hanghoa"))
            {
                sSQL += "EXEC SP_BAOCAO_HOADON @TUNGAY=" + cls_Main.SQLString(id) + ", @DENNGAY=" + cls_Main.SQLString(id1) + ",@DIEUKIEN=" + cls_Main.SQLString(id3) + ",@TYPE=" + cls_Main.SQLString("HH") + ", @USERLOGIN=" + cls_Main.SQLString(userID);

            }
            else if (id2 != null && id2.Equals("nhanvien"))
            {
                sSQL += "EXEC SP_BAOCAO_HOADON @TUNGAY=" + cls_Main.SQLString(id) + ", @DENNGAY=" + cls_Main.SQLString(id1) + ",@DIEUKIEN=" + cls_Main.SQLString(id3) + ",@TYPE=" + cls_Main.SQLString("NV") + ", @USERLOGIN=" + cls_Main.SQLString(userID);

            }
            else if (id2 != null && id2.Equals("hdgiamgia"))
            {
                sSQL += "EXEC SP_BAOCAO_HOADON @TUNGAY=" + cls_Main.SQLString(id) + ", @DENNGAY=" + cls_Main.SQLString(id1) + ",@DIEUKIEN=" + cls_Main.SQLString(id3) + ",@TYPE=" + cls_Main.SQLString("GG") + ", @USERLOGIN=" + cls_Main.SQLString(userID);

            }
            else if (id2 != null && id2.Equals("nganhhang"))
            {
                sSQL += "EXEC SP_BAOCAO_HOADON @TUNGAY=" + cls_Main.SQLString(id) + ", @DENNGAY=" + cls_Main.SQLString(id1) + ",@DIEUKIEN=" + cls_Main.SQLString(id3) + ",@TYPE=" + cls_Main.SQLString("NGH") + ", @USERLOGIN=" + cls_Main.SQLString(userID);
            }

            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            int i = 0;
            

            foreach (DataRow dr in dt.Rows)
            {
                DanhSachHoaDon item = new DanhSachHoaDon();
                item.STT = ++i;
                item.MAHOADON = dr["MA_HOADON"].ToString();
                item.NHANVIEN = dr["TenNV"].ToString();
                item.NGAYBAN = Convert.ToDateTime(dr["NGAYTAO"].ToString());
                item.TONGTIEN = decimal.Parse(dr["TONGTIEN"].ToString());
                item.TIENMAT = decimal.Parse(dr["MONEYCASH"].ToString());
                item.TIENVOUCHER = decimal.Parse(dr["MONEYVOUCHER"].ToString());
                item.TIENVISA = decimal.Parse(dr["MONEYVISA"].ToString());
                item.TIENCHUYENKHOAN = decimal.Parse(dr["MONEYCHUYENKHOAN"].ToString());
                item.TIENTHETTNB = decimal.Parse(dr["MONEYINNERCARD"].ToString());
                item.VIMOCA = decimal.Parse(dr["MONEYMOCA"].ToString());
                item.VIVNPAY = decimal.Parse(dr["MONEYVNPAY"].ToString());
                item.VIZALO = decimal.Parse(dr["MONEYZALO"].ToString());
                item.VIMOMO = decimal.Parse(dr["MONEYMOMO"].ToString());
                item.VIVIETTEL = decimal.Parse(dr["MONEYVIETTEL"].ToString());
                item.VISHOPEE = decimal.Parse(dr["MONEYSHOPEEPAY"].ToString());
                item.KHACHHANG = dr["KHACHHANG"].ToString();
                item.GHICHU = dr["GHICHU1"].ToString();
                listHD.Add(item);

                tongTien += item.TONGTIEN;
                tienMat += item.TIENMAT;
                tienVoucher += item.TIENVOUCHER;
                tienVisa += item.TIENVISA;
                tienChuyenKhoan += item.TIENCHUYENKHOAN;
                tienTheTTNB += item.TIENTHETTNB;
                viMOCA += item.VIMOCA;
                viVNPAY += item.VIVNPAY;
                viZALO += item.VIZALO;
                viMOMO += item.VIMOMO;
                viVIETTEL += item.VIVIETTEL;
                viSHOPEE += item.VISHOPEE;
            }

            if (!string.IsNullOrEmpty(id))
            {
                sTungay = id + " - " + id1;
            }
            if (!string.IsNullOrEmpty(id2))
            {
                selectedValue = id2;
            }
            if (!string.IsNullOrEmpty(id2))
            {
                sCombo = id3;
            }

        }

    }
}
public class CUAHANG_CBO
{
    public string MA { get; set; }
    public string TEN { get; set; }
}
public class KHO_CBO
{
    public string MA { get; set; }
    public string TEN { get; set; }
}
public class QUAY_CBO
{
    public string MA { get; set; }
    public string TEN { get; set; }
}
public class NHOMHANG_CBO
{
    public string MA { get; set; }
    public string TEN { get; set; }
}
public class HANGHOA_CBO
{
    public string MA { get; set; }
    public string TEN { get; set; }
}
public class NHANVIEN_CBO
{
    public string MA { get; set; }
    public string TEN { get; set; }
}
public class HOADONGIAMGIA_CBO
{
    public string MA { get; set; }
    public string TEN { get; set; }
}
public class NGANHHANG_CBO
{
    public string MA { get; set; }
    public string TEN { get; set; }
}
public class DATACOMBO_CBO
{
    public string MA { get; set; }
    public string TEN { get; set; }
}
public class DanhSachHoaDon
{
    public int STT { get; set; }
    public string MAHOADON { get; set; }
    public string NHANVIEN { get; set; }
    public DateTime NGAYBAN { get; set; }
    public decimal TONGTIEN { get; set; }
    public decimal TIENMAT { get; set; }
    public decimal TIENVOUCHER { get; set; }
    public decimal TIENVISA { get; set; }
    public decimal TIENCHUYENKHOAN { get; set; }
    public decimal TIENTHETTNB { get; set; }
    public decimal VIMOCA { get; set; }
    public decimal VIVNPAY { get; set; }
    public decimal VIZALO { get; set; }
    public decimal VIMOMO { get; set; }
    public decimal VIVIETTEL { get; set; }
    public decimal VISHOPEE { get; set; }
    public string KHACHHANG { get; set; }
    public string GHICHU { get; set; }
}