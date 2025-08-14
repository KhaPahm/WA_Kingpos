using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.Baocao.TraCuuKHTT
{
    public class IndexModel : PageModel
    {
        public List<KHTT> lstKHTT =new List<KHTT>();
        public IActionResult OnGet(string id)
        {
            try
            {
                int sodu = 0;
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string sSQL = "EXEC dbo.SP_SELECT_TTNB_KHTT " + cls_Main.SQLString(id);
                DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                foreach (DataRow dr in dt.Rows)
                {
                    sodu += int.Parse(dr["LoaiTien"].ToString());
                    KHTT kHTT = new KHTT();
                    kHTT.MaHD = dr["MaHD"].ToString();
                    kHTT.TenHD = dr["TenHD"].ToString();
                    kHTT.SoTien = dr["SoTien"].ToString();
                    kHTT.NgayNop = dr["NgayNop"].ToString();
                    kHTT.TEN_HANGHOA = dr["TEN_HANGHOA"].ToString();
                    kHTT.TEN_QUAY = dr["TEN_QUAY"].ToString();
                    kHTT.GIABAN_THAT = dr["GIABAN_THAT"].ToString();
                    kHTT.SOLUONG = dr["SOLUONG"].ToString();
                    kHTT.FLAG= dr["FLAG"].ToString();
                    kHTT.TongSoDu = dr["TongSoDu"].ToString();
                    kHTT.SoDu = sodu.ToString();

                    kHTT.TEN_CUAHANG= dr["TEN_CUAHANG"].ToString();
                    kHTT.Header= dr["Header"].ToString();
                    kHTT.Footer= dr["Footer"].ToString();
                    kHTT.Logo= dr["Logo"].ToString();
                    lstKHTT.Add(kHTT);
                }
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "TraCuuKHTT : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
    }
}
