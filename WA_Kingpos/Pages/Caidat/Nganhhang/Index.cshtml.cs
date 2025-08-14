using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.Nganhhang
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public List<clsNganhhang> listitem = new List<clsNganhhang>();
        public bool bThem { get; set; }
        public bool bSua { get; set; }
        public bool bXoa { get; set; }
        public IActionResult OnGet()
        {
            try
            {
                //check quyen
                if (!cls_UserManagement .AllowView ("6", HttpContext.Session .GetString ("Permission")))
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
                string sSQL = "select ID,VNValue,OrderBy from ReferenceList where Status=1 and Category='ProductType'";
                DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                foreach (DataRow dr in dt.Rows)
                {
                    clsNganhhang item = new clsNganhhang();
                    item.ID = dr["ID"].ToString();
                    item.VNValue = dr["VNValue"].ToString();
                    item.OrderBy = dr["OrderBy"].ToString();
                    listitem.Add(item);
                }
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "Nganhhang", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage",new { id = ex.ToString() });
            }
        }
        public async Task<IActionResult> OnPostImportAsync(IFormFile ExcelFile)
        {
            if (ExcelFile == null || ExcelFile.Length == 0)
            {
                ModelState.AddModelError("", "Vui lòng chọn file Excel.");
                return Page();
            }

            using var stream = new MemoryStream();
            await ExcelFile.CopyToAsync(stream);

            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheets.First();

            var firstRow = worksheet.FirstRowUsed().RowNumber() + 1; 

            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            int successCount = 0;
            int errorCount = 0;
            var logPath = @"C:ImportExcell\Log\ImportNganhHangErrorLog.txt";

            // ??m b?o th? m?c t?n t?i
            Directory.CreateDirectory(Path.GetDirectoryName(logPath));
            string sSQL = "";
            for (int row = firstRow; row <= worksheet.LastRowUsed().RowNumber(); row++)
            {
                try
                {
                    var wsRow = worksheet.Row(row);
                    var tennganh = wsRow.Cell(2).GetString();

                    if (string.IsNullOrWhiteSpace(tennganh))
                        continue;

                    var ma = wsRow.Cell(1).GetString().Trim();

                    
                    sSQL = "";
                    sSQL += "select OrderBy from ReferenceList where Category='ProductType' and OrderBy=" + cls_Main.SQLString(ma) + "\n";
                    DataTable dtCheck = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                    if (dtCheck.Rows.Count > 0)
                    {
                        continue;
                    }

                    sSQL += "insert into ReferenceList(ID,VNValue,ENValue,Category,OrderBy,Status) Values((select NEWID())," + "\n";
                    sSQL += cls_Main.SQLStringUnicode(tennganh) + "," + "\n";
                    sSQL += "(select dbo.[fLocDauTiengViet](" + cls_Main.SQLStringUnicode(tennganh) + "))" + "," + "\n";
                    sSQL += "'ProductType'," + "\n";
                    sSQL += cls_Main.SQLString(ma) + ",1)";

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
            TempData["Message"] = $"Import hoàn tấtt: {successCount} dòng thành công, {errorCount} dòng lỗi.";



            return Page();
        }
    }
}
