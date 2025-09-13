using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using WA_Kingpos.Data;

namespace WA_Kingpos.Pages.Nghiepvu.KhaiBaoNhienLieuQuocChanh
{
    public class ListModel : PageModel
    {
        [BindProperty]
        public string sTungayDat { get; set; } = "";
        [BindProperty]
        public string sDenngayDat { get; set; } = "";
        public bool bThem { get; set; }
        public bool bSua { get; set; }
        public bool bXoa { get; set; }
        public List<DOITUONG> lstDoiTuong = new List<DOITUONG>();
        public IActionResult OnGet(string sTungayDat, string sDenNgay)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowView("2023121302", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                LoadData(sTungayDat, sDenNgay);
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "khaibaosudungnguyenlieu List: " , ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        public IActionResult OnPost(string submitButton)
        {
            try
            {                
                    return RedirectToPage("List", new { sTungayDat = sTungayDat.Substring(0, 10), sDenNgay = sTungayDat.Substring(13, 10)});   
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "PDFPhieuDau List : ", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        private void LoadData(string sTungayDat, string sDenNgay)
        {
            if (!string.IsNullOrEmpty(sTungayDat))
            {
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string sSQL = " ";
                sSQL += "EXEC SP_DS_PHIEUDAU " + cls_Main.SQLString(sTungayDat) + "," + cls_Main.SQLString(sDenNgay) + "\n";
                DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        DOITUONG tau = new DOITUONG();
                        tau.ID = dr["ID"].ToString();
                        tau.NAME = dr["TENDOITUONG"].ToString();
                        tau.SOCHUYEN = int.Parse(dr["SOCHUYEN"].ToString());
                        tau.DAUTON = int.Parse(dr["DAUTON"].ToString());
                        tau.CAPTHEM = int.Parse(dr["CAPTHEM"].ToString());
                        tau.TIEUTHU = int.Parse(dr["TIEUTHU"].ToString());
                        tau.TIEUTHUMAYCHINH = int.Parse(dr["TIEUTHUMAYCHINH"].ToString());
                        tau.TIEUTHUTUNGCHUYEN = int.Parse(dr["TIEUTHUTUNGCHUYEN"].ToString());
                        tau.TRADAU = int.Parse(dr["TRADAU"].ToString());
                        tau.CONLAI = int.Parse(dr["CONLAI"].ToString());
                        tau.GHICHU = dr["GHICHU"].ToString();
                        tau.NGAY = dr["NGAY1"].ToString();
                        //tau.TRANGTHAI = int.Parse(dr["SUDUNG"].ToString());
                        lstDoiTuong.Add(tau);
                    }

                }
            }
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
        public string? NGAY { get; set; }
        public int? TRANGTHAI { get; set; }

    }
}
