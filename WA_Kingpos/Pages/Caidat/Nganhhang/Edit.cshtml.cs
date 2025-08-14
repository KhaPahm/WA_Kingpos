using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Runtime.Intrinsics.X86;
using WA_Kingpos.Data;
using WA_Kingpos.Models;
using static WA_Kingpos.Pages.Bep.CreateModel;

namespace WA_Kingpos.Pages.Nganhhang
{
    [Authorize]
    public class EditModel : PageModel
    {
        public clsNganhhang item = new clsNganhhang();
        public IActionResult OnGet(string id)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowEdit("6", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                LoadData(id);
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "Nganhhang Edit : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        public IActionResult OnPost(string id, clsNganhhang item)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    LoadData(id);
                    return Page();
                }
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string sSQL = "";
                sSQL = "";
                sSQL += "select OrderBy from ReferenceList where Category='ProductType' and OrderBy=" + cls_Main.SQLString(item.OrderBy) + " and ID!=" + cls_Main.SQLString(id) + "\n";
                DataTable dtCheck = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                if (dtCheck.Rows.Count > 0)
                {
                    ViewData["Message"] = string.Format("Trùng mã ngành hàng");
                    LoadData(id);
                    return Page();
                }
                sSQL = "";
                sSQL += "Update ReferenceList set" + "\n";
                sSQL += "OrderBy=" + cls_Main.SQLString(item.OrderBy) + "," + "\n";
                sSQL += "VNValue=" + cls_Main.SQLStringUnicode(item.VNValue) + "," + "\n";
                sSQL += "ENValue=(select dbo.[fLocDauTiengViet](" + cls_Main.SQLStringUnicode(item.VNValue) + "))" + "\n";
                sSQL += "where ID=" + cls_Main.SQLString(id);
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
                cls_Main.WriterLog(cls_Main.sFilePath, "Nganhhang Edit : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        private void LoadData(string id)
        {
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "select ID,VNValue,OrderBy from ReferenceList where Category='ProductType' and ID=" + cls_Main.SQLString(id);
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                item.ID = dr["ID"].ToString();
                item.VNValue = dr["VNValue"].ToString();
                item.OrderBy = dr["OrderBy"].ToString();
            }
        }
    }
}
