using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.NhomNhanVien
{
    [Authorize]
    public class EditModel : PageModel
    {
        public clsNhomNhanVien item = new();

        public List<clsNhanvien> listNhanVien = new();

        public bool bXoa { get; set; }

        public IActionResult OnGet(string id)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowEdit(IndexModel.PermNhomNhanVien, HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                bXoa = cls_UserManagement.AllowDelete(IndexModel.PermNhomNhanVien, HttpContext.Session.GetString("Permission"));
                //xu ly trang
                LoadData(id);
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "SYS_GROUP Edit : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        public IActionResult OnPost(string id, clsNhomNhanVien item, [FromForm(Name = "THEM_THANHVIEN")] List<string> listThemThanhVien)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    LoadData(id);
                    return Page();
                }
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                // nối danh sách thành viên cũ và mới, và quản lý
                item.THANHVIEN = string.Join(",", $"{item.THANHVIEN}".Split(',').Concat(listThemThanhVien).Concat(new List<string?>() { item.MA_NGUOIQUANLY?.ToString() })
                    .Select(r => { uint ret; uint.TryParse(r, out ret); return ret; }).Where(r => r > 0).Distinct());
                string sSQL = $"UPDATE dbo.DM_NHOM_NHANVIEN SET TENNHOM_NHANVIEN = {ExtensionObject.toSqlPar(item.TENNHOM_NHANVIEN)}"
                    + $", MA_NGUOIQUANLY = {ExtensionObject.toSqlPar(item.MA_NGUOIQUANLY)}, THANHVIEN = {ExtensionObject.toSqlPar(item.THANHVIEN)}, GHICHU = {ExtensionObject.toSqlPar(item.GHICHU)}"
                    + $" WHERE MANHOM = {ExtensionObject.toSqlPar(item.MANHOM)}";

                bool bRunSQL = cls_Main.ExecuteSQL(sSQL, sConnectionString_live);
                if (bRunSQL)
                {
                    return RedirectToPage("Index");
                }
                else
                {
                    return RedirectToPage("/ErrorPage", new { id = "Lưu không thành công : " + sSQL });
                }
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "SYS_GROUP Edit : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }

        private void LoadData(string id)
        {
            item = IndexModel.LoadList(id).First();
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = $"SELECT * FROM dbo.DM_NHANVIEN WHERE TRANGTHAI <> 2 ORDER BY TENNHANVIEN ";

            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            listNhanVien = dt.ToList<clsNhanvien>();
            // xóa các nhân viên đã có trong nhóm
            var listOld = item.listThanhVien.Select(r => r.manhanvien).ToHashSet();
            listNhanVien.RemoveAll(r => listOld.Contains(r.manhanvien));
        }


    }
}
