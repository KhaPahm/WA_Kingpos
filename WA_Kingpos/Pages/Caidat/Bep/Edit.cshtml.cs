using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Runtime.Intrinsics.X86;
using WA_Kingpos.Data;
using WA_Kingpos.Models;
using static WA_Kingpos.Pages.Bep.CreateModel;
using static WA_Kingpos.Pages.Nhanvien.EditModel;

namespace WA_Kingpos.Pages.Bep
{
    [Authorize]
    public class EditModel : PageModel
    {
        public clsBep item = new clsBep();
        public List<CreateModel.CUAHANG> ListCuahang { set; get; } = new List<CreateModel.CUAHANG>();
        public IActionResult OnGet(string id)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowEdit("8", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                LoadData(id);
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "DM_Bep Edit : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        public IActionResult OnPost(string id, clsBep item)
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
                sSQL += "Update DM_BEP Set " + "\n";
                sSQL += "MA_CUAHANG=" + cls_Main.SQLString(item.ma_cuahang) + "," + "\n";
                sSQL += "TEN_BEP=" + cls_Main.SQLStringUnicode(item.ten_bep) + "," + "\n";
                sSQL += "MAYINBEP=" + cls_Main.SQLStringUnicode(item.mayinbep) + "," + "\n";
                sSQL += "IP=" + cls_Main.SQLStringUnicode(item.ip) + "," + "\n";
                sSQL += "GHICHU=" + cls_Main.SQLStringUnicode(item.ghichu) + "," + "\n";
                sSQL += "SUDUNG=" + cls_Main.SQLBit(bool.Parse (item.sudung)) + "\n";
                sSQL += "Where MA_BEP=" + cls_Main.SQLString(id) + "\n";
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
                cls_Main.WriterLog(cls_Main.sFilePath, "DM_Bep Edit : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        private void LoadData(string id)
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
            //bep
            sSQL = "SELECT DM_BEP.*,CUAHANG.TEN_CUAHANG FROM DM_BEP INNER JOIN CUAHANG ON DM_BEP.MA_CUAHANG = CUAHANG.MA_CUAHANG WHERE MA_BEP=" + cls_Main.SQLString(id);
            dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                item.ma_bep = dr["MA_BEP"].ToString();
                item.ten_bep = dr["TEN_BEP"].ToString();
                item.mayinbep = dr["MAYINBEP"].ToString();
                item.ip = dr["IP"].ToString();
                item.ghichu = dr["GHICHU"].ToString();
                item.sudung = dr["SUDUNG"].ToString();
                item.ma_cuahang = dr["MA_CUAHANG"].ToString();
                item.ten_cuahang = dr["TEN_CUAHANG"].ToString();
            }
        }

    }
}
