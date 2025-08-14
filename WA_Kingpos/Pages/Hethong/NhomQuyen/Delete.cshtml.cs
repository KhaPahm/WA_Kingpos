using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.NhomQuyen
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        public clsNhomquyen item = new clsNhomquyen();
        public IActionResult OnGet(string id)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowDelete("2", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                LoadData(id);
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "DM_CHUCVU Delete : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        private void LoadData(string id)
        {
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "Select * From SYS_GROUP Where GroupID=" + cls_Main.SQLString(id);
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                item.ma_nhomquyen = dr["GroupID"].ToString();
                item.ten_nhomquyen = dr["GroupName"].ToString();
                item.ghichu = dr["Description"].ToString();
                item.sudung = dr["Active"].ToString();
            }
        }
        public IActionResult Onpost(string id)
        {
            try
            {
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string sSQL = "";
                sSQL = "Select TOP 1 GroupID from SYS_USER where GroupID=" + cls_Main.SQLString(id);
                DataTable dtCheck = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                if (dtCheck.Rows.Count > 0)
                {
                    ViewData["Message"] = string.Format("Nhóm quyền đang được sử dụng, không thể xóa!");
                    LoadData(id);
                    return Page();
                }
                else
                {
                    sSQL = "";
                    sSQL += "Delete from SYS_GROUP_RULE where GroupID=" + cls_Main.SQLString(id) + "\n";
                    sSQL += "Delete From SYS_GROUP Where GroupID=" + cls_Main.SQLString(id) + "\n";
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
                cls_Main.WriterLog(cls_Main.sFilePath, "DM_CHUCVU Delete : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
    }
}
