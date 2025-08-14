using FastReport.Export.PdfSimple;
using FastReport.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeOpenXml;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Reflection;
using static WA_Kingpos.Pages.TaiKhoan.CreateModel;
using System.IO;
using FastReport.Export.Image;
using FastReport.Data;
using FastReport;
using System.Text.Json;
using System.Globalization;
using System.ComponentModel.DataAnnotations;
using System.Windows.Forms;

namespace WA_Kingpos.Pages.Baocao.SoLuongThucTeQC
{
    [Authorize]
    public class IndexModel : PageModel
    {
        [BindProperty]
        public DateTime SelectedDate { get; set; } = DateTime.Now;

        public List<Item> listitem = new List<Item>();

        public List<Item> listitem_tuyen = new List<Item>();

        public List<View> listitem_view = new List<View>();

        [BindProperty]
        [Required(ErrorMessage = "Phải chọn tuyến khai báo")]
        public int MATUYEN { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Phải chọn chuyến tàu khai báo")]
        public int MALICHTRINH { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Phải chọn ngay khai báo")]
        public string? NGAY { get; set; }

        public IActionResult OnGet(string id, string day, string malichtrinh)
        {
            try
            {
                //check quyen
                /*if (!cls_UserManagement.AllowView("23120901", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }*/
                //xu ly trang
                LoadData(id, day, malichtrinh);
                if (string.IsNullOrEmpty(day))
                {
                    NGAY = DateTime.Now.ToString("dd/MM/yyyy");
                }
                else
                {
                    NGAY = day; 
                }
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "SoLuongThucTeQC Get", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        public IActionResult OnPost(string submitButton, string id)
        {
            try
            {
                if (string.IsNullOrEmpty(submitButton))
                {
                    submitButton = "btn1ReLoad";
                }
                if (submitButton == "btn1ReLoad")
                {
                    return RedirectToPage("Index", new { id = MATUYEN, day = NGAY, malichtrinh = "" });
                }
                else if (submitButton == "view")
                {
                    return RedirectToPage("Index", new { id = MATUYEN, day = NGAY, malichtrinh = MALICHTRINH });
                }
                else if (submitButton == "print")
                {
                    // Khởi tạo báo cáo
                    WebReport webReport = new WebReport();

                    // Load mẫu báo cáo từ file .frx
                    string reportPath = Path.Combine(cls_Main.sFilePathReport, "rptSoLuongThucTeQC.frx");
                    webReport.Report.Load(reportPath);

                    // Gán Data Source vào báo cáo
                    string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                    string sSQL = "";
                    sSQL += "EXEC SP_SOLUONGTHUCTE " + cls_Main.SQLString(MALICHTRINH.ToString()) + "\n";
                    DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                    webReport.Report.RegisterData(dt, "TableMain");

                    // Gán Parameter vào báo cáo
                    webReport.Report.SetParameterValue("tencongty", cls_ConfigCashier.sTENCONGTY);
                    webReport.Report.SetParameterValue("diachi", cls_ConfigCashier.sDIACHICONGTY);
                    webReport.Report.SetParameterValue("dienthoai", cls_ConfigCashier.sSODIENTHOAICONGTY);
                    string dtime;
                    if (string.IsNullOrEmpty(NGAY))
                    {
                        dtime = DateTime.Now.ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        dtime = NGAY;
                    }
                    sSQL = "select l.MALICHTRINH,  CONVERT(varchar(20),l.NGAYDI,103) NGAYDI, l.GIODI, t.TENTUYEN, tt.TENDOITUONG from LICHTRINH l, DM_TUYEN t, DM_DOITUONG tt where l.MATUYEN = t.MATUYEN and tt.MADOITUONG = l.MADOITUONG AND l.MALICHTRINH = " + cls_Main.SQLString(MALICHTRINH.ToString());
                    DataTable dtb = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                    if (dtb.Rows.Count > 0)
                    {
                        webReport.Report.SetParameterValue("tuyen", dtb.Rows[0]["TENTUYEN"].ToString());
                        webReport.Report.SetParameterValue("ngay", dtb.Rows[0]["NGAYDI"].ToString());
                        webReport.Report.SetParameterValue("gio", dtb.Rows[0]["GIODI"].ToString());
                        webReport.Report.SetParameterValue("tau", dtb.Rows[0]["TENDOITUONG"].ToString());
                    }  

                        
                    webReport.Report.SetParameterValue("nhanvien", dt.Rows[0]["TENNHANVIEN"].ToString());



                    //Render report
                    webReport.Report.Prepare();

                    // Export báo cáo sang PDF và trả về kết quả
                    MemoryStream stream = new MemoryStream();
                    webReport.Report.Export(new PDFSimpleExport(), stream);
                    stream.Position = 0;
                    return File(stream, "application/pdf", "rptSoLuongThucTeQC.pdf");
                }
                else if (submitButton == "excel")
                {
                    // Tạo DataTable với dữ liệu của bảng bạn muốn xuất ra Excel
                    string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                    string sSQL = "";
                    sSQL += "EXEC SP_SOLUONGTHUCTE " + cls_Main.SQLString(MALICHTRINH.ToString()) + "\n";
                    DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                    DataView view = new System.Data.DataView(dt);
                    DataTable dataTable = view.ToTable("MyDataTable", false, "TREEM", "CAOTUOI", "NGUOILON", "VEMOI", "GHICHU", "TONGHANHKHACH", "GANMAY", "SLOTO4C", "SLOTO7C", "SLOTO16C", "SLOTO25C", "SLOTO50C", "SLXETAI");
                    dataTable.Columns["TREEM"].ColumnName = "Trẻ em";
                    dataTable.Columns["CAOTUOI"].ColumnName = "Cao tuổi";
                    dataTable.Columns["NGUOILON"].ColumnName = "Người lớn";
                    dataTable.Columns["VEMOI"].ColumnName = "Vé mời";
                    dataTable.Columns["GHICHU"].ColumnName = "Ghi chú";
                    dataTable.Columns["TONGHANHKHACH"].ColumnName = "Tổng hành khách";
                    dataTable.Columns["GANMAY"].ColumnName = "GM + MOTO";
                    dataTable.Columns["SLOTO4C"].ColumnName = "4-5 chỗ";
                    dataTable.Columns["SLOTO7C"].ColumnName = "5-7 chỗ";
                    dataTable.Columns["SLOTO16C"].ColumnName = "10-16 chỗ";
                    dataTable.Columns["SLOTO25C"].ColumnName = "17-25 chỗ";
                    dataTable.Columns["SLOTO50C"].ColumnName = "26-50 chỗ";
                    dataTable.Columns["SLXETAI"].ColumnName = "Xe tải";

                    //Thêm Parameter vào excel
                    string tencongty = cls_ConfigCashier.sTENCONGTY;
                    string diachi = "Địa chỉ : " + cls_ConfigCashier.sDIACHICONGTY;
                    string dienthoai = "Điện thoại : " + cls_ConfigCashier.sSODIENTHOAICONGTY;
                    string tieude = "BÁO CÁO SỐ LƯỢNG THỰC TẾ TRÊN TÀU";

                    // Tạo một package ExcelPackage từ EPPlus
                    using (ExcelPackage package = new ExcelPackage())
                    {
                        // Tạo một worksheet trong package
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("rptDSKhachhang");

                        worksheet.Cells["A1:M1"].Merge = true;
                        worksheet.Cells["A1"].Style.Font.Bold = true;
                        worksheet.Cells["A1"].Value = tencongty;

                        worksheet.Cells["A2:M2"].Merge = true;
                        worksheet.Cells["A2"].Style.Font.Bold = true;
                        worksheet.Cells["A2"].Value = diachi;

                        worksheet.Cells["A3:M3"].Merge = true;
                        worksheet.Cells["A3"].Style.Font.Bold = true;
                        worksheet.Cells["A3"].Value = dienthoai;

                        worksheet.Cells["A4:M4"].Merge = true;
                        worksheet.Cells["A4"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells["A4"].Style.Font.Bold = true;
                        worksheet.Cells["A4"].Value = tieude;

                        string dtime;
                        if (string.IsNullOrEmpty(NGAY))
                        {
                            dtime = DateTime.Now.ToString("dd/MM/yyyy");
                        }
                        else
                        {
                            dtime = NGAY;
                        }
                        sSQL = "select l.MALICHTRINH, CONVERT(varchar(20),l.NGAYDI,103) NGAYDI, l.GIODI, t.TENTUYEN, tt.TENDOITUONG from LICHTRINH l, DM_TUYEN t, DM_DOITUONG tt where l.MATUYEN = t.MATUYEN and tt.MADOITUONG = l.MADOITUONG AND l.MALICHTRINH = " + cls_Main.SQLString(MALICHTRINH.ToString());
                        DataTable dtb = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                        if (dtb.Rows.Count > 0)
                        {
                            worksheet.Cells["A5"].Style.Font.Bold = true;
                            worksheet.Cells["A5"].Value = "Tuyến:";
                            worksheet.Cells["B5"].Style.Font.Bold = true;
                            worksheet.Cells["B5"].Value = dtb.Rows[0]["TENTUYEN"].ToString();

                            worksheet.Cells["A6"].Style.Font.Bold = true;
                            worksheet.Cells["A6"].Value = "Tàu:";
                            worksheet.Cells["B6"].Style.Font.Bold = true;
                            worksheet.Cells["B6"].Value = dtb.Rows[0]["TENDOITUONG"].ToString();

                            worksheet.Cells["A7"].Style.Font.Bold = true;
                            worksheet.Cells["A7"].Value = "Ngày:";
                            worksheet.Cells["B7"].Style.Font.Bold = true;
                            worksheet.Cells["B7"].Value = dtb.Rows[0]["NGAYDI"].ToString();
                            worksheet.Cells["C7"].Style.Font.Bold = true;
                            worksheet.Cells["C7"].Value = "Giờ:";
                            worksheet.Cells["D7"].Style.Font.Bold = true;
                            worksheet.Cells["D7"].Value = dtb.Rows[0]["GIODI"].ToString();


                        }
                        worksheet.Cells["A8"].Style.Font.Bold = true;
                        worksheet.Cells["A8"].Value = "Nhân viên:";
                        worksheet.Cells["B8"].Style.Font.Bold = true;
                        worksheet.Cells["B8"].Value = dt.Rows[0]["TENNHANVIEN"].ToString();

                        worksheet.Row(9).Style.Font.Bold = true;

                        // Đưa dữ liệu từ DataTable vào worksheet
                        worksheet.Cells["A9"].LoadFromDataTable(dataTable, true);

                        // Tự động điều chỉnh kích thước của tất cả cột dựa trên nội dung
                        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                        // Lưu tệp Excel và trả về kết quả
                        MemoryStream stream = new MemoryStream();
                        package.SaveAs(stream);
                        stream.Position = 0;
                        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "rptBCSoLuongThucTe.xlsx");
                    }
                }
                LoadData(string.Empty, string.Empty, string.Empty);
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "SoLuongThucTeQC Post : ", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        private void LoadData(string id, string day, string malichtrinh)
        {
            if (string.IsNullOrEmpty(id))
            {
                id = "0";
            }
            if (string.IsNullOrEmpty(malichtrinh))
            {
                malichtrinh = "0";
            }
            //lichtrinh

            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string dtime = "";
            if(string.IsNullOrEmpty(day))
            {
                dtime = DateTime.Now.ToString("dd/MM/yyyy");
            }    
            else
            {
                dtime = day;
            }    
            string sSQL = "Exec SP_GetLichTauTheoNgay_SL " + cls_Main.SQLString(dtime) + "," + cls_Main.SQLString("0");
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
            //tuyen
            if (dt.Rows.Count > 0)
            {
                DataView view = new DataView(dt);
                DataTable dt_tuyen = view.ToTable("dt_tuyen", true, "ID_Trip", "Name_Trip");
                foreach (DataRow dr in dt_tuyen.Rows)
                {
                    Item item = new Item();
                    item.ID_Trip = dr["ID_Trip"].ToString();
                    item.Name_Trip = dr["Name_Trip"].ToString();
                    listitem_tuyen.Add(item);
                }
            }

            //matuyen
            if (!string.IsNullOrEmpty(id))
            {
                MATUYEN = int.Parse(id);
            }
            if (!string.IsNullOrEmpty(malichtrinh))
            {
                MALICHTRINH = int.Parse(malichtrinh);
            }

            //danh sach khach hang
            sSQL = "";
            sSQL += "EXEC SP_SOLUONGTHUCTE " + cls_Main.SQLString(MALICHTRINH.ToString()) + "\n";
            dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                View item = new View();
                item.TREEM = dr["TREEM"].ToString();
                item.CAOTUOI = dr["CAOTUOI"].ToString();
                item.VIP = dr["VIP"].ToString();
                item.NGUOILON = dr["NGUOILON"].ToString();
                item.VEMOI = dr["VEMOI"].ToString();
                item.TONGHANHKHACH = dr["TONGHANHKHACH"].ToString();
                item.GHICHU = dr["GHICHU"].ToString();
                item.GANMAY = dr["GANMAY"].ToString();
                item.SLOTO4C = dr["SLOTO4C"].ToString();
                item.SLOTO7C = dr["SLOTO7C"].ToString();
                item.SLOTO16C = dr["SLOTO16C"].ToString();
                item.SLOTO25C = dr["SLOTO25C"].ToString();
                item.SLOTO50C = dr["SLOTO50C"].ToString();
                item.SLXETAI = dr["SLXETAI"].ToString();
                listitem_view.Add(item);
            }
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
        }
        public class View
        {
            public string? TREEM { get; set; }
            public string? CAOTUOI { get; set; }
            public string? VIP { get; set; }
            public string? NGUOILON { get; set; }
            public string? VEMOI { get; set; }
            public string? TONGHANHKHACH { get; set; }
            public string? GHICHU { get; set; }
            public string? GANMAY { get; set; }
            public string? SLOTO4C { get; set; }
            public string? SLOTO7C { get; set; }
            public string? SLOTO16C { get; set; }
            public string? SLOTO25C { get; set; }
            public string? SLOTO50C { get; set; }
            public string? SLXETAI { get; set; }
        }
    }
}
