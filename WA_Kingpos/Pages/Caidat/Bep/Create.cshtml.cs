using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.Bep
{
    [Authorize]
    public class CreateModel : PageModel
    {
        [BindProperty]
        public clsBep item { get; set; }
        public List<CUAHANG> ListCuahang { set; get; } = new List<CUAHANG>();
        public IActionResult OnGet()
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowAdd("8", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                LoadData();

                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "DM_Bep Add", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        public IActionResult OnPost(clsBep item)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    //xu ly trang
                    LoadData();
                    return Page();
                }
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string sSQL = "";
                sSQL += "Insert into DM_BEP (MA_CUAHANG,TEN_BEP,MAYINBEP,IP,GHICHU,SUDUNG)" + "\n";
                sSQL += "Values ( ";
                sSQL += cls_Main.SQLString(item.ma_cuahang) + ",";
                sSQL += cls_Main.SQLStringUnicode(item.ten_bep) + ",";
                sSQL += cls_Main.SQLStringUnicode(item.mayinbep) + ",";
                sSQL += cls_Main.SQLStringUnicode(item.ip) + ",";
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
                    return RedirectToPage("/ErrorPage", new { id = "Lưu không thành công : " + sSQL });
                }
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "DM_Bep Add", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        private void LoadData()
        {
            //cua hang
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "Select * From CUAHANG";
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                CUAHANG cuahang = new CUAHANG();
                cuahang.MA_CUAHANG = dr["MA_CUAHANG"].ToString();
                cuahang.TEN_CUAHANG = dr["TEN_CUAHANG"].ToString();
                ListCuahang.Add(cuahang);
            }
        }
        public class CUAHANG
        {
            public string MA_CUAHANG { get; set; }
            public string TEN_CUAHANG { get; set; }
        }
    }
}
