using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.TaiKhoan
{
    [Authorize]
    public class CreateModel : PageModel
    {
        [BindProperty]
        public List<DM_NHANVIEN> Listnhanvien { set; get; } = new List<DM_NHANVIEN>();
        public List<NHOMQUYEN> ListNhomquyen { set; get; } = new List<NHOMQUYEN>();
        public clsTaikhoan item { get; set; }
        public IActionResult OnGet(clsTaikhoan? item)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowAdd("19", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                if (item != null)
                {
                    this.item = item;
                }
                Loaddata();
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "SYS_USER Add", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        private void Loaddata()
        {
            //nhanvien
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "Select MANHANVIEN,TENNHANVIEN From DM_NHANVIEN where SUDUNG=1 order by TENNHANVIEN";
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                DM_NHANVIEN Nhanvien = new DM_NHANVIEN();
                Nhanvien.MANHANVIEN = dr["MANHANVIEN"].ToString();
                Nhanvien.TENNHANVIEN = dr["TENNHANVIEN"].ToString();
                Listnhanvien.Add(Nhanvien);
            }
            //nhomquyen
            sSQL = "select GroupId, GroupName from sys_group where Active=1 order by GroupName";
            dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                NHOMQUYEN Nhomquyen = new NHOMQUYEN();
                Nhomquyen.MA = dr["GroupId"].ToString();
                Nhomquyen.TEN = dr["GroupName"].ToString();
                ListNhomquyen.Add(Nhomquyen);
            }
        }
        public IActionResult OnPost(clsTaikhoan item)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Loaddata();
                    return Page();
                }
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string sSQL = "";
                sSQL = $"SELECT * FROM dbo.SYS_USER WHERE UserID = {ExtensionObject.toSqlPar(item.userid)}";
                DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                if (dt.Rows.Count > 0)
                {
                    TempData["AlertMessage"] = "Tài khoản đã tồn tại";
                    TempData["AlertType"] = "alert-danger";
                    return RedirectToPage("Create", item);
                }

                sSQL += "EXEC SP_W_TAIKHOAN ";
                sSQL += cls_Main.SQLStringUnicode(item.userid) + ",";
                sSQL += cls_Main.SQLStringUnicode(item.matkhau) + ",";
                sSQL += cls_Main.SQLStringUnicode(item.nhanvien) + ",";
                sSQL += cls_Main.SQLStringUnicode(item.nhomquyen) + ",";
                sSQL += cls_Main.SQLBit(bool.Parse(item.sudung));
                bool bRunSQL = cls_Main.ExecuteSQL(sSQL, sConnectionString_live);
                if (bRunSQL)
                {
                    TempData["AlertMessage"] = "Lưu thành công";
                    TempData["AlertType"] = "alert-success";
                    return RedirectToPage("Create");
                }
                else
                {
                    return RedirectToPage("/ErrorPage", new { id = "Lưu không thành công : " + sSQL });
                }
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "SYS_USER Add", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        public class DM_NHANVIEN
        {
            public string MANHANVIEN { get; set; }
            public string TENNHANVIEN { get; set; }
        }
        public class NHOMQUYEN
        {
            public string MA { get; set; }
            public string TEN { get; set; }
        }
    }
}
