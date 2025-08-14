using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.TaiXe
{
    [Authorize]
    public class TaiXeEditModel : PageModel
    {
        public TaiXeModels item = new TaiXeModels();
        public IActionResult OnGet(string id)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowEdit("23120802", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                LoadData(id);
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "TaiXeFina Edit : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        public IActionResult OnPost(string id, TaiXeModels item)
        {
            try
            {
                if (item.ID == "")
                {
                    ModelState.AddModelError(string.Empty, "ID Khách Hàng Không Được Trống");
                }
                if (!CheckCMND(item))
                {
                    ModelState.AddModelError("item.CMND", "CMND Này Đã Tồn Tại");
                }
                if (!ModelState.IsValid)
                {
                    LoadData(id);
                    return Page();
                }
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string sSQL = "";
                sSQL += "Update NHACUNGCAP Set " + "\n";
                sSQL += "LOAI=" + cls_Main.SQLString("1") + ",";
                sSQL += "TEN=" + cls_Main.SQLStringUnicode(item.TEN) + "," + "\n";
                sSQL += "DIACHI=" + cls_Main.SQLStringUnicode("") + "," + "\n";
                sSQL += "DIENTHOAI=" + cls_Main.SQLString(item.DIENTHOAI) + "," + "\n";
                sSQL += "FAX=" + cls_Main.SQLString("") + "," + "\n";
                sSQL += "EMAIL=" + cls_Main.SQLString("") + "," + "\n";
                sSQL += "WEBSITE=" + cls_Main.SQLString(item.WEBSITE) + "," + "\n";
                sSQL += "NGUOILIENHE=" + cls_Main.SQLStringUnicode("") + "," + "\n";
                sSQL += "GHICHU=" + cls_Main.SQLStringUnicode("") + "," + "\n";
                sSQL += "CMND=" + cls_Main.SQLString(item.CMND ?? "") + "," + "\n";
                sSQL += "NGAYSINH=" + cls_Main.SQLString(String.Format("{0:yyyyMMdd}", DateTime.Now)) + "," + "\n";
                sSQL += "LOAIDAILY=" + cls_Main.SQLString("1") + "," + "\n";
                sSQL += "CAPDO=" + cls_Main.SQLString("1") + "," + "\n";
                sSQL += "GIOITINH=" + cls_Main.SQLBit(true) + "," + "\n";

                sSQL += "HANMUC_CONGNO=" + cls_Main.SQLString("0") + "," + "\n";
                sSQL += "HANTHANHTOAN=" + cls_Main.SQLString("0") + "," + "\n";
                sSQL += "ISDATCOC=" + cls_Main.SQLBit(false) + "," + "\n";
                sSQL += "TIENDATCOC=" + cls_Main.SQLString("0") + "," + "\n";
                sSQL += "LastUpdate=GETDATE()" + "," + "\n";

                sSQL += "SUDUNG=" + cls_Main.SQLBit(bool.Parse(item.SUDUNG)) + "\n";
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
                cls_Main.WriterLog(cls_Main.sFilePath, "TaiXeFina Edit : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        private void LoadData(string id)
        {
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "EXEC SP_FINA_SELECT_DATA @LOAI=18, @MANHANVIEN=" + cls_Main.SQLString(id);
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                item.MA = dr["MA"].ToString();
                item.ID = dr["ID"].ToString();
                item.TEN = dr["TEN"].ToString();
                item.DIENTHOAI = dr["DIENTHOAI"].ToString();
                item.CMND = dr["CMND"].ToString();
                item.WEBSITE = dr["WEBSITE"].ToString();
                item.SUDUNG = dr["SUDUNG"].ToString();
            }
        }
        private bool CheckCMND(TaiXeModels TX)
        {
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "";
            if (!string.IsNullOrEmpty(TX.CMND))
            {
                if (string.IsNullOrEmpty(TX.MA))
                {
                    sSQL += "select CMND from  NHACUNGCAP  where CMND=" + cls_Main.SQLString(TX.CMND);
                }
                else
                {
                    sSQL += "select CMND from  NHACUNGCAP  where CMND=" + cls_Main.SQLString(TX.CMND) + " and MA<>" + cls_Main.SQLString(TX.MA);

                }
                DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                if (dt.Rows.Count > 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
