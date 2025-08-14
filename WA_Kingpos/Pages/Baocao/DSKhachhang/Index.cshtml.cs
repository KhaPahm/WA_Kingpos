using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Reflection;
using WA_Kingpos.Data;
using WA_Kingpos.Models;
using static WA_Kingpos.Pages.TaiKhoan.CreateModel;
using System.IO;
using FastReport.Web;
using FastReport.Export.Image;
using FastReport.Export.PdfSimple;
using FastReport.Data;
using FastReport;
using System.Text.Json;
using OfficeOpenXml;

namespace WA_Kingpos.Pages.DSKhachhang
{
    [Authorize]
    public class IndexModel : PageModel
    {
        [BindProperty]
        public List<DM_NHANVIEN> Listnhanvien { set; get; } = new List<DM_NHANVIEN>();
        public List<KhachHangModels> listitem { set; get; } = new List<KhachHangModels>();
       
        [BindProperty]
        public string sMaNhanVien { get; set; } = "";
        public IActionResult OnGet(string id)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement .AllowView ("23120901", HttpContext.Session .GetString ("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                LoadData(id);
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "DSKhachhang Get", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage",new { id = ex.ToString() });
            }
        }
        public IActionResult OnPost(string submitButton, string id)
        {
            try
            {
                if (submitButton == "view")
                {
                    return RedirectToPage("Index", new { id = sMaNhanVien });
                }
                else if (submitButton == "print")
                {
                    // Khởi tạo báo cáo
                    WebReport webReport = new WebReport();

                    // Load mẫu báo cáo từ file .frx
                    string reportPath = Path.Combine(cls_Main.sFilePathReport, "rptDSKhachhang.frx");
                    webReport.Report.Load(reportPath);

                    // Gán Data Source vào báo cáo
                    string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                    string sSQL = "";
                    sSQL += "EXEC SP_BAOCAO_KHACHANGFINA " + cls_Main.SQLString(id) + "\n";
                    DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                    webReport.Report.RegisterData(dt, "TableMain");

                    // Gán Parameter vào báo cáo
                    webReport.Report.SetParameterValue("tencongty", cls_ConfigCashier .sTENCONGTY);
                    webReport.Report.SetParameterValue("diachi", cls_ConfigCashier.sDIACHICONGTY);
                    webReport.Report.SetParameterValue("dienthoai", cls_ConfigCashier.sSODIENTHOAICONGTY);

                    string tennhanvien = "";
                    DM_NHANVIEN nv = Listnhanvien.Find(item => item.MANHANVIEN == id);
                    if (nv != null)
                    {
                        tennhanvien = nv.TENNHANVIEN;
                    }
                    webReport.Report.SetParameterValue("nhanvien", tennhanvien);

                    //Render report
                    webReport.Report.Prepare();

                    // Export báo cáo sang PDF và trả về kết quả
                    MemoryStream stream = new MemoryStream();
                    webReport.Report.Export(new PDFSimpleExport(), stream);
                    stream.Position = 0;
                    return File(stream, "application/pdf", "rptDSKhachhang.pdf");
                }
                else if (submitButton == "excel")
                {
                    // Tạo DataTable với dữ liệu của bảng bạn muốn xuất ra Excel
                    string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                    string sSQL = "";
                    sSQL += "EXEC SP_BAOCAO_KHACHANGFINA " + cls_Main.SQLString(id) + "\n";
                    DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                    DataView view = new System.Data.DataView(dt);
                    DataTable dataTable =view.ToTable("MyDataTable", false, "MA", "TEN", "DIACHI", "DIENTHOAI", "EMAIL", "FAX", "CMND", "TENNHANVIEN", "SUDUNG" );
                    dataTable.Columns["MA"].ColumnName = "Mã";
                    dataTable.Columns["TEN"].ColumnName = "Tên";
                    dataTable.Columns["DIACHI"].ColumnName = "Địa chỉ";
                    dataTable.Columns["DIENTHOAI"].ColumnName = "Điện thoại";
                    dataTable.Columns["EMAIL"].ColumnName = "Email";
                    dataTable.Columns["FAX"].ColumnName = "Mã số thuế";
                    dataTable.Columns["CMND"].ColumnName = "CMND";
                    dataTable.Columns["TENNHANVIEN"].ColumnName = "Nhân viên";
                    dataTable.Columns["SUDUNG"].ColumnName = "Sử dụng";

                    //Thêm Parameter vào excel
                    string tencongty = cls_ConfigCashier.sTENCONGTY;
                    string diachi ="Địa chỉ : "+ cls_ConfigCashier.sDIACHICONGTY;
                    string dienthoai ="Điện thoại : "+ cls_ConfigCashier.sSODIENTHOAICONGTY;
                    string tieude = "DANH SÁCH KHÁCH HÀNG";
                    string tennhanvien = "";
                    DM_NHANVIEN nv = Listnhanvien.Find(item => item.MANHANVIEN == id);
                    if (nv != null)
                    {
                        tennhanvien = nv.TENNHANVIEN;
                    }

                    // Tạo một package ExcelPackage từ EPPlus
                    using (ExcelPackage package = new ExcelPackage())
                    {
                        // Tạo một worksheet trong package
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("rptDSKhachhang");

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

                        worksheet.Cells["A5"].Style.Font.Bold = true;
                        worksheet.Cells["A5"].Value = "Nhân viên phụ trách:";
                        worksheet.Cells["B5"].Style.Font.Bold = true;
                        worksheet.Cells["B5"].Value = tennhanvien;
                        worksheet.Row(6).Style.Font.Bold = true;

                        // Đưa dữ liệu từ DataTable vào worksheet
                        worksheet.Cells["A6"].LoadFromDataTable(dataTable, true);

                        // Tự động điều chỉnh kích thước của tất cả cột dựa trên nội dung
                        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                        // Lưu tệp Excel và trả về kết quả
                        MemoryStream stream = new MemoryStream();
                        package.SaveAs(stream);
                        stream.Position = 0;
                        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "rptDSKhachhang.xlsx");
                    }
                }
                LoadData(string.Empty);
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "DSKhachhang Post : ", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        private void LoadData(string id)
        {
            //nhanvien
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "";
            sSQL += "Select 0 as MANHANVIEN,N'Tất cả' as TENNHANVIEN From DM_NHANVIEN"+"\n";
            sSQL += "Union" + "\n";
            sSQL += "Select MANHANVIEN,TENNHANVIEN From DM_NHANVIEN where SUDUNG=1"+"\n";
            sSQL += "Order by MANHANVIEN";
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                DM_NHANVIEN Nhanvien = new DM_NHANVIEN();
                Nhanvien.MANHANVIEN = dr["MANHANVIEN"].ToString();
                Nhanvien.TENNHANVIEN = dr["TENNHANVIEN"].ToString();
                Listnhanvien.Add(Nhanvien);
            }
            //manhanvien
            if (!string.IsNullOrEmpty(id))
            {
                sMaNhanVien = id;
            }
            //danh sach khach hang
            sSQL = "";
            sSQL += "EXEC SP_BAOCAO_KHACHANGFINA "+cls_Main .SQLString (id ) + "\n";
            dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                KhachHangModels item = new KhachHangModels();
                item.MA = dr["MA"].ToString();
                item.ID = dr["ID"].ToString();
                item.TEN = dr["TEN"].ToString();
                item.DIACHI = dr["DIACHI"].ToString();
                item.DIENTHOAI = dr["DIENTHOAI"].ToString();
                item.EMAIL = dr["EMAIL"].ToString();
                item.FAX = dr["FAX"].ToString();
                item.CMND = dr["CMND"].ToString();
                item.SUDUNG = dr["SUDUNG"].ToString();
                item.TENKHGIOITHIEU = dr["TENNHANVIEN"].ToString();
                listitem.Add(item);
            }
        }
    }
}
