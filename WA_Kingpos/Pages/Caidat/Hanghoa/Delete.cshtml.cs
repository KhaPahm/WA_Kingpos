using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Runtime.Intrinsics.X86;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.Hanghoa
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        public clsHanghoa item = new clsHanghoa();
        public IActionResult OnGet(string id)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowDelete("6", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string sSQL = "";
                sSQL += "SELECT MA_HANGHOA,ID_HANGHOA,TEN_HANGHOA,H.STT,H.GHICHU,H.MAVACH,TEN_DONVITINH,H.MA_DONVITINH,";
                sSQL += "H.SUDUNG,H.SUAGIA,H.HINHANH,H.MA_NHOMHANG,TEN_NHOMHANG,THUE,GIANHAP,GIABAN1,GIABAN2,GIABAN3,GIABAN4,TONKHO,H.IS_INBEP,";
                sSQL += "THUCDON,TONTOITHIEU,H.SOLUONG,H.MA_BEP,B.TEN_BEP,H.MA_THANHPHAN,(SELECT ISNULL(TEN_HANGHOA,'') FROM HANGHOA WHERE MA_HANGHOA= H.MA_THANHPHAN) AS TEN_THANHPHAN,ISNULL(SUADINHLUONG,0) AS SUADINHLUONG,H.GIATHEOTRONGLUONG,H.PLU, H.HANSUDUNG, H.MONTHEM,H.INTEM" + "\n";
                sSQL += "FROM DONVITINH D,HANGHOA H,NHOMHANG N,DM_BEP B \n";
                sSQL += "WHERE H.MA_NHOMHANG=N.MA_NHOMHANG and H.MA_DONVITINH=D.MA_DONVITINH and H.MA_BEP=B.MA_BEP \n";
                sSQL += "AND MA_HANGHOA=" + cls_Main.SQLString(id) + "\n";
                DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                foreach (DataRow dr in dt.Rows)
                {
                    item.MA = dr["MA_HANGHOA"].ToString();
                    item.ID_HANGHOA = dr["ID_HANGHOA"].ToString();
                    item.TEN = dr["TEN_HANGHOA"].ToString();
                    item.TEN_DONVITINH = dr["TEN_DONVITINH"].ToString();
                    item.TEN_NHOMHANG = dr["TEN_NHOMHANG"].ToString();
                    item.MAVACH = dr["MAVACH"].ToString();
                    item.THUE = dr["THUE"].ToString();
                    item.GIANHAP = dr["GIANHAP"].ToString();
                    item.GIABAN1 = dr["GIABAN1"].ToString();
                    item.GIABAN2 = dr["GIABAN2"].ToString();
                    item.GIABAN3 = dr["GIABAN3"].ToString();
                    item.GIABAN4 = dr["GIABAN4"].ToString();
                    item.TONKHO = dr["TONKHO"].ToString();
                    item.TONTOITHIEU = dr["TONTOITHIEU"].ToString();
                    item.MA_THANHPHAN = dr["MA_THANHPHAN"].ToString();
                    item.TEN_THANHPHAN = dr["TEN_THANHPHAN"].ToString();
                    item.SOLUONG = dr["SOLUONG"].ToString();
                    item.IS_INBEP = dr["IS_INBEP"].ToString();
                    item.TEN_BEP = dr["TEN_BEP"].ToString();
                    item.GHICHU = dr["GHICHU"].ToString();
                    item.SUDUNG = dr["SUDUNG"].ToString();
                    item.THUCDON = dr["THUCDON"].ToString();
                    item.GIATHEOTRONGLUONG = dr["GIATHEOTRONGLUONG"].ToString();
                    item.PLU = dr["PLU"].ToString();
                    item.HANSUDUNG = dr["HANSUDUNG"].ToString();
                    item.SUAGIA = dr["SUAGIA"].ToString();
                    item.SUADINHLUONG = dr["SUADINHLUONG"].ToString();
                    item.MONTHEM = dr["MONTHEM"].ToString();
                    item.INTEM = dr["INTEM"].ToString();
                    item.STT = dr["STT"].ToString();
                    item.HINHANH = dr["HINHANH"] == DBNull.Value ? null : (byte[])dr["HINHANH"];
                }
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "Hanghoa Delete : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        public IActionResult Onpost(string id)
        {
            try
            {
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string sSQL = "";
                sSQL += "Delete From HANGHOA" + "\n";
                sSQL += "Where MA_HANGHOA=" + cls_Main.SQLString(id) + "\n";
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
                cls_Main.WriterLog(cls_Main.sFilePath, "Hanghoa Delete : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
    }
}
