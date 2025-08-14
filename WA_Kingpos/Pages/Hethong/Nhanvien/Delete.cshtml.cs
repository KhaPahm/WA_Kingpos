using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.Nhanvien
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        public clsNhanvien item = new clsNhanvien();
        public IActionResult OnGet(string id)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowDelete("11", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                LoadData(id);
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "DM_NHANVIEN Delete : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        public IActionResult Onpost(string id)
        {
            try
            {
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string sSQL = "";
                sSQL = "Select TOP 1 MaNV from SYS_USER where MaNV=" + cls_Main.SQLString(id);
                DataTable dtCheck = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                if (dtCheck.Rows.Count > 0)
                {
                    ViewData["Message"] = string.Format("Tài khoản còn tồn tại, không thể xóa!");
                    LoadData(id);
                    return Page();
                }
                else
                {
                    sSQL += "Delete From DM_NHANVIEN" + "\n";
                    sSQL += "Where MANHANVIEN=" + cls_Main.SQLString(id) + "\n";
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
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "DM_NHANVIEN Delete : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }

        private void LoadData(string id)
        {
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "EXEC SP_W_GETNHANVIEN " + cls_Main.SQLString(id);
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
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
            }
        }
    }
}
