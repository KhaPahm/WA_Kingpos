
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using WA_Kingpos.Data;

namespace WA_Kingpos.Pages.Nghiepvu.KhaiBaoNhienLieuQuocChanh
{
    [Authorize]
    public class ViewModel : PageModel
    {
        public List<ITEM> LIST_ITEM { get; set; } = new List<ITEM>();
        public DOITUONG DT = new DOITUONG();

        public IActionResult OnGet(string id)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowView("2023121302", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string sSQL = " ";
                sSQL += "select NL.*, DT.TENDOITUONG from DM_DOITUONG_BCNL NL, DM_DOITUONG DT where NL.MADOITUONG = DT.MADOITUONG and NL.SUDUNG = 1 and NL.ID = " + cls_Main.SQLString(id) + "\n";
                DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        DT.ID = dr["ID"].ToString();
                        DT.NAME = dr["TENDOITUONG"].ToString();
                        DT.SOCHUYEN = int.Parse(dr["SOCHUYEN"].ToString());
                        DT.DAUTON = int.Parse(dr["DAUTON"].ToString());
                        DT.CAPTHEM = int.Parse(dr["CAPTHEM"].ToString());
                        DT.TONGDAU = int.Parse(dr["TONGDAU"].ToString());
                        DT.TIEUTHU = int.Parse(dr["TIEUTHU"].ToString());
                        DT.TIEUTHUMAYCHINH = int.Parse(dr["TIEUTHUMAYCHINH"].ToString());
                        DT.TIEUTHUTUNGCHUYEN = int.Parse(dr["TIEUTHUTUNGCHUYEN"].ToString());
                        DT.TRADAU = int.Parse(dr["TRADAU"].ToString());
                        DT.GHICHU = dr["GHICHU"].ToString();
                        DT.CONLAI= int.Parse(dr["CONLAI"].ToString());

                    }
                    
                }
                else
                {
                    return RedirectToPage("/Index");
                }

                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "Khaibaonguyenlieu View : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        public class DOITUONG
        {
            public string? ID { get; set; }
            public string? NAME { get; set; }
            public int? SOCHUYEN { get; set; }
            public int? DAUTON { get; set; }
            public int? CAPTHEM { get; set; }
            public int? TONGDAU { get; set; }
            public int? TIEUTHU { get; set; }
            public int? CONLAI { get; set; }
            public int? TIEUTHUMAYCHINH { get; set; }
            public int? TIEUTHUTUNGCHUYEN { get; set; }
            public int? TRADAU { get; set; }
            public string? GHICHU { get; set; }

        }

        public class ITEM
        {
            public string? MALICHTRINH { get; set; }
            public string? GIODI { get; set; }
            public double? VONGTUAMAY { get; set; }
            public double? DINHMUC { get; set; }
            public double? DAUMAYCHINH { get; set; }
            public double? DAUMAYDEN { get; set; }
            public double? DAUPHATSINH { get; set; }
            public double TONGTIEUTHU { get; set; }
            public double? DAUTON { get; set; }
            public string? GHICHU { get; set; }
        }
    }
}
