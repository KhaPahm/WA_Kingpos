using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static WA_Kingpos.Pages.Nhanvien.EditModel;
using WA_Kingpos.Models;
using Microsoft.AspNetCore.Authorization;
using WA_Kingpos.Data;
using System.Data;

namespace WA_Kingpos.Pages.Kho
{
    [Authorize]
    public class EditModel : PageModel
    {
        public clsKho item = new clsKho();
        public List<Combobox> Combobox { set; get; } = new List<Combobox>();
        public IActionResult OnGet(string id)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowEdit("4", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                LoadData(id);

                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "KHO Edit", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        public IActionResult OnPost(string id, clsKho item)
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
                sSQL += "Update KHO set TEN_KHO = " + cls_Main.SQLStringUnicode(item.ten_kho) + ", ";
                sSQL += "MA_CUAHANG = " + cls_Main.SQLStringUnicode(item.ma_cuahang) + ", ";
                sSQL += "GHICHU = " + cls_Main.SQLStringUnicode(item.ghichu) + ", ";
                sSQL += "SUDUNG = " + cls_Main.SQLBit(bool.Parse(item.sudung));
                sSQL += " WHERE MA_KHO = " + cls_Main.SQLStringUnicode(id);
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
                cls_Main.WriterLog(cls_Main.sFilePath, "KHO Edit : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        private void LoadData(string id)
        {
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "select K.*, C.TEN_CUAHANG from KHO K, CUAHANG C where K.MA_CUAHANG = C.MA_CUAHANG AND MA_KHO = " + cls_Main.SQLString(id);
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                item.ma_kho = dr["MA_KHO"].ToString();
                item.ten_kho = dr["TEN_KHO"].ToString();
                item.ma_cuahang = dr["MA_CUAHANG"].ToString();
                item.ten_cuahang = dr["TEN_CUAHANG"].ToString();
                item.ghichu = dr["GHICHU"].ToString();
                item.sudung = dr["SUDUNG"].ToString();
            }
            //Cuahang          
            sSQL = "Select * From CUAHANG";
            dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                Combobox Cbb = new Combobox();
                Cbb.Value = dr["MA_CUAHANG"].ToString();
                Cbb.Text = dr["TEN_CUAHANG"].ToString();
                if (item.ma_cuahang == dr["MA_CUAHANG"].ToString())
                {
                    Cbb.MacDinh = "1";
                }
                else
                {
                    Cbb.MacDinh = "0";
                }
                Combobox.Add(Cbb);
            }
        }
    }
}
