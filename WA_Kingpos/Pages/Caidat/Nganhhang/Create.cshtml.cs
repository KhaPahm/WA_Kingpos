using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Xml.Linq;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.Nganhhang
{
    [Authorize]
    public class CreateModel : PageModel
    {
        [BindProperty]
        public clsNganhhang item { get; set; }
        public IActionResult OnGet()
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowAdd("6", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "Nganhhang Add", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        public IActionResult OnPost(clsNganhhang item)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string sSQL = "";
                sSQL = "";
                sSQL += "select OrderBy from ReferenceList where Category='ProductType' and OrderBy="+cls_Main.SQLString(item.OrderBy) + "\n";
                DataTable dtCheck = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                if (dtCheck.Rows.Count > 0)
                {
                    ViewData["Message"] = string.Format("Trùng mã ngành hàng");
                    return Page();
                }
                sSQL = "";
                sSQL += "insert into ReferenceList(ID,VNValue,ENValue,Category,OrderBy,Status) Values((select NEWID())," + "\n";
                sSQL += cls_Main.SQLStringUnicode(item.VNValue) + "," + "\n";
                sSQL += "(select dbo.[fLocDauTiengViet](" + cls_Main.SQLStringUnicode(item.VNValue) + "))" + "," + "\n";
                sSQL += "'ProductType'," + "\n";
                sSQL += cls_Main.SQLString(item.OrderBy) + ",1)";
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
                cls_Main.WriterLog(cls_Main.sFilePath, "Nganhhang Add", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
    }
}
