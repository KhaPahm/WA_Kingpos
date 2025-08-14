using FastReport.Export.PdfSimple;
using FastReport.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeOpenXml;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.Baocao.DSHanghoa
{
    [Authorize]
    public class IndexModel : PageModel
    {
        [BindProperty]
        public List<TONKHO> Listtonkho { set; get; } = new List<TONKHO>();

        [BindProperty]
        public List<KINHDOANH> Listkinhdoanh { set; get; } = new List<KINHDOANH>();

        [BindProperty]
        public List<NGANHHANG> Listnganhhang { set; get; } = new List<NGANHHANG>();

        [BindProperty]
        public List<NHOMHANG> Listnhomhang { set; get; } = new List<NHOMHANG>();

        [BindProperty]
        public List<COMBO> Listcombo { set; get; } = new List<COMBO>();

        public List<clsHanghoa> listitem { set; get; } = new List<clsHanghoa>();

        [BindProperty]
        public string sTonKho { get; set; } = "";
        [BindProperty]
        public string sKinhDoanh { get; set; } = "";
        [BindProperty]
        public string sNganhHang { get; set; } = "";
        [BindProperty]
        public string sNhomHang { get; set; } = "";
        [BindProperty]
        public string sCombo { get; set; } = "";
        public IActionResult OnGet(string id, string id1, string id2, string id3, string id4)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowView("48", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                LoadData(id, id1, id2, id3, id4);
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "DSHangHoa Get", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        public IActionResult OnPost(string submitButton, string id, string id1, string id2, string id3, string id4)
        {
            try
            {
                if (submitButton == "view")
                {
                    return RedirectToPage("Index", new { id = sTonKho, id1 = sKinhDoanh, id2 = sNganhHang, id3 = sNhomHang, id4 = sCombo });
                }
                else if (submitButton == "print")
                {
                    // Khởi tạo báo cáo
                    WebReport webReport = new WebReport();

                    // Load mẫu báo cáo từ file .frx
                    string reportPath = Path.Combine(cls_Main.sFilePathReport, "rptDSHanghoa.frx");
                    webReport.Report.Load(reportPath);

                    // Gán Data Source vào báo cáo
                    string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                    string sSQL = "";
                    sSQL += "EXEC SP_W_BAOCAO_HANGHOA " + cls_Main.SQLString(id) + "," + cls_Main.SQLString(id1) + "," + cls_Main.SQLString(id2) + "," + cls_Main.SQLString(id3) + "," + cls_Main.SQLString(id4) + "\n";
                    DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                    webReport.Report.RegisterData(dt, "TableMain");

                    // Gán Parameter vào báo cáo
                    webReport.Report.SetParameterValue("tencongty", cls_ConfigCashier.sTENCONGTY);
                    webReport.Report.SetParameterValue("diachi", cls_ConfigCashier.sDIACHICONGTY);
                    webReport.Report.SetParameterValue("dienthoai", cls_ConfigCashier.sSODIENTHOAICONGTY);

                    string tonkho = "";
                    string kinhdoanh = "";
                    string nganhhang = "";
                    string nhomhang = "";
                    string combo = "";
                    TONKHO tk = Listtonkho.Find(item => item.MA == id);
                    if (tk != null)
                    {
                        tonkho = tk.TEN;
                    }
                    KINHDOANH kd = Listkinhdoanh.Find(item => item.MA == id1);
                    if (kd != null)
                    {
                        kinhdoanh = kd.TEN;
                    }
                    NGANHHANG ngh = Listnganhhang.Find(item => item.MA == id2);
                    if (ngh != null)
                    {
                        nganhhang = ngh.TEN;
                    }
                    NHOMHANG nh = Listnhomhang.Find(item => item.MA == id3);
                    if (nh != null)
                    {
                        nhomhang = nh.TEN;
                    }
                    COMBO cb = Listcombo.Find(item => item.MA == id4);
                    if (cb != null)
                    {
                        combo = cb.TEN;
                    }
                    webReport.Report.SetParameterValue("tonkho", tonkho);
                    webReport.Report.SetParameterValue("kinhdoanh", kinhdoanh);
                    webReport.Report.SetParameterValue("nganhhang", nganhhang);
                    webReport.Report.SetParameterValue("nhomhang", nhomhang);
                    webReport.Report.SetParameterValue("combo", combo);
                    //Render report
                    webReport.Report.Prepare();

                    // Export báo cáo sang PDF và trả về kết quả
                    MemoryStream stream = new MemoryStream();
                    webReport.Report.Export(new PDFSimpleExport(), stream);
                    stream.Position = 0;
                    return File(stream, "application/pdf", "rptDSHanghoa.pdf");
                }
                else if (submitButton == "excel")
                {
                    // Tạo DataTable với dữ liệu của bảng bạn muốn xuất ra Excel
                    string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                    string sSQL = "";
                    sSQL += "EXEC SP_W_BAOCAO_HANGHOA " + cls_Main.SQLString(id) + "," + cls_Main.SQLString(id1) + "," + cls_Main.SQLString(id2) + "," + cls_Main.SQLString(id3) + "," + cls_Main.SQLString(id4) + "\n";
                    DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                    DataView view = new System.Data.DataView(dt);
                    DataTable dataTable = view.ToTable("MyDataTable", false, "MA_HANGHOA", "ID_HANGHOA", "TEN_HANGHOA", "TEN_NHOMHANG", "TEN_DONVITINH", "THUE", "GIANHAP", "GIABAN1", "GIABAN2", "TONKHO", "THUCDON");
                    dataTable.Columns["MA_HANGHOA"].ColumnName = "Mã hàng hóa";
                    dataTable.Columns["ID_HANGHOA"].ColumnName = "Mã nội bộ";
                    dataTable.Columns["TEN_HANGHOA"].ColumnName = "Tên hàng hóa";
                    dataTable.Columns["TEN_DONVITINH"].ColumnName = "ĐVT";
                    dataTable.Columns["THUE"].ColumnName = "Thuế";
                    dataTable.Columns["GIANHAP"].ColumnName = "Giá nhập";
                    dataTable.Columns["GIABAN1"].ColumnName = "Giá bán 1";
                    dataTable.Columns["GIABAN2"].ColumnName = "Giá bán 2";
                    dataTable.Columns["TONKHO"].ColumnName = "Tồn kho";
                    dataTable.Columns["THUCDON"].ColumnName = "Thực đơn";
                    dataTable.Columns["TEN_NHOMHANG"].ColumnName = "Nhóm";

                    //Thêm Parameter vào excel
                    string tencongty = cls_ConfigCashier.sTENCONGTY;
                    string diachi = "Địa chỉ : " + cls_ConfigCashier.sDIACHICONGTY;
                    string dienthoai = "Điện thoại : " + cls_ConfigCashier.sSODIENTHOAICONGTY;

                    string tieude = "DANH SÁCH HÀNG HÓA";
                    string tonkho = "";
                    string kinhdoanh = "";
                    string nganhhang = "";
                    string nhomhang = "";
                    string combo = "";
                    TONKHO tk = Listtonkho.Find(item => item.MA == id);
                    if (tk != null)
                    {
                        tonkho = tk.TEN;
                    }
                    KINHDOANH kd = Listkinhdoanh.Find(item => item.MA == id1);
                    if (kd != null)
                    {
                        kinhdoanh = kd.TEN;
                    }
                    NGANHHANG ngh = Listnganhhang.Find(item => item.MA == id2);
                    if (ngh != null)
                    {
                        nganhhang = ngh.TEN;
                    }
                    NHOMHANG nh = Listnhomhang.Find(item => item.MA == id3);
                    if (nh != null)
                    {
                        nhomhang = nh.TEN;
                    }
                    COMBO cb = Listcombo.Find(item => item.MA == id4);
                    if (cb != null)
                    {
                        combo = cb.TEN;
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

                        worksheet.Cells["A4:L4"].Merge = true;
                        worksheet.Cells["A4"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells["A4"].Style.Font.Bold = true;
                        worksheet.Cells["A4"].Value = tieude;
                        //
                        worksheet.Cells["A5"].Style.Font.Bold = true;
                        worksheet.Cells["A5"].Value = "Tồn kho:";
                        worksheet.Cells["B5"].Style.Font.Bold = true;
                        worksheet.Cells["B5"].Value = tonkho;
                        //
                        worksheet.Cells["A6"].Style.Font.Bold = true;
                        worksheet.Cells["A6"].Value = "Kinh doanh:";
                        worksheet.Cells["B6"].Style.Font.Bold = true;
                        worksheet.Cells["B6"].Value = kinhdoanh;
                        //
                        worksheet.Cells["A7"].Style.Font.Bold = true;
                        worksheet.Cells["A7"].Value = "Ngành hàng:";
                        worksheet.Cells["B7"].Style.Font.Bold = true;
                        worksheet.Cells["B7"].Value = nganhhang;
                        //
                        worksheet.Cells["A8"].Style.Font.Bold = true;
                        worksheet.Cells["A8"].Value = "Nhóm hàng:";
                        worksheet.Cells["B8"].Style.Font.Bold = true;
                        worksheet.Cells["B8"].Value = nhomhang;
                        //
                        worksheet.Cells["A9"].Style.Font.Bold = true;
                        worksheet.Cells["A9"].Value = "Combo:";
                        worksheet.Cells["B9"].Style.Font.Bold = true;
                        worksheet.Cells["B9"].Value = combo;

                        worksheet.Row(10).Style.Font.Bold = true;
                        // Đưa dữ liệu từ DataTable vào worksheet
                        worksheet.Cells["A10"].LoadFromDataTable(dataTable, true);

                        // Tự động điều chỉnh kích thước của tất cả cột dựa trên nội dung
                        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                        // Lưu tệp Excel và trả về kết quả
                        MemoryStream stream = new MemoryStream();
                        package.SaveAs(stream);
                        stream.Position = 0;
                        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "rptDSHanghoa.xlsx");
                    }
                }
                LoadData(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "DSHangHoa Post : ", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        private void LoadData(string id, string id1, string id2, string id3, string id4)
        {
            //tonkho
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "";
            DataTable dt = new DataTable();
            dt.Columns.Add("MA", typeof(int));
            dt.Columns.Add("TEN", typeof(string));
            dt.Rows.Add("0", "Tất cả");
            dt.Rows.Add("1", "Hàng tính tồn kho");
            dt.Rows.Add("2", "Hàng không tồn kho");
            foreach (DataRow dr in dt.Rows)
            {
                TONKHO Tonkho = new TONKHO();
                Tonkho.MA = dr["MA"].ToString();
                Tonkho.TEN = dr["TEN"].ToString();
                Listtonkho.Add(Tonkho);
            }

            //kinh doanh
            dt.Rows.Clear();
            dt.Rows.Add("0", "Tất cả");
            dt.Rows.Add("1", "Hàng kinh doanh");
            dt.Rows.Add("2", "Hàng không kinh doanh");
            foreach (DataRow dr in dt.Rows)
            {
                KINHDOANH Kinhdoanh = new KINHDOANH();
                Kinhdoanh.MA = dr["MA"].ToString();
                Kinhdoanh.TEN = dr["TEN"].ToString();
                Listkinhdoanh.Add(Kinhdoanh);
            }

            //Nganhhang
            sSQL = "select 0 as MA, N'Tất cả' as TEN" + "\n";
            sSQL += "union" + "\n";
            sSQL += "select OrderBy AS MA, VNValue AS TEN from ReferenceList where Category='ProductType' and Status=1 order by MA";
            DataTable dtNganhHang = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dtNganhHang.Rows)
            {
                NGANHHANG Nganhhang = new NGANHHANG();
                Nganhhang.MA = dr["MA"].ToString();
                Nganhhang.TEN = dr["TEN"].ToString();
                Listnganhhang.Add(Nganhhang);
            }

            //Nhomhang           
            sSQL = "select 0 as MA, N'Tất cả' as TEN" + "\n";
            sSQL += "union" + "\n";
            sSQL += "SELECT MA_NHOMHANG AS MA,TEN_NHOMHANG AS TEN FROM NHOMHANG WHERE SUDUNG = 1 ORDER BY MA";
            DataTable dtNhomHang = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dtNhomHang.Rows)
            {
                NHOMHANG Nhomhang = new NHOMHANG();
                Nhomhang.MA = dr["MA"].ToString();
                Nhomhang.TEN = dr["TEN"].ToString();
                Listnhomhang.Add(Nhomhang);
            }

            //Combo
            dt.Rows.Clear();
            dt.Rows.Add("0", "Tất cả");
            dt.Rows.Add("1", "Hàng hóa combo");
            dt.Rows.Add("2", "Hàng hóa bình thường");
            foreach (DataRow dr in dt.Rows)
            {
                COMBO Combo = new COMBO();
                Combo.MA = dr["MA"].ToString();
                Combo.TEN = dr["TEN"].ToString();
                Listcombo.Add(Combo);
            }
            //

            //danh sach hang hoa
            sSQL = "";
            sSQL += "EXEC SP_W_BAOCAO_HANGHOA " + cls_Main.SQLString(id) + "," + cls_Main.SQLString(id1) + "," + cls_Main.SQLString(id2) + "," + cls_Main.SQLString(id3) + "," + cls_Main.SQLString(id4) + "\n";
            dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                clsHanghoa item = new clsHanghoa();
                item.MA = dr["MA_HANGHOA"].ToString();
                item.ID_HANGHOA = dr["ID_HANGHOA"].ToString();
                item.TEN = dr["TEN_HANGHOA"].ToString();
                item.TEN_DONVITINH = dr["TEN_DONVITINH"].ToString();
                item.THUE = dr["THUE"].ToString() + "%";
                item.GIANHAP = decimal.Parse(dr["GIANHAP"].ToString()).ToString("N0");
                item.GIABAN1 = decimal.Parse(dr["GIABAN1"].ToString()).ToString("N0");
                item.GIABAN2 = decimal.Parse(dr["GIABAN2"].ToString()).ToString("N0");
                item.TONKHO = dr["TONKHO"].ToString();
                item.THUCDON = dr["THUCDON"].ToString();
                item.TEN_NHOMHANG = dr["TEN_NHOMHANG"].ToString();
                listitem.Add(item);
            }
            if (!string.IsNullOrEmpty(id))
            {
                sTonKho = id;
            }
            if (!string.IsNullOrEmpty(id1))
            {
                sKinhDoanh = id1;
            }
            if (!string.IsNullOrEmpty(id2))
            {
                sNganhHang = id2;
            }
            if (!string.IsNullOrEmpty(id3))
            {
                sNhomHang = id3;
            }
            if (!string.IsNullOrEmpty(id4))
            {
                sCombo = id4;
            }
        }
        public class TONKHO
        {
            public string MA { get; set; }
            public string TEN { get; set; }
        }
        public class KINHDOANH
        {
            public string MA { get; set; }
            public string TEN { get; set; }
        }
        public class NGANHHANG
        {
            public string MA { get; set; }
            public string TEN { get; set; }
        }
        public class NHOMHANG
        {
            public string MA { get; set; }
            public string TEN { get; set; }
        }
        public class COMBO
        {
            public string MA { get; set; }
            public string TEN { get; set; }
        }

    }
}

