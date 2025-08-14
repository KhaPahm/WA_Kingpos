using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.TaiXe
{
    [Authorize]
    public class TaiXeIndexModel : PageModel
    {
        public List<TaiXeModels> listitem = new List<TaiXeModels>();
        public bool bThem { get; set; }
        public bool bSua { get; set; }
        public bool bXoa { get; set; }
        public string sID = "";
        public IActionResult OnGet()
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowView("23120802", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                else
                {
                    bThem = cls_UserManagement.AllowAdd("23120802", HttpContext.Session.GetString("Permission"));
                    bSua = cls_UserManagement.AllowEdit("23120802", HttpContext.Session.GetString("Permission"));
                    bXoa = cls_UserManagement.AllowDelete("23120802", HttpContext.Session.GetString("Permission"));
                }
                //xu ly trang
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string nguoitao = User.FindFirst("Username")?.Value;
                string manhanvien = User.FindFirst("UserId")?.Value;
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
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "TaiXeFina", ex.ToString(), "0");
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

            using var stream = new MemoryStream();
            await ExcelFile.CopyToAsync(stream);

            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheets.First();

            var firstRow = worksheet.FirstRowUsed().RowNumber() + 1; // B? dòng tiêu ??, v.v.

            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            int successCount = 0;
            int errorCount = 0;
            var logPath = @"C:ImportExcell\Log\ImportTaiXeErrorLog.txt";

            // ??m b?o th? m?c t?n t?i
            Directory.CreateDirectory(Path.GetDirectoryName(logPath));
            string? manhanvien = User.FindFirst("UserId")?.Value;
            for (int row = firstRow; row <= worksheet.LastRowUsed().RowNumber(); row++)
            {
                try
                {
                    var wsRow = worksheet.Row(row);
                    var tennhanvien = wsRow.Cell(2).GetString();

                    if (string.IsNullOrWhiteSpace(tennhanvien))
                        continue;
                    LoadData();
                   
                    var sdt = wsRow.Cell(3).GetString().Trim();
                    var cccd = wsRow.Cell(4).GetString().Trim();
                    var bienSo = wsRow.Cell(5).GetString().Trim();
                    var Sudung = wsRow.Cell(6).GetString().Trim().ToLower();
                    bool sudungBool = Sudung == "có";
                    if (!CheckCMND(cccd))
                    {
                        continue;
                    }

                    string sSQL = "";
                    sSQL = "";
                    sSQL += "Insert into NHACUNGCAP (ID,LOAI,TEN,DIACHI,DIENTHOAI,FAX,EMAIL,WEBSITE,NGUOILIENHE,GHICHU,CMND,NGAYSINH,GIOITINH,LOAIDAILY,CAPDO,TIENDATCOC,ISDATCOC,HANTHANHTOAN,HANMUC_CONGNO,LastUpdate,MAKHGIOITHIEU,SUDUNG)" + "\n";
                    sSQL += "Values ( ";
                    sSQL += cls_Main.SQLString(sID) + ",";
                    sSQL += cls_Main.SQLString("1") + ",";
                    sSQL += cls_Main.SQLStringUnicode(tennhanvien) + ",";
                    sSQL += cls_Main.SQLStringUnicode("") + ",";
                    sSQL += cls_Main.SQLString(sdt) + ",";
                    sSQL += cls_Main.SQLString("") + ",";
                    sSQL += cls_Main.SQLString("") + ",";
                    sSQL += cls_Main.SQLString(bienSo) + ",";
                    sSQL += cls_Main.SQLStringUnicode("") + ",";
                    sSQL += cls_Main.SQLStringUnicode("") + ",";
                    sSQL += cls_Main.SQLString(cccd ?? "") + ",";
                    sSQL += cls_Main.SQLString(String.Format("{0:yyyyMMdd}", DateTime.Now)) + ",";
                    sSQL += cls_Main.SQLBit(true) + ",";
                    sSQL += cls_Main.SQLString("1") + ",";
                    sSQL += cls_Main.SQLString("1") + ",";

                    sSQL += cls_Main.SQLString("0") + ",";
                    sSQL += cls_Main.SQLBit(false) + ",";
                    sSQL += cls_Main.SQLString("0") + ",";
                    sSQL += cls_Main.SQLString("0") + ",";
                    sSQL += "GETDATE()" + ",";
                    sSQL += cls_Main.SQLString(manhanvien) + ",";
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
        private void LoadData()
        {
            // ID
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "select [dbo].[fc_NewCodeID_NHACUNGCAP]  ()";
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            if (dt.Rows.Count > 0)
            {
                sID = dt.Rows[0][0].ToString();
            }
            else
            {
                sID = "";
            }
        }
        private bool CheckCMND(string cmnd)
        {
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "";
            if (!string.IsNullOrEmpty(cmnd))
            {
                 sSQL += "select CMND from  NHACUNGCAP  where CMND=" + cls_Main.SQLString(cmnd);                                
                DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                if (dt.Rows.Count > 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return true;
        }
    }
}
