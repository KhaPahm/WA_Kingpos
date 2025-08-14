using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Runtime.Intrinsics.X86;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.TaiXe
{
    [Authorize]
    public class TaixeDeleteModel : PageModel
    {
        public TaiXeModels item = new TaiXeModels();
        public IActionResult OnGet(string id)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowDelete("23120802", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                LoadData(id);
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "TaiXeFina Delete : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        public IActionResult Onpost(string id)
        {
            try
            {
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string sSQL = "";
                sSQL = "select top 1 * from PHIEUDATHANG where NGUOIGIAOHANG =" + cls_Main.SQLString(id);
                DataTable dtCheck = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                if (dtCheck.Rows.Count > 0)
                {
                    ViewData["Message"] = string.Format("Tài xế này đã tham gia chương trình không thể xóa");
                    LoadData(id);
                    return Page();
                }
                else
                {
                    sSQL = "Delete From NHACUNGCAP" + "\n";
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
                cls_Main.WriterLog(cls_Main.sFilePath, "TaiXeFina Delete : " + id, ex.ToString(), "0");
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
    }
}
