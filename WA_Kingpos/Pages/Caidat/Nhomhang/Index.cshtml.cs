
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;
using static WA_Kingpos.Pages.Baocao.DSHanghoa.IndexModel;
using System.Data.SqlClient;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;
using A = DocumentFormat.OpenXml.Drawing;

namespace WA_Kingpos.Pages.Nhomhang
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public List<clsNhomhang> listitem = new List<clsNhomhang>();
        public bool bThem { get; set; }
        public bool bSua { get; set; }
        public bool bXoa { get; set; }
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
                sSQL += "Select MA_NHOMHANG As MA,TEN_NHOMHANG As TEN,GHICHU,SUDUNG,THUCDON,STT,HINHANH,MONTHEM" + "\n";
                sSQL += "From NHOMHANG" + "\n";
                sSQL += "Order by SUDUNG DESC,STT" + "\n";
                DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                foreach (DataRow dr in dt.Rows)
                {
                    clsNhomhang item = new clsNhomhang();
                    item.MA = dr["MA"].ToString();
                    item.TEN = dr["TEN"].ToString();
                    item.GHICHU = dr["GHICHU"].ToString();
                    item.SUDUNG = dr["SUDUNG"].ToString();
                    item.THUCDON = dr["THUCDON"].ToString();
                    item.STT = dr["STT"].ToString();
                    item.HINHANH = dr["HINHANH"] == DBNull.Value ? null : (byte[])dr["HINHANH"];
                    item.MONTHEM = dr["MONTHEM"].ToString();

                    listitem.Add(item);
                }
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "Nhomhang", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }

        public async Task<IActionResult> OnPostImportAsync(IFormFile ExcelFile)
        {
            if (ExcelFile == null || ExcelFile.Length == 0)
            {
                TempData["Exception"] = "Không có file được chọn.";
                return RedirectToPage();
            }

            int successCount = 0;
            int errorCount = 0;
            string logPath = @"C:\ImportExcell\Log\ImportErrorNhomHangLog.txt";
            var pictureDict = new Dictionary<int, byte[]>(); // key: row, value: image bytes

            // Lưu file tạm để đọc bằng ClosedXML + OpenXML
            var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".xlsx");
            using (var fileStream = new FileStream(tempPath, FileMode.Create))
            {
                await ExcelFile.CopyToAsync(fileStream);
            }

            // Extract image dictionary theo dòng từ OpenXML
            pictureDict = ExtractImagesFromExcel(tempPath);

            try
            {
                using (var workbook = new XLWorkbook(tempPath))
                {
                    var worksheet = workbook.Worksheet(1); // sheet đầu tiên
                    int rowCount = worksheet.LastRowUsed().RowNumber();

                    string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                    string sSQL = "";
                    for (int row = 2; row <= rowCount; row++)
                    {
                        try
                        {
                            string STT = worksheet.Cell(row, 1).GetString().Trim();
                            string TEN = worksheet.Cell(row, 2).GetString().Trim();
                            if (string.IsNullOrWhiteSpace(TEN))
                                continue;

                            string GHICHU = worksheet.Cell(row, 3).GetString().Trim();
                            bool SUDUNG = worksheet.Cell(row, 5).GetString().ToLower().Contains("có");
                            bool THUCDON = worksheet.Cell(row, 6).GetString().ToLower().Contains("có");
                            bool MONTHEM = worksheet.Cell(row, 7).GetString().ToLower().Contains("có");

                            byte[] imageData = pictureDict.ContainsKey(row) ? pictureDict[row] : null;

                            sSQL = $"select MA_NHOMHANG,TEN_NHOMHANG,STT from nhomhang where TEN_NHOMHANG={cls_Main.SQLStringUnicode(TEN)} and STT={cls_Main.SQLString(STT)}";
                            DataTable check = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                            if(check.Rows.Count>0)
                            {
                                continue;
                            }    


                            sSQL = @"INSERT INTO NHOMHANG 
                            (TEN_NHOMHANG, GHICHU, SUDUNG, THUCDON, STT, HINHANH, MONTHEM) 
                            VALUES (@TEN, @GHICHU, @SUDUNG, @THUCDON, @STT, @HINHANH, @MONTHEM)";

                            var parameters = new List<SqlParameter>
                            {
                                new SqlParameter("@TEN", TEN),
                                new SqlParameter("@GHICHU", string.IsNullOrEmpty(GHICHU) ? DBNull.Value : GHICHU),
                                new SqlParameter("@SUDUNG", SUDUNG),
                                new SqlParameter("@THUCDON", THUCDON),
                                new SqlParameter("@STT", STT),
                                new SqlParameter("@HINHANH", SqlDbType.Image) { Value = (object?)imageData ?? DBNull.Value },
                                new SqlParameter("@MONTHEM", MONTHEM)
                            };

                            bool bRunSQL = cls_Main.ExecuteSQL(sSQL, sConnectionString_live, parameters);

                            if (bRunSQL)
                            {
                                successCount++;
                            }
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
                }
            }
            finally
            {
                System.IO.File.Delete(tempPath); // cleanup file tạm
            }

            TempData["Message"] = $"Import thành công {successCount} dòng. Lỗi {errorCount} dòng.";
            return RedirectToPage("/Caidat/Nhomhang/Index");
        }
        public Dictionary<int, byte[]> ExtractImagesFromExcel(string filePath)
        {
            var images = new Dictionary<int, byte[]>();

            using (SpreadsheetDocument document = SpreadsheetDocument.Open(filePath, false))
            {
                var workbookPart = document.WorkbookPart;
                var worksheetPart = workbookPart.WorksheetParts.FirstOrDefault();
                if (worksheetPart == null) return images;

                var drawingsPart = worksheetPart.DrawingsPart;
                if (drawingsPart == null) return images;

                var wsDr = drawingsPart.WorksheetDrawing;

                foreach (var drawing in wsDr.OfType<TwoCellAnchor>())
                {
                    var blip = drawing.Descendants<A.Blip>().FirstOrDefault();
                    if (blip == null) continue;

                    var embedId = blip.Embed.Value;
                    var imagePart = (ImagePart)drawingsPart.GetPartById(embedId);

                    using (var stream = imagePart.GetStream())
                    using (var ms = new MemoryStream())
                    {
                        stream.CopyTo(ms);
                        byte[] imageBytes = ms.ToArray();

                        // Xác định dòng (row) gắn ảnh, Excel là 0-based → +1
                        int fromRow = drawing.FromMarker?.RowId != null ? int.Parse(drawing.FromMarker.RowId.InnerText) + 1 : -1;

                        if (fromRow > 0 && !images.ContainsKey(fromRow))
                        {
                            images[fromRow] = imageBytes;
                        }
                    }
                }
            }

            return images;
        }


        //public async Task<IActionResult> OnPostImportAsync(IFormFile ExcelFile)
        //{
        //    if (ExcelFile == null || ExcelFile.Length == 0)
        //    {
        //        ModelState.AddModelError("", "Vui lòng chọn file Excel.");
        //        return Page();
        //    }

        //    using var stream = new MemoryStream();
        //    await ExcelFile.CopyToAsync(stream);

        //    using var workbook = new XLWorkbook(stream);
        //    var worksheet = workbook.Worksheets.First();

        //    var firstRow = worksheet.FirstRowUsed().RowNumber() + 1;

        //    string sConnectionString_live = cls_ConnectDB.GetConnect("0");
        //    int successCount = 0;
        //    int errorCount = 0;
        //    var logPath = @"C:ImportExcell\Log\ImportNganhHangErrorLog.txt";

        //    // ??m b?o th? m?c t?n t?i
        //    Directory.CreateDirectory(Path.GetDirectoryName(logPath));
        //    string sSQL = "";
        //    for (int row = firstRow; row <= worksheet.LastRowUsed().RowNumber(); row++)
        //    {
        //        try
        //        {
        //            var wsRow = worksheet.Row(row);
        //            var tennhom = wsRow.Cell(2).GetString();

        //            if (string.IsNullOrWhiteSpace(tennhom))
        //                continue;

        //            var stt = wsRow.Cell(1).GetString().Trim();
        //            var ghiChu = wsRow.Cell(3).GetString().Trim();


        //            //sSQL = "";
        //            //sSQL += "select OrderBy from ReferenceList where Category='ProductType' and OrderBy=" + cls_Main.SQLString(ma) + "\n";
        //            //DataTable dtCheck = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
        //            //if (dtCheck.Rows.Count > 0)
        //            //{
        //            //    continue;
        //            //}

        //            //sSQL += "insert into ReferenceList(ID,VNValue,ENValue,Category,OrderBy,Status) Values((select NEWID())," + "\n";
        //            //sSQL += cls_Main.SQLStringUnicode(tennganh) + "," + "\n";
        //            //sSQL += "(select dbo.[fLocDauTiengViet](" + cls_Main.SQLStringUnicode(tennganh) + "))" + "," + "\n";
        //            //sSQL += "'ProductType'," + "\n";
        //            //sSQL += cls_Main.SQLString(ma) + ",1)";

        //            bool bRunSQL = cls_Main.ExecuteSQL(sSQL, sConnectionString_live);

        //            if (bRunSQL)
        //                successCount++;
        //            else
        //            {
        //                errorCount++;
        //                System.IO.File.AppendAllText(logPath, $"Lỗi tại dòng {row}: Không thể thực thi SQL.\n");
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            errorCount++;
        //            System.IO.File.AppendAllText(logPath, $"Lỗi tại dòng {row}: {ex.Message}\n");
        //        }
        //    }

        //    // Thông báo
        //    TempData["Message"] = $"Import hoàn tấtt: {successCount} dòng thành công, {errorCount} dòng lỗi.";



        //    return Page();
        //}
    }
}
