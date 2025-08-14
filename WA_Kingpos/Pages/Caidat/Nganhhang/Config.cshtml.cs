using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.Nganhhang
{
    [Authorize]
    public class ConfigModel : PageModel
    {
        public clsNganhhang item = new clsNganhhang();

        [BindProperty]
        public List<clsNhomhang> ListNhomhang { set; get; } = new List<clsNhomhang>();
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
                cls_Main.WriterLog(cls_Main.sFilePath, "Nganhhang Config : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        public IActionResult OnPost(string id, clsNganhhang item, List<clsNhomhang> ListNhomhang)
        {
            try
            {
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string sSQL = "";
                sSQL = "";
                sSQL += "Delete from CAUHINHKHUVUC where MAQUAY=" + cls_Main.SQLString(id) + " and MODE='20221112'"+"\n";
                foreach (clsNhomhang nh in ListNhomhang )
                {
                    if(bool.Parse (nh.CHON) )
                    {
                        sSQL += "Insert into CAUHINHKHUVUC (MAQUAY,MANHOM,MODE) values(" + cls_Main.SQLString(id) + "," + cls_Main.SQLString(nh.MA) + ",'20221112')" + "\n";
                    }
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
                cls_Main.WriterLog(cls_Main.sFilePath, "Nganhhang Config : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        private void LoadData(string id)
        {
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "select ID,VNValue,OrderBy from ReferenceList where Category='ProductType' and ID=" + cls_Main.SQLString(id);
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                item.ID = dr["ID"].ToString();
                item.VNValue = dr["VNValue"].ToString();
                item.OrderBy = dr["OrderBy"].ToString();
            }
            //cauhinhkhuvuc
            sSQL = "Select * from CAUHINHKHUVUC where MAQUAY=" + cls_Main.SQLString(id) + " and MODE='20221112'" + "\n";
            DataTable dtCauhinh = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            //Nhom hang
            sSQL = "Select MA_NHOMHANG,TEN_NHOMHANG From NHOMHANG";
            dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                clsNhomhang nhomhang = new clsNhomhang();
                nhomhang.MA = dr["MA_NHOMHANG"].ToString();
                nhomhang.TEN = dr["TEN_NHOMHANG"].ToString();
                DataRow[] drTim = dtCauhinh.Select("MANHOM=" + cls_Main.SQLString(dr["MA_NHOMHANG"].ToString()));
                if(drTim .Length ==0)
                {
                    nhomhang.CHON = "false";
                }
                else
                {
                    nhomhang.CHON = "true";
                }
                ListNhomhang.Add(nhomhang);
            }
        }
    }
}
