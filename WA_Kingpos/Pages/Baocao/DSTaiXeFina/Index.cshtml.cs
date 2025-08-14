using FastReport.Export.PdfSimple;
using FastReport.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeOpenXml;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.Baocao.DSTaiXeFina
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public List<TaiXeModels> listitem = new List<TaiXeModels>();
        public bool bThem { get; set; }
        public bool bSua { get; set; }
        public bool bXoa { get; set; }
        public IActionResult OnGet(string id)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowView("23120901", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                LoadData(id);
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "TaiXeFina Report", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        public IActionResult OnPost(string submitButton, string id)
        {
            try
            {
                if (submitButton == "view")
                {
                    return RedirectToPage("Index", new { id = "1" });
                }
                else if (submitButton == "print")
                {
                    // Khởi tạo báo cáo
                    WebReport webReport = new WebReport();

                    // Load mẫu báo cáo từ file .frx
                    string reportPath = Path.Combine(cls_Main.sFilePathReport, "rptDSTaixe.frx");
                    webReport.Report.Load(reportPath);

                    // Gán Data Source vào báo cáo
                    string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                    string sSQL = "";
                    sSQL += "EXEC SP_FINA_SELECT_DATA @LOAI=16" + "\n";
                    DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                    webReport.Report.RegisterData(dt, "TableMain");

                    // Gán Parameter vào báo cáo
                    webReport.Report.SetParameterValue("tencongty", cls_ConfigCashier.sTENCONGTY);
                    webReport.Report.SetParameterValue("diachi", cls_ConfigCashier.sDIACHICONGTY);
                    webReport.Report.SetParameterValue("dienthoai", cls_ConfigCashier.sSODIENTHOAICONGTY);
                    //Render report
                    webReport.Report.Prepare();

                    // Export báo cáo sang PDF và trả về kết quả
                    MemoryStream stream = new MemoryStream();
                    webReport.Report.Export(new PDFSimpleExport(), stream);
                    stream.Position = 0;
                    return File(stream, "application/pdf", "rptDSTaixe.pdf");
                }
                else if (submitButton == "excel")
                {
                    // Tạo DataTable với dữ liệu của bảng bạn muốn xuất ra Excel
                    string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                    string sSQL = "";
                    sSQL += "EXEC SP_FINA_SELECT_DATA @LOAI=16" + "\n";
                    DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                    DataView view = new System.Data.DataView(dt);
                    DataTable dataTable = view.ToTable("MyDataTable", false, "MA", "TEN", "DIENTHOAI", "CMND", "WEBSITE", "SUDUNG");
                    dataTable.Columns["MA"].ColumnName = "Mã";
                    dataTable.Columns["TEN"].ColumnName = "Tên";
                    dataTable.Columns["DIENTHOAI"].ColumnName = "Điện thoại";
                    dataTable.Columns["CMND"].ColumnName = "CMT/CCCD";
                    dataTable.Columns["WEBSITE"].ColumnName = "Biển số xe";
                    dataTable.Columns["SUDUNG"].ColumnName = "Sử dụng";

                    //Thêm Parameter vào excel
                    string tencongty = cls_ConfigCashier.sTENCONGTY;
                    string diachi = "Địa chỉ : " + cls_ConfigCashier.sDIACHICONGTY;
                    string dienthoai = "Điện thoại : " + cls_ConfigCashier.sSODIENTHOAICONGTY;

                    string tieude = "DANH SÁCH TÀI XẾ";

                    // Tạo một package ExcelPackage từ EPPlus
                    using (ExcelPackage package = new ExcelPackage())
                    {
                        // Tạo một worksheet trong package
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("rptDSTaiXe");
                        worksheet.Cells["A1:I1"].Merge = true;
                        worksheet.Cells["A1"].Style.Font.Bold = true;
                        worksheet.Cells["A1"].Value = tencongty;

                        worksheet.Cells["A2:I2"].Merge = true;
                        worksheet.Cells["A2"].Style.Font.Bold = true;
                        worksheet.Cells["A2"].Value = diachi;

                        worksheet.Cells["A3:I3"].Merge = true;
                        worksheet.Cells["A3"].Style.Font.Bold = true;
                        worksheet.Cells["A3"].Value = dienthoai;

                        worksheet.Cells["A4:F4"].Merge = true;
                        worksheet.Cells["A4"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells["A4"].Style.Font.Bold = true;
                        worksheet.Cells["A4"].Value = tieude;

                        worksheet.Row(5).Style.Font.Bold = true;
                        // Đưa dữ liệu từ DataTable vào worksheet
                        worksheet.Cells["A5"].LoadFromDataTable(dataTable, true);

                        // Tự động điều chỉnh kích thước của tất cả cột dựa trên nội dung
                        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                        // Lưu tệp Excel và trả về kết quả
                        MemoryStream stream = new MemoryStream();
                        package.SaveAs(stream);
                        stream.Position = 0;
                        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "rptDSTaixe.xlsx");
                    }
                }
                LoadData(string.Empty);
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "DSTaiXe Post : ", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        private void LoadData(string id)
        {
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string nguoitao = User.FindFirst("Username")?.Value;
            string manhanvien = User.FindFirst("UserId")?.Value;
            if (id == "1")
            {
                string sSQL = "EXEC SP_FINA_SELECT_DATA @LOAI=16";
                DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                foreach (DataRow dr in dt.Rows)
                {
                    TaiXeModels item = new TaiXeModels();
                    item.MA = dr["MA"].ToString();
                    item.ID = dr["ID"].ToString();
                    item.TEN = dr["TEN"].ToString();
                    item.DIENTHOAI = dr["DIENTHOAI"].ToString();
                    item.CMND = dr["CMND"].ToString();
                    item.WEBSITE = dr["WEBSITE"].ToString();
                    item.MAKHGIOITHIEU = dr["MAKHGIOITHIEU"].ToString();
                    item.SUDUNG = dr["SUDUNG"].ToString();
                    listitem.Add(item);
                }
            }
        }
    }
}
