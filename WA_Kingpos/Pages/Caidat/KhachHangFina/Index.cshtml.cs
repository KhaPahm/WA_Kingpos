using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Globalization;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.KHACHANG
{
    [Authorize]
    public class KhachHangIndexModel : PageModel
    {
        public List<KhachHangModels> listitem = new List<KhachHangModels>();
        public bool bThem { get; set; }
        public bool bSua { get; set; }
        public bool bXoa { get; set; }
        public KhachHangModels item { get; set; } = new KhachHangModels();
        public string sIDKHTT = "";
        public string sTen = "";
        public string sDiaChi = "";
        public string sMST = "";
        public IActionResult OnGet()
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowView("23120801", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                else
                {
                    bThem = cls_UserManagement.AllowAdd("23120801", HttpContext.Session.GetString("Permission"));
                    bSua = cls_UserManagement.AllowEdit("23120801", HttpContext.Session.GetString("Permission"));
                    bXoa = cls_UserManagement.AllowDelete("23120801", HttpContext.Session.GetString("Permission"));
                }
                //xu ly trang
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string manhanvien = User.FindFirst("UserId")?.Value;
                string sSQL = "EXEC SP_GET_CUAHANG_KHO_QUAY_THEO_NHANVIEN " + cls_Main.SQLString(manhanvien) + ", 10";
                DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                foreach (DataRow dr in dt.Rows)
                {
                    KhachHangModels item = new KhachHangModels();
                    item.MA = dr["MA"].ToString();
                    item.TEN = dr["TEN"].ToString();
                    item.DIACHI = dr["DIACHI"].ToString();
                    item.DIENTHOAI = dr["DIENTHOAI"].ToString();
                    item.EMAIL = dr["EMAIL"].ToString();
                    item.FAX = dr["FAX"].ToString();
                    item.CMND = dr["CMND"].ToString();
                    item.SUDUNG = dr["SUDUNG"].ToString();
                    listitem.Add(item);
                }
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "KhachHangFina", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }

        public async Task<IActionResult> OnPostImportAsync(IFormFile ExcelFile)
        {
            if (ExcelFile == null || ExcelFile.Length == 0)
            {
                ModelState.AddModelError("", "Vui lòng ch?n file Excel.");
                return Page();
            }
            
            using var stream = new MemoryStream();
            await ExcelFile.CopyToAsync(stream);

            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheets.First();
            
            var firstRow = worksheet.FirstRowUsed().RowNumber() + 1; // B? dòng tiêu ??, v.v.

            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            int successCount = 0;
            int errorCount = 0;
            var logPath = @"C:ImportExcell\Log\ImportKHErrorLog.txt";

            // ??m b?o th? m?c t?n t?i
            Directory.CreateDirectory(Path.GetDirectoryName(logPath));
            string? manhanvien = User.FindFirst("UserId")?.Value;
            for (int row = firstRow ; row <= worksheet.LastRowUsed().RowNumber(); row++)
            {
                try
                {
                    var wsRow = worksheet.Row(row);
                    var tennhanvien = wsRow.Cell(2).GetString();

                    if (string.IsNullOrWhiteSpace(tennhanvien))
                        continue;
                    LoadData();
                    var thon = wsRow.Cell(3).GetString().Trim();
                    var xa = wsRow.Cell(4).GetString().Trim();
                    var huyen = wsRow.Cell(5).GetString().Trim();
                    var tinh = wsRow.Cell(6).GetString().Trim();
                    var mst = wsRow.Cell(7).GetString().Trim();
                    var sdt = wsRow.Cell(8).GetString().Trim();
                    var chuSoHuu = wsRow.Cell(9).GetString().Trim();
                    var cccd = wsRow.Cell(10).GetString().Trim();
                    var email = wsRow.Cell(11).GetString().Trim();
                    var Sudung = wsRow.Cell(12).GetString().Trim().ToLower();
                    var diaChi = thon+", "+xa + ", " +huyen + ", " +tinh;
                    bool sudungBool = Sudung == "có";
                    if(!CheckMST(mst))
                    {
                        continue;
                    }    

                    string sSQL = "";
                    sSQL += "Insert into NHACUNGCAP (ID,LOAI,TEN,DIACHI,DIENTHOAI,FAX,EMAIL,MAKHGIOITHIEU,SUDUNG)" + "\n";
                    sSQL += "Values ( ";
                    sSQL += cls_Main.SQLString(sIDKHTT) + ",";
                    sSQL += cls_Main.SQLString("0") + ",";
                    sSQL += cls_Main.SQLStringUnicode(tennhanvien) + ",";
                    sSQL += cls_Main.SQLStringUnicode(diaChi) + ",";
                    sSQL += cls_Main.SQLString(sdt) + ",";
                    sSQL += cls_Main.SQLString(mst ?? "") + ",";
                    sSQL += cls_Main.SQLString(email ?? "") + ",";
                    sSQL += cls_Main.SQLString(manhanvien ?? "") + ",";
                    sSQL += cls_Main.SQLBit(sudungBool) + ")";

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

        private void LoadData(string sten = "", string sdiachi = "", string smst = "")
        {
            // ID
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "select [dbo].[fc_NewCodeID_NHACUNGCAP]  ()";
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            if (dt.Rows.Count > 0)
            {
                sIDKHTT = dt.Rows[0][0].ToString();
            }
            else
            {
                sIDKHTT = "";
            }
            //TEN
            if (!string.IsNullOrEmpty(sten))
            {
                sTen = sten;
            }
            //DIACHI
            if (!string.IsNullOrEmpty(sten))
            {
                sDiaChi = sdiachi;
            }
            //MST
            if (!string.IsNullOrEmpty(smst))
            {
                sMST = smst;
            }
        }

        private bool CheckMST(string MST)
        {
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "";
            if (!string.IsNullOrEmpty(MST))
            {
                if (!string.IsNullOrEmpty(MST))
                {
                    sSQL += "select FAX from  NHACUNGCAP  where FAX=" + cls_Main.SQLString(MST);
                }
                else
                {
                    sSQL += "select FAX from  NHACUNGCAP  where FAX=" + cls_Main.SQLString(MST) + " OR MA<>" + cls_Main.SQLString(MST);

                }
                DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                if (dt.Rows.Count > 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
