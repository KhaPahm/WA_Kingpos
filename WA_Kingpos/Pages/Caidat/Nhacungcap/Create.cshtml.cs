using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualBasic;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;
using static WA_Kingpos.Pages.Index2Model;

namespace WA_Kingpos.Pages.Nhacungcap
{
    [Authorize]
    public class CreateModel : PageModel
    {
        [BindProperty]        
        public List<CAPDAILY> ListCAPDAILY { set; get; } = new List<CAPDAILY>();
        public List<LOAIDAILY> ListLOAIDAILY { set; get; } = new List<LOAIDAILY>();
        public clsNhacungcap item { get; set; }
        public string sIDKH { get; set; }
        public IActionResult OnGet()
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowAdd("9", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                LoadData();

                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "NHACUNGCAP Add", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        public IActionResult OnPost(clsNhacungcap item)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    //xu ly trang
                    LoadData();
                    return Page();
                }
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string sSQL = "";
                sSQL = "";
                sSQL = "Select * From NHACUNGCAP Where ID=" + cls_Main.SQLString(item.ID);
                DataTable dtCheck = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                if (dtCheck.Rows.Count > 0)
                {
                    ViewData["Message"] = string.Format("Mã đại lý đã tồn tại , cần nhập mã khác");
                    LoadData();
                    return Page();
                }
                sSQL = "";
                sSQL += "Insert into NHACUNGCAP (ID,LOAI,TEN,DIACHI,DIENTHOAI,FAX,EMAIL,WEBSITE,NGUOILIENHE,GHICHU,CMND,NGAYSINH,GIOITINH,LOAIDAILY,CAPDO,TIENDATCOC,ISDATCOC,HANTHANHTOAN,HANMUC_CONGNO,SUDUNG)" + "\n";
                sSQL += "Values ( ";
                sSQL += cls_Main.SQLString(item.ID.ToString()) + ",";
                sSQL += cls_Main.SQLString(item.LOAI.ToString()) + ",";
                sSQL += cls_Main.SQLStringUnicode(item.TEN) + ",";
                sSQL += cls_Main.SQLStringUnicode(item.DIACHI) + ",";
                sSQL += cls_Main.SQLString(item.DIENTHOAI) + ",";
                sSQL += cls_Main.SQLString(item.FAX) + ",";
                sSQL += cls_Main.SQLString(item.EMAIL) + ",";
                sSQL += cls_Main.SQLString(item.WEBSITE) + ",";
                sSQL += cls_Main.SQLStringUnicode(item.NGUOILIENHE) + ",";
                sSQL += cls_Main.SQLStringUnicode(item.GHICHU) + ",";
                sSQL += cls_Main.SQLString(item.CMND) + ",";
                sSQL += cls_Main.SQLString(item.NGAYSINH) + ",";
                sSQL += cls_Main.SQLString(item.GIOITINH.ToString()) + ",";
                sSQL += cls_Main.SQLString(item.LOAIDAILY.ToString()) + ",";
                sSQL += cls_Main.SQLString(item.CAPDO.ToString()) + ",";

                sSQL += cls_Main.SQLString(item.TIENDATCOC.Replace(",", "")) + ",";
                sSQL += cls_Main.SQLBit(bool.Parse (item.ISDATCOC)) + ",";
                sSQL += cls_Main.SQLString(item.HANTHANHTOAN.Replace(",", "")) + ",";
                sSQL += cls_Main.SQLString(item.HANMUC_CONGNO.Replace(",", "")) + ",";

                sSQL += cls_Main.SQLBit(bool.Parse(item.SUDUNG)) + ")";
                bool bRunSQL = cls_Main.ExecuteSQL(sSQL, sConnectionString_live);
                if (bRunSQL)
                {
                    TempData["AlertMessage"] = "Lưu thành công";
                    TempData["AlertType"] = "alert-success";
                    return RedirectToPage("Create");
                }
                else
                {
                    return RedirectToPage("/ErrorPage", new { id = "Lưu không thành công : " + sSQL });
                }
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "NHACUNGCAP Add", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        private void LoadData()
        {
            //CAPDAILY
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "Select * From CAPDODAILY";
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
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
            //MADAILY
            sSQL = "select [dbo].[fc_NewCodeID_NHACUNGCAP]  ()";
            dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            if (dt.Rows.Count > 0)
            {
                sIDKH = dt.Rows[0][0].ToString();
            }
        }
        public class CAPDAILY
        {
            public string MA { get; set; }
            public string TEN { get; set; }
        }
        public class LOAIDAILY
        {
            public string MA { get; set; }
            public string TEN { get; set; }
        }
    }
}
