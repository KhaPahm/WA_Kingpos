using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static WA_Kingpos.Pages.Quay.EditModel;
using WA_Kingpos.Models;
using Microsoft.AspNetCore.Authorization;
using WA_Kingpos.Data;
using System.Data;

namespace WA_Kingpos.Pages.Quay
{
    [Authorize]
    public class EditModel : PageModel
    {
        public clsQuay item = new clsQuay();
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
                cls_Main.WriterLog(cls_Main.sFilePath, "QUAY", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }

        public IActionResult OnPost(string id, clsQuay item)
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
                sSQL += "Update QUAY set TEN_QUAY = " + cls_Main.SQLStringUnicode(item.ten_quay) + ", ";
                sSQL += "MA_KHO = " + cls_Main.SQLStringUnicode(item.ma_kho) + ", ";
                sSQL += "GHICHU = " + cls_Main.SQLStringUnicode(item.ghichu) + ", ";
                sSQL += "SUDUNG = " + cls_Main.SQLBit(bool.Parse(item.sudung));
                sSQL += " WHERE MA_QUAY = " + cls_Main.SQLStringUnicode(id);
                bool bRunSQL = cls_Main.ExecuteSQL(sSQL, sConnectionString_live);
                if (bRunSQL)
                {
                    return RedirectToPage("Index");
                }
                else
                {
                    return RedirectToPage("/ErrorPage", new { id = "Lýu không thành công : " + sSQL });
                }
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "QUAY Edit : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        private void LoadData(string id)
        {
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "select Q.*, K.TEN_KHO from QUAY Q, KHO K where K.MA_KHO = Q.MA_KHO AND MA_QUAY =" + cls_Main.SQLString(id);
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                item.ma_quay = dr["MA_QUAY"].ToString();
                item.ten_quay = dr["TEN_QUAY"].ToString();
                item.ma_kho = dr["MA_KHO"].ToString();
                item.ten_kho = dr["TEN_KHO"].ToString();
                item.ghichu = dr["GHICHU"].ToString();
                item.sudung = dr["SUDUNG"].ToString();
            }
            //chuc vu             
            sSQL = "Select * From KHO";
            dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                Combobox Cbb = new Combobox();
                Cbb.Value = dr["MA_KHO"].ToString();
                Cbb.Text = dr["TEN_KHO"].ToString();
                if (item.ma_kho == dr["MA_KHO"].ToString())
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
