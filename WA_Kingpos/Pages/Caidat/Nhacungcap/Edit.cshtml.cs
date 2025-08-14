using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Runtime.Intrinsics.X86;
using WA_Kingpos.Data;
using WA_Kingpos.Models;
using static WA_Kingpos.Pages.Nhacungcap.CreateModel;

namespace WA_Kingpos.Pages.Nhacungcap
{
    [Authorize]
    public class EditModel : PageModel
    {
        public clsNhacungcap item = new clsNhacungcap();
        public List<CAPDAILY> ListCAPDAILY { set; get; } = new List<CAPDAILY>();
        public List<LOAIDAILY> ListLOAIDAILY { set; get; } = new List<LOAIDAILY>();
        public IActionResult OnGet(string id)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowEdit("9", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                LoadData(id);
                
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "NHACUNGCAP Edit", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        public IActionResult OnPost(string id, clsNhacungcap item)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    LoadData(id);
                    return Page();
                }
                if (item.ID == "NK00000000")
                {
                    ViewData["Message"] = string.Format("Thông tin mặc định, không thể sửa!");
                    LoadData(id);
                    return Page();
                }
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string sSQL = "";
                sSQL += "Update NHACUNGCAP Set " + "\n";
                sSQL += "LOAI=" + cls_Main.SQLString(item.LOAI.ToString()) + ",";
                sSQL += "TEN=" + cls_Main.SQLStringUnicode(item.TEN) + "," + "\n";
                sSQL += "DIACHI=" + cls_Main.SQLStringUnicode(item.DIACHI) + "," + "\n";
                sSQL += "DIENTHOAI=" + cls_Main.SQLString(item.DIENTHOAI) + "," + "\n";
                sSQL += "FAX=" + cls_Main.SQLString(item.FAX) + "," + "\n";
                sSQL += "EMAIL=" + cls_Main.SQLString(item.EMAIL) + "," + "\n";
                sSQL += "WEBSITE=" + cls_Main.SQLString(item.WEBSITE) + "," + "\n";
                sSQL += "NGUOILIENHE=" + cls_Main.SQLStringUnicode(item.NGUOILIENHE) + "," + "\n";
                sSQL += "GHICHU=" + cls_Main.SQLStringUnicode(item.GHICHU) + "," + "\n";
                sSQL += "CMND=" + cls_Main.SQLString(item.CMND) + "," + "\n";
                sSQL += "NGAYSINH=" + cls_Main.SQLString(item.NGAYSINH) + "," + "\n";
                sSQL += "LOAIDAILY=" + cls_Main.SQLString(item.LOAIDAILY.ToString()) + "," + "\n";
                sSQL += "CAPDO=" + cls_Main.SQLString(item.CAPDO.ToString()) + "," + "\n";
                sSQL += "GIOITINH=" + cls_Main.SQLString(item.GIOITINH.ToString()) + "," + "\n";

                sSQL += "HANMUC_CONGNO=" + cls_Main.SQLString(item.HANMUC_CONGNO.Replace(",", "")) + "," + "\n";
                sSQL += "HANTHANHTOAN=" + cls_Main.SQLString(item.HANTHANHTOAN.Replace(",", "")) + "," + "\n";
                sSQL += "ISDATCOC=" + cls_Main.SQLBit(bool.Parse (item.ISDATCOC)) + "," + "\n";
                sSQL += "TIENDATCOC=" + cls_Main.SQLString(item.TIENDATCOC.Replace(",", "")) + "," + "\n";


                sSQL += "SUDUNG=" + cls_Main.SQLBit(bool.Parse (item.SUDUNG)) + "\n";
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
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "NHACUNGCAP Edit : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        private void LoadData(string id)
        {
            //NHACUNGCAP
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
            //CAPDAILY
            sSQL = "Select * From CAPDODAILY";
            dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                CAPDAILY capdaily = new CAPDAILY();
                capdaily.MA = dr["MA"].ToString();
                capdaily.TEN = dr["TEN"].ToString();
                ListCAPDAILY.Add(capdaily);
            }
            //LOAIDAILY
            sSQL = "Select * From LOAIDAILY";
            dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                LOAIDAILY loaidaily = new LOAIDAILY();
                loaidaily.MA = dr["MA"].ToString();
                loaidaily.TEN = dr["TEN"].ToString();
                ListLOAIDAILY.Add(loaidaily);
            }
        }
       
    }
}
