using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeOpenXml;
using System.Data;
using System.Diagnostics;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.Hanghoa
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public List<clsHanghoa> listitem = new List<clsHanghoa>();
        public bool bThem { get; set; }
        public bool bSua { get; set; }
        public bool bXoa { get; set; }

        [BindProperty]
        public IFormFile? UploadedFile { get; set; }

        public IActionResult OnGet()
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowView("6", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                else
                {
                    bThem = cls_UserManagement.AllowAdd("6", HttpContext.Session.GetString("Permission"));
                    bSua = cls_UserManagement.AllowEdit("6", HttpContext.Session.GetString("Permission"));
                    bXoa = cls_UserManagement.AllowDelete("6", HttpContext.Session.GetString("Permission"));
                }
                //xu ly trang
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string sSQL = "";
                sSQL += "SELECT MA_HANGHOA,ID_HANGHOA,TEN_HANGHOA,H.STT,H.GHICHU,H.MAVACH,TEN_DONVITINH,H.MA_DONVITINH,";
                sSQL += "H.SUDUNG,H.SUAGIA,H.HINHANH,H.MA_NHOMHANG,TEN_NHOMHANG,THUE,GIANHAP,GIABAN1,GIABAN2,GIABAN3,GIABAN4,TONKHO,H.IS_INBEP,";
                sSQL += "THUCDON,TONTOITHIEU,H.SOLUONG,H.MA_BEP,B.TEN_BEP,H.MA_THANHPHAN,ISNULL(SUADINHLUONG,0) AS SUADINHLUONG,H.GIATHEOTRONGLUONG,H.PLU, H.HANSUDUNG, H.MONTHEM,H.INTEM,kk.TEN_KHO AS NOIXUAT " + "\n";
                sSQL += "FROM DONVITINH D,HANGHOA H\n";
                sSQL += "OUTER APPLY (\n";
                sSQL += "SELECT STRING_AGG(b1.MAQUAY,',') AS MA_KHO, STRING_AGG(c1.TEN_KHO,', ') AS TEN_KHO\n";
                sSQL += "FROM dbo.CAUHINHKHUVUC b1 INNER JOIN dbo.KHO c1 ON TRY_CAST(b1.MAQUAY AS INT) = c1.MA_KHO\n";
                sSQL += "WHERE b1.MODE = '20240326' AND b1.MANHOM = h.MA_HANGHOA ) k\n";
                sSQL += "OUTER APPLY (SELECT k.MA_KHO, k.TEN_KHO FROM KHO k WHERE H.noixuat = k.MA_KHO) kk\n";
                sSQL += ",NHOMHANG N,DM_BEP B \n";
                sSQL += "WHERE H.MA_NHOMHANG=N.MA_NHOMHANG and H.MA_DONVITINH=D.MA_DONVITINH and H.MA_BEP=B.MA_BEP \n";
                sSQL += "ORDER BY TEN_NHOMHANG,TEN_HANGHOA";
                DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                foreach (DataRow dr in dt.Rows)
                {
                    clsHanghoa item = new clsHanghoa();
                    item.MA = dr["MA_HANGHOA"].ToString();
                    item.ID_HANGHOA = dr["ID_HANGHOA"].ToString();
                    item.TEN = dr["TEN_HANGHOA"].ToString();
                    item.TEN_DONVITINH = dr["TEN_DONVITINH"].ToString();
                    item.TEN_NHOMHANG = dr["TEN_NHOMHANG"].ToString();
                    item.MAVACH = dr["MAVACH"].ToString();
                    item.THUE = dr["THUE"].ToString();
                    item.GIANHAP = dr["GIANHAP"].ToString();
                    item.GIABAN1 = dr["GIABAN1"].ToString();
                    item.GIABAN2 = dr["GIABAN2"].ToString();
                    item.GIABAN3 = dr["GIABAN3"].ToString();
                    item.GIABAN4 = dr["GIABAN4"].ToString();
                    item.TONKHO = dr["TONKHO"].ToString();
                    item.TONTOITHIEU = dr["TONTOITHIEU"].ToString();
                    item.MA_THANHPHAN = dr["MA_THANHPHAN"].ToString();
                    item.SOLUONG = dr["SOLUONG"].ToString();
                    item.IS_INBEP = dr["IS_INBEP"].ToString();
                    item.TEN_BEP = dr["TEN_BEP"].ToString();
                    item.GHICHU = dr["GHICHU"].ToString();
                    item.SUDUNG = dr["SUDUNG"].ToString();
                    item.THUCDON = dr["THUCDON"].ToString();
                    item.GIATHEOTRONGLUONG = dr["GIATHEOTRONGLUONG"].ToString();
                    item.PLU = dr["PLU"].ToString();
                    item.HANSUDUNG = dr["HANSUDUNG"].ToString();
                    item.SUAGIA = dr["SUAGIA"].ToString();
                    item.SUADINHLUONG = dr["SUADINHLUONG"].ToString();
                    item.MONTHEM = dr["MONTHEM"].ToString();
                    item.INTEM = dr["INTEM"].ToString();
                    item.STT = dr["STT"].ToString();
                    item.HINHANH = dr["HINHANH"] == DBNull.Value ? null : (byte[])dr["HINHANH"];
                    item.NOIXUAT = dr["NOIXUAT"].ToString();

                    listitem.Add(item);
                }
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "Hanghoa", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }

        public IActionResult OnPost()
        {
            List<cls_hanghoa_excel> listData = new();
            if (UploadedFile != null)
            {
                try
                {
                    using (var stream = new MemoryStream())
                    {
                        UploadedFile.CopyTo(stream);
                        using (var package = new ExcelPackage(stream))
                        {
                            var workbook = package.Workbook;
                            // Get the names of all sheets in the workbook

                            foreach (var sheet in workbook.Worksheets)
                            {
                                // Get the used range of the worksheet
                                var usedRange = sheet.Dimension;
                                if (usedRange != null)
                                {
                                    int startRow = usedRange.Start.Row;
                                    int endRow = usedRange.End.Row;
                                    int startColumn = usedRange.Start.Column;
                                    int endColumn = usedRange.End.Column;

                                    // Read the used range into a 2D array
                                    var cells = sheet.Cells[startRow, startColumn, endRow, endColumn];
                                    var rows = cells.GroupBy(c => c.Start.Row)
                                                    .Select(g => g.Select(c => c.Text).ToArray())
                                                    .ToArray();
                                    //// Example: access a specific cell value
                                    //string cellValue = rows[0][0]; // Value of the top-left cell in the used range
                                    // validate data
                                    if (startColumn == 1) // dữ liệu bắt đầu từ cột A
                                    {
                                        foreach (var row in rows)
                                        {
                                            if (row.Length > 9)
                                            {
                                                double hesokpi = 1;
                                                double.TryParse(row[7], out hesokpi);
                                                if (hesokpi <= 0)
                                                {
                                                    hesokpi = 1;
                                                }
                                                if (!string.IsNullOrWhiteSpace(row[0]) && !string.IsNullOrWhiteSpace(row[3]) && double.TryParse(row[4]?.Trim(), out double sl_goi)
                                                    && double.TryParse(row[5]?.Trim(), out double giaban))
                                                {
                                                    string ten_hanghoa = row[0].Trim();
                                                    string ten_donvitinh = $"{row[3].Trim()}{(sl_goi == 1 ? "" : $" {sl_goi} KG")}";
                                                    if (!string.IsNullOrWhiteSpace(row[2])
                                                        && ten_hanghoa.Contains(row[2].Trim(), StringComparison.InvariantCultureIgnoreCase)
                                                        && ten_hanghoa.Contains(row[3].Trim(), StringComparison.InvariantCultureIgnoreCase))
                                                    {
                                                        // Tên hàng đã ghép nối với quy cách đóng gói, không cần nối thêm
                                                    }
                                                    else
                                                    {
                                                        Debug.Print($"ten_hanghoa: '{ten_hanghoa}', hat: '{row[2]}', donvi: '{row[3]}'");
                                                        // row[2] chứa dữ liệu "Kiểu hạt", không bắt buộc
                                                        if (!string.IsNullOrWhiteSpace(row[2]))
                                                        {
                                                            ten_hanghoa += $", {row[2].Trim()}";
                                                        }
                                                        ten_hanghoa += $", {ten_donvitinh}";
                                                    }
                                                    var item = new cls_hanghoa_excel()
                                                    {
                                                        ten_hanghoa = ten_hanghoa,
                                                        ten_donvitinh = row[3].Trim(),// chỉ lấy tên đơn vị tính không kể phần số lượng/gói
                                                        giaban = giaban,
                                                        ghichu = $"{row[9].Trim() ?? ""}{(row.Length > 10 && !string.IsNullOrWhiteSpace(row[10]) ? $", {row[10].Trim()}" : "")}",
                                                        hesokpi = hesokpi,
                                                        sl_goi = sl_goi,
                                                        tenkho = row[8]?.Trim() ?? "",
                                                        ten_nhomhang = sheet.Name.Trim(),
                                                        noi_sanxuat = row[1]?.Trim() ?? "",
                                                    };
                                                    listData.Add(item);
                                                }
                                            }
                                        }
                                    }

                                }
                                //break;
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    cls_Main.WriterLog(cls_Main.sFilePath, "Hanghoa", ex.ToString(), "0");
                    return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
                }
            }
            if (listData.Any())
            {
                // success, save database
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string sSQL = string.Join($";{Environment.NewLine}", listData.Select(r => $"EXEC dbo.SP_IMPORT_HANGHOA @ten_hanghoa = {ExtensionObject.toSqlPar(r.ten_hanghoa)}"
                + $", @noi_sanxuat = {ExtensionObject.toSqlPar(r.noi_sanxuat)}"
                + $", @sl_goi = {ExtensionObject.toSqlPar(r.sl_goi)}"
                + $", @giaban = {ExtensionObject.toSqlPar(r.giaban)}"
                + $", @hesokpi = {ExtensionObject.toSqlPar(r.hesokpi)}"
                + $", @ten_nhomhang = {ExtensionObject.toSqlPar(r.ten_nhomhang)}"
                + $", @tenkho = {ExtensionObject.toSqlPar(r.tenkho)}"
                + $", @ten_donvitinh = {ExtensionObject.toSqlPar(r.ten_donvitinh)}"
                + $", @ghichu = {ExtensionObject.toSqlPar(r.ghichu)}"));
                bool bRunSQL = cls_Main.ExecuteSQL(sSQL, sConnectionString_live);
                if (bRunSQL)
                {
                    TempData["AlertMessage"] = "Lưu thành công";
                    TempData["AlertType"] = "alert-success";
                }
                else
                {
                    TempData["AlertMessage"] = "Lưu không thành công";
                    TempData["AlertType"] = "alert-danger";
                }
            }
            else
            {
                TempData["AlertMessage"] = "Không tìm thấy dữ liệu trong file Excel";
                TempData["AlertType"] = "alert-danger";
            }
            return RedirectToPage("Index");
        }
    }
}
