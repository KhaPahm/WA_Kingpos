using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.KHACHANG
{
    [Authorize]
    public class KhachHangEditModel : PageModel
    {
        public KhachHangModels item = new KhachHangModels();
        public IActionResult OnGet(string id)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowEdit("23120801", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                LoadData(id);
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "KhachHangFina Edit : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        public IActionResult OnPost(string id, KhachHangModels item)
        {
            try
            {
                if (item.ID == "")
                {
                    ModelState.AddModelError(string.Empty, "ID Khách Hàng Không Được Trống");
                }
                if (!CheckMST(item))
                {
                    ModelState.AddModelError("item.FAX", "Mã Số Thuế Này Đã Tồn Tại");
                }
                if (!ModelState.IsValid)
                {
                    LoadData(id);
                    return Page();
                }
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string sSQL = "";
                sSQL += "Update NHACUNGCAP Set " + "\n";
                sSQL += "LOAI=" + cls_Main.SQLString("0") + "," + "\n";
                sSQL += "TEN=" + cls_Main.SQLStringUnicode(item.TEN) + "," + "\n";
                sSQL += "DIACHI=" + cls_Main.SQLStringUnicode(item.DIACHI) + "," + "\n";
                sSQL += "DIENTHOAI=" + cls_Main.SQLString(item.DIENTHOAI) + "," + "\n";
                sSQL += "FAX=" + cls_Main.SQLString(item.FAX ?? "") + "," + "\n";
                sSQL += "EMAIL=" + cls_Main.SQLString(item.EMAIL ?? "") + "," + "\n";
                sSQL += "CMND=" + cls_Main.SQLString(item.CMND ?? "") + "," + "\n";
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
                cls_Main.WriterLog(cls_Main.sFilePath, "KhachHangFina Edit : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        private void LoadData(string id)
        {
            //khachhang
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string manhanvien = User.FindFirst("UserId")?.Value;
            string sSQL = "EXEC SP_FINA_SELECT_DATA @LOAI=17, @MANHANVIEN=" + cls_Main.SQLString(id);
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                item.ID = dr["ID"].ToString();
                item.MA = dr["MA"].ToString();
                item.TEN = dr["TEN"].ToString();
                item.DIACHI = dr["DIACHI"].ToString();
                item.DIENTHOAI = dr["DIENTHOAI"].ToString();
                item.FAX = dr["FAX"].ToString();
                item.EMAIL = dr["EMAIL"].ToString();
                item.CMND = dr["CMND"].ToString();
                item.SUDUNG = dr["SUDUNG"].ToString();
            }
        }
        private bool CheckMST(KhachHangModels KH)
        {
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "";
            if (!string.IsNullOrEmpty(KH.FAX))
            {
                if (string.IsNullOrEmpty(KH.MA))
                {
                    sSQL += "select FAX from  NHACUNGCAP  where FAX=" + cls_Main.SQLString(KH.FAX);
                }
                else
                {
                    sSQL += "select FAX from  NHACUNGCAP  where FAX=" + cls_Main.SQLString(KH.FAX) + " and MA<>" + cls_Main.SQLString(KH.MA);

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
