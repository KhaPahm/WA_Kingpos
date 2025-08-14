using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Intrinsics.X86;
using WA_Kingpos.Data;
using WA_Kingpos.Models;
using static WA_Kingpos.Pages.Bep.CreateModel;

namespace WA_Kingpos.Pages.Nhomhang
{
    [Authorize]
    public class EditInbepModel : PageModel
    {
        public clsNhomhang item = new clsNhomhang();
        public List<clsBep> ListBep = new List<clsBep>();
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
                cls_Main.WriterLog(cls_Main.sFilePath, "Nhomhang Edit : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        public IActionResult OnPost(string id, clsNhomhang item)
        {
            try
            {
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string sSQL = "";
                if (bool.Parse ( item.INBEP))
                {
                    sSQL += "Update HANGHOA Set" + "\n";
                    sSQL += "IS_INBEP=1,";
                    sSQL += "MA_BEP=" + cls_Main.SQLString(item.MABEP.ToString()) + " \n";
                    sSQL += "Where MA_NHOMHANG=" + cls_Main.SQLString(id);
                }
                else
                {
                    sSQL += "Update HANGHOA Set" + "\n";
                    sSQL += "IS_INBEP=0" + "\n";
                    sSQL += "Where MA_NHOMHANG=" + cls_Main.SQLString(id);
                }
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
                cls_Main.WriterLog(cls_Main.sFilePath, "Nhomhang Edit : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        private void LoadData(string id)
        {
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "";
            sSQL += "Select MA_NHOMHANG As MA,TEN_NHOMHANG As TEN,GHICHU,SUDUNG,THUCDON,STT,HINHANH,MONTHEM" + "\n";
            sSQL += "From NHOMHANG" + "\n";
            sSQL += "Where MA_NHOMHANG=" + cls_Main.SQLString(id) + "\n";
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                item.MA = dr["MA"].ToString();
                item.TEN = dr["TEN"].ToString();
                item.GHICHU = dr["GHICHU"].ToString();
                item.SUDUNG = dr["SUDUNG"].ToString();
                item.THUCDON = dr["THUCDON"].ToString();
                item.STT = dr["STT"].ToString();
                item.HINHANH = dr["HINHANH"] == DBNull.Value ? null : (byte[])dr["HINHANH"];
                item.MONTHEM = dr["MONTHEM"].ToString();
                item.INBEP = "true";
            }
            //bep
            sSQL = "SELECT DM_BEP.*,CUAHANG.TEN_CUAHANG FROM DM_BEP INNER JOIN CUAHANG ON DM_BEP.MA_CUAHANG = CUAHANG.MA_CUAHANG";
            dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                clsBep item = new clsBep();
                item.ma_bep = dr["MA_BEP"].ToString();
                item.ten_bep = dr["TEN_BEP"].ToString();
                item.mayinbep = dr["MAYINBEP"].ToString();
                item.ip = dr["IP"].ToString();
                item.ghichu = dr["GHICHU"].ToString();
                item.sudung = dr["SUDUNG"].ToString();
                item.ma_cuahang = dr["MA_CUAHANG"].ToString();
                item.ten_cuahang = dr["TEN_CUAHANG"].ToString();

                ListBep.Add(item);
            }

        }
    }
}
