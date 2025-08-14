using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using WA_Kingpos.Data;
using WA_Kingpos.Models;
using System.IO;
using DocumentFormat.OpenXml.Spreadsheet;

namespace WA_Kingpos.Pages.Nhanvien
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public List<clsNhanvien> listitem = new List<clsNhanvien>();
        public List<DM_CHUCVU> ListChucvu { set; get; } = new List<DM_CHUCVU>();
        public bool bThem { get; set; }
        public bool bSua { get; set; }
        public bool bXoa { get; set; }
        public IActionResult OnGet()
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowView("11", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                else
                {
                    bThem = cls_UserManagement.AllowAdd("11", HttpContext.Session.GetString("Permission"));
                    bSua = cls_UserManagement.AllowEdit("11", HttpContext.Session.GetString("Permission"));
                    bXoa = cls_UserManagement.AllowDelete("11", HttpContext.Session.GetString("Permission"));
                }
                //xu ly trang
               
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string sSQL = "EXEC SP_W_GETNHANVIEN";
                DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                foreach (DataRow dr in dt.Rows)
                {
                    clsNhanvien item = new clsNhanvien();
                    item.manhanvien = dr["MANHANVIEN"].ToString();
                    item.tennhanvien = dr["TENNHANVIEN"].ToString();
                    item.ngaysinh = dr["NGAYSINH"].ToString();
                    item.gioitinh = dr["GIOITINH"].ToString();
                    item.diachi = dr["DIACHI"].ToString();
                    item.dienthoai = dr["DIENTHOAI"].ToString();
                    item.email = dr["EMAIL"].ToString();
                    item.phongban = dr["TENCHUCVU"].ToString();
                    item.ghichu = dr["GHICHU"].ToString();
                    item.sudung = dr["SUDUNG"].ToString();

                    item.NGAYNHANVIEC = dr["NGAYNHANVIEC"]?.ToStrDate();
                    //item.NGUOIQUANLY = dr["NGUOIQUANLY"]?.ChangeType<int?>();
                    //item.TEN_NGUOIQUANLY = dr["TEN_NGUOIQUANLY"]?.ChangeType<string?>();
                    item.TRANGTHAI = dr["TRANGTHAI"]?.ChangeType<int?>();

                    listitem.Add(item);
                }
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "DM_NHANVIEN", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        public async Task<IActionResult> OnPostImportAsync(IFormFile ExcelFile)
        {
            if (ExcelFile == null || ExcelFile.Length == 0)
            {
                ModelState.AddModelError("", "Vui lòng chọn file Excel.");
                return Page();
            }
            LoadData();
            using var stream = new MemoryStream();
            await ExcelFile.CopyToAsync(stream);

            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheets.First();
            var firstRow = worksheet.FirstRowUsed().RowNumber() + 3; // Bỏ dòng tiêu đề, v.v.

            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            int successCount = 0;
            int errorCount = 0;
            var logPath = @"C:ImportExcell\Log\ImportErrorLog.txt";

            // Đảm bảo thư mục tồn tại
            Directory.CreateDirectory(Path.GetDirectoryName(logPath));

            for (int row = firstRow + 1; row <= worksheet.LastRowUsed().RowNumber(); row++)
            {
                try
                {
                    var wsRow = worksheet.Row(row);
                    var tennhanvien = wsRow.Cell(3).GetString();

                    if (string.IsNullOrWhiteSpace(tennhanvien))
                        continue;

                    var phongban = wsRow.Cell(4).GetString();
                    //var ngaysinh = DateTime.ParseExact(wsRow.Cell(8).GetString(), "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                    var cellValueNS = wsRow.Cell(8);
                    DateTime ngaysinh;
                    if (cellValueNS.DataType == XLDataType.DateTime)
                    {
                        ngaysinh = cellValueNS.GetDateTime();
                    }
                    else
                    {
                        // Nếu không phải kiểu ngày thì cố gắng parse thủ công
                        DateTime.TryParseExact(cellValueNS.GetString().Trim(),
                            "dd.MM.yyyy",
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.None,
                            out ngaysinh);
                    }
                    var gioitinh = wsRow.Cell(9).GetString().ToLower() == "nam";
                    var dienthoai = wsRow.Cell(10).GetString();
                    var email = wsRow.Cell(16).GetString();
                    var ghichu = wsRow.Cell(23).GetString();
                    var diachi = wsRow.Cell(21).GetString();
                    var sudung = wsRow.Cell(25).GetString().Trim().ToLower();
                    bool sudungBool = sudung == "có";
                    //var ngaynhanviec = DateTime.ParseExact(wsRow.Cell(5).GetString(), "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                    var cellValueNV = wsRow.Cell(5);
                    DateTime ngaynhanviec;
                    if (cellValueNV.DataType == XLDataType.DateTime)
                    {
                        ngaynhanviec = cellValueNV.GetDateTime();
                    }
                    else
                    {
                        // Nếu không phải kiểu ngày thì cố gắng parse thủ công
                        DateTime.TryParseExact(cellValueNV.GetString().Trim(),
                            "dd.MM.yyyy",
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.None,
                            out ngaynhanviec);
                    }

                    var trangThai = wsRow.Cell(24).GetString().Trim().ToLower() switch
                    {
                        "thử việc" => "0",
                        "chính thức" => "1",
                        _ => "2"
                    };

                    var machucvu = ListChucvu
                                    .FirstOrDefault(x => string.Equals(x.TENCHUCVU.Trim(), phongban.Trim(), StringComparison.OrdinalIgnoreCase))
                                    ?.MACHUCVU ?? "0";                              

                    // Gọi stored procedure
                    string sSQL = "";
                    sSQL += "EXEC SP_W_NHANVIEN ";
                    sSQL += cls_Main.SQLStringUnicode(tennhanvien) + ",";
                    //sSQL += ExtensionObject.toSqlParDateTime(ngaysinh) + ",";
                    sSQL += cls_Main.SQLStringUnicode(ngaysinh.ToString("yyyy-MM-dd")) + ",";
                    //sSQL += cls_Main.SQLStringUnicode(ngaysinh) + ",";
                    sSQL += cls_Main.SQLBit(gioitinh) + ",";
                    sSQL += cls_Main.SQLStringUnicode(diachi) + ",";
                    sSQL += cls_Main.SQLStringUnicode(dienthoai) + ",";
                    sSQL += cls_Main.SQLStringUnicode(email) + ",";
                    sSQL += cls_Main.SQLStringUnicode(ghichu) + ",";
                    sSQL += cls_Main.SQLString(machucvu) + ",";
                    sSQL += cls_Main.SQLBit(sudungBool) + ",";
                    //sSQL += $"0, {ExtensionObject.toSqlPar(trangThai)}, {ExtensionObject.toSqlParDateTime(ngaynhanviec)}";
                    sSQL += $"0, {ExtensionObject.toSqlPar(trangThai)}, {cls_Main.SQLStringUnicode(ngaynhanviec.ToString("yyyy-MM-dd"))}";

                    bool bRunSQL = cls_Main.ExecuteSQL(sSQL, sConnectionString_live);

                    if (bRunSQL)
                        successCount++;
                    else
                    {
                        errorCount++;
                        System.IO.File.AppendAllText(logPath, $"Lỗi tại dòng {row}: Không thể thực thi SQL.\n");
                    }
                }
                catch (Exception ex)
                {
                    errorCount++;
                    System.IO.File.AppendAllText(logPath, $"Lỗi tại dòng {row}: {ex.Message}\n");
                }
            }

            // Thông báo
            TempData["Message"] = $"Import hoàn tất: {successCount} dòng thành công, {errorCount} dòng lỗi.";

            

            return Page();
        }
        
        private void LoadData()
        {
            //chuc vu
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "Select * From DM_CHUCVU";
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                DM_CHUCVU Chucvu = new DM_CHUCVU();
                Chucvu.MACHUCVU = dr["MACHUCVU"].ToString();
                Chucvu.TENCHUCVU = dr["TENCHUCVU"].ToString();
                ListChucvu.Add(Chucvu);
            }
        }
        public class DM_CHUCVU
        {
            public string MACHUCVU { get; set; }
            public string TENCHUCVU { get; set; }
        }       

    }
}
