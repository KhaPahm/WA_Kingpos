using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.TaiKhoan
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public List<clsTaikhoan> listitem = new List<clsTaikhoan>();
        public bool bThem { get; set; }
        public bool bSua { get; set; }
        public bool bXoa { get; set; }
        public bool bHuydangnhap { get; set; }
        public bool bTientamung { get; set; }
        public List<DM_NHOMQUYEN> ListNhomQuyen { set; get; } = new List<DM_NHOMQUYEN>();
        public List<DM_CHUCVU> ListChucvu { set; get; } = new List<DM_CHUCVU>();
        public IActionResult OnGet()
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowView("19", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                else
                {
                    bThem = cls_UserManagement.AllowAdd("19", HttpContext.Session.GetString("Permission"));
                    bSua = cls_UserManagement.AllowEdit("19", HttpContext.Session.GetString("Permission"));
                    bXoa = cls_UserManagement.AllowDelete("19", HttpContext.Session.GetString("Permission"));
                    bHuydangnhap= cls_UserManagement.AllowView("19", HttpContext.Session.GetString("Permission"));
                    bTientamung= cls_UserManagement.AllowView("19", HttpContext.Session.GetString("Permission"));
                }
                //xu ly trang
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string sSQL = "EXEC SP_W_GETTAIKHOAN";
                DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                foreach (DataRow dr in dt.Rows)
                {
                    clsTaikhoan item = new clsTaikhoan();
                    item.userid = dr["UserId"].ToString();
                    item.nhomquyen = dr["GroupName"].ToString();
                    item.nhanvien = dr["TENNHANVIEN"].ToString();
                    item.ngaysinh = dr["NGAYSINH"].ToString();
                    item.gioitinh = dr["GIOITINH"].ToString();
                    item.diachi = dr["DIACHI"].ToString();
                    item.dienthoai = dr["DIENTHOAI"].ToString();
                    item.sudung = dr["Active"].ToString();
                    item.trangthai= dr["TRANGTHAI"].ToString();
                    item.thietbi = dr["PCName"].ToString();
                    item.thoiggian = dr["THOIGIAN"].ToString();
                    item.tientamung = dr["TamUng"].ToString();

                    listitem.Add(item);
                }
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "SYS_USER", ex.ToString(), "0");
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

            var firstRow = worksheet.FirstRowUsed().RowNumber() + 3;

            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            int successCount = 0;
            int errorCount = 0;
            var logPath = @"C:ImportExcell\Log\ImportTaiKhoanErrorLog.txt";
            LoadDataNhomQuyen();
            LoadDataChucVu();
            // ??m b?o th? m?c t?n t?i
            Directory.CreateDirectory(Path.GetDirectoryName(logPath));
            string sSQL = "";
            string MaNhanVien = string.Empty;
            for (int row = firstRow +1; row <= worksheet.LastRowUsed().RowNumber(); row++)
            {
                try
                {
                    var wsRow = worksheet.Row(row);
                    var tenTaiKhoan = wsRow.Cell(26).GetString();

                    if (string.IsNullOrWhiteSpace(tenTaiKhoan))
                        continue;

                    var matKhau = wsRow.Cell(27).GetString().Trim();
                    var nhomQuyen = wsRow.Cell(28).GetString().Trim();
                    var maNhomQuyen = ListNhomQuyen
                                   .FirstOrDefault(x => string.Equals(x.TENNHOMQUENU.Trim(), nhomQuyen.Trim(), StringComparison.OrdinalIgnoreCase))
                                   ?.MANHOMQUENU ?? "0";
                    var sudung = wsRow.Cell(29).GetString().Trim().ToLower();
                    bool sudungBool = sudung == "có";

                    var tenNhanVien = wsRow.Cell(3).GetString().Trim();
                    var chucVu = wsRow.Cell(4).GetString().Trim();
                    var machucvu = ListChucvu
                                    .FirstOrDefault(x => string.Equals(x.TENCHUCVU.Trim(), chucVu.Trim(), StringComparison.OrdinalIgnoreCase))
                                    ?.MACHUCVU ?? "0";
                    var sdt = wsRow.Cell(10).GetString().Trim();
                    sSQL = "";
                    sSQL = $"SELECT TOP 1 MANHANVIEN,CHUCVU,DIENTHOAI FROM DM_NHANVIEN WHERE TENNHANVIEN = {cls_Main.SQLStringUnicode(tenNhanVien)} AND REPLACE(DIENTHOAI, ' ', '') = REPLACE({cls_Main.SQLStringUnicode(sdt)}, ' ', '') AND CHUCVU={cls_Main.SQLString(machucvu)}";
                    DataTable dtCheckNV = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                    if (dtCheckNV.Rows.Count == 0)
                    {
                        continue;
                    }else
                    {
                        MaNhanVien = dtCheckNV.Rows[0]["MANHANVIEN"].ToString();
                    }         

                    // kiểm tra đã có tài khoản chưa
                    sSQL = "";
                    sSQL = $"SELECT * FROM dbo.SYS_USER WHERE UserID = {ExtensionObject.toSqlPar(tenTaiKhoan)}";
                    DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                    if (dt.Rows.Count > 0)
                    {
                        continue;
                    }

                    sSQL = "";
                    sSQL += "EXEC SP_W_TAIKHOAN ";
                    sSQL += cls_Main.SQLStringUnicode(tenTaiKhoan) + ",";
                    sSQL += cls_Main.SQLStringUnicode(matKhau) + ",";
                    sSQL += cls_Main.SQLStringUnicode(MaNhanVien) + ",";
                    sSQL += cls_Main.SQLStringUnicode(maNhomQuyen) + ",";
                    sSQL += cls_Main.SQLBit(sudungBool);

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
        private void LoadDataNhomQuyen()
        {
            //chuc vu
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "Select * From sys_group";
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                DM_NHOMQUYEN Chucvu = new DM_NHOMQUYEN();
                Chucvu.MANHOMQUENU = dr["GROUPID"].ToString();
                Chucvu.TENNHOMQUENU = dr["GROUPNAME"].ToString();
                ListNhomQuyen.Add(Chucvu);
            }
        }
        public class DM_NHOMQUYEN
        {
            public string MANHOMQUENU { get; set; }
            public string TENNHOMQUENU { get; set; }
        }
        private void LoadDataChucVu()
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
