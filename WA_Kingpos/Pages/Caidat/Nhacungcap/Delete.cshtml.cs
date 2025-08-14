using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.Nhacungcap
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        public clsNhacungcap item = new clsNhacungcap();
        public IActionResult OnGet(string id)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowDelete("9", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                LoadData(id);
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "NHACUNGCAP Delete : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        public IActionResult Onpost(string id,clsNhacungcap item)
        {
            try
            {
                if (item.ID == "NK00000000")
                {
                    ViewData["Message"] = string.Format("Thông tin mặc định, không thể xóa!");
                    LoadData(id);
                    return Page();
                }
                else
                {
                    string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                    string sSQL = "";
                    sSQL += "Delete From NHACUNGCAP" + "\n";
                    sSQL += "Where MA=" + cls_Main.SQLString(id) + "\n";
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
                cls_Main.WriterLog(cls_Main.sFilePath, "NHACUNGCAP Delete : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        private void LoadData(string id)
        {
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "";
            sSQL += "Select MA,ID,TEN,LOAI,(Case When LOAI=1 Then N'Nhà cung cấp' Else N'Khách hàng' End) As TENLOAI,";
            sSQL += "ISNULL(HANMUC_CONGNO,0) AS HANMUC_CONGNO,ISNULL(HANTHANHTOAN,0) AS HANTHANHTOAN,";
            sSQL += "ISNULL(TIENDATCOC,0) AS TIENDATCOC,ISNULL(ISDATCOC,0) AS ISDATCOC,";
            sSQL += "DIACHI,DIENTHOAI,FAX,EMAIL,WEBSITE,NGUOILIENHE,GHICHU,SUDUNG,CMND,NGAYSINH,GIOITINH,LOAIDAILY,CAPDO,(SELECT A.TEN FROM LOAIDAILY A WHERE A.MA=LOAIDAILY)AS LOAIDAILY1,(SELECT A.TEN FROM CAPDODAILY A WHERE A.MA=CAPDO)AS CAPDAILY,(SELECT A.CHIETKHAU FROM CAPDODAILY A WHERE A.MA=CAPDO)AS CHIETKHAU" + "\n";
            sSQL += "From NHACUNGCAP" + "\n";
            sSQL += "Where MA=" + cls_Main.SQLString(id) + "\n";
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                item.MA = dr["MA"].ToString();
                item.ID = dr["ID"].ToString();
                item.TEN = dr["TEN"].ToString();
                item.LOAI = dr["LOAI"].ToString();
                item.TENLOAI = dr["TENLOAI"].ToString();
                item.HANMUC_CONGNO = dr["HANMUC_CONGNO"].ToString();
                item.HANTHANHTOAN = dr["HANTHANHTOAN"].ToString();
                item.TIENDATCOC = dr["TIENDATCOC"].ToString();
                item.ISDATCOC = dr["ISDATCOC"].ToString();
                item.DIACHI = dr["DIACHI"].ToString();
                item.DIENTHOAI = dr["DIENTHOAI"].ToString();
                item.FAX = dr["FAX"].ToString();
                item.EMAIL = dr["EMAIL"].ToString();
                item.WEBSITE = dr["WEBSITE"].ToString();
                item.NGUOILIENHE = dr["NGUOILIENHE"].ToString();
                item.GHICHU = dr["GHICHU"].ToString();
                item.SUDUNG = dr["SUDUNG"].ToString();
                item.CMND = dr["CMND"].ToString();
                item.NGAYSINH = ((DateTime)dr["NGAYSINH"]).ToString("dd/MM/yyyy");
                item.GIOITINH = dr["GIOITINH"].ToString();
                item.LOAIDAILY = dr["LOAIDAILY"].ToString();
                item.CAPDO = dr["CAPDO"].ToString();
                item.LOAIDAILY1 = dr["LOAIDAILY1"].ToString();
                item.CAPDAILY = dr["CAPDAILY"].ToString();
                item.CHIETKHAU = dr["CHIETKHAU"].ToString();
            }
        }
    }
}
