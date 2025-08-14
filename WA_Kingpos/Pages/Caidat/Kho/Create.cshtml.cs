using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using System.Net;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.Kho
{
    [Authorize]
    public class CreateModel : PageModel
    {
        [BindProperty]
        public clsKho item { get; set; }
        public List<Combobox> Combobox = new List<Combobox>();
        public IActionResult OnGet()
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowAdd("4", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                LoadData();
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "KHO Add", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        public IActionResult OnPost(clsKho item)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    LoadData();
                    return Page();
                }
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string sSQL = "";
                sSQL += "Insert into KHO(TEN_KHO, MA_CUAHANG,GHICHU,SUDUNG)" + "\n";
                sSQL += "Values ( ";
                sSQL += cls_Main.SQLStringUnicode(item.ten_kho) + ",";
                sSQL += cls_Main.SQLStringUnicode(item.ma_cuahang) + ",";
                sSQL += cls_Main.SQLStringUnicode(item.ghichu) + ",";
                sSQL += cls_Main.SQLBit(bool.Parse(item.sudung)) + ")";
                bool bRunSQL = cls_Main.ExecuteSQL(sSQL, sConnectionString_live);
                if (bRunSQL)
                {
                    TempData["AlertMessage"] = "Lưu thành công";
                    TempData["AlertType"] = "alert-success";
                    return RedirectToPage("Create");
                }
                else
                {
                    return RedirectToPage("/ErrorPage", new { id = "Lýu không thành công : " + sSQL });
                }
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "KHO Add", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        private void LoadData()
        {

            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "Select * From CUAHANG";
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                Combobox item = new Combobox();
                item.Value = dr["MA_CUAHANG"].ToString();
                item.Text = dr["TEN_CUAHANG"].ToString();

                Combobox.Add(item);
            }
        }
    }
}
