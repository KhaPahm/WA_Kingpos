using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.Baocao.TraCuuQRCode
{
    public class IndexModel : PageModel
    {
        public List<QRCode> lstQr = new List<QRCode>();
        public int veDaDung = 0;
        public IActionResult OnGet(string id)
        {
            try
            {
               
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string sSQL = "EXEC dbo.APP_KP_GetItemBill " + cls_Main.SQLString(id);
                DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                foreach (DataRow dr in dt.Rows)
                {
                    QRCode qr = new QRCode();
                    qr.STT= dr["STT"].ToString();
                    qr.MaHangHoa = dr["MA_HANGHOA"].ToString();
                    qr.TenHangHoa = dr["TEN_HANGHOA"].ToString();
                    qr.SuDung = dr["SUDUNG"].ToString();
                    qr.NgaySuDung = dr["NGAYSUDUNG"].ToString();
                    qr.Code = dr["CODE"].ToString();
                    qr.MaCong = dr["MACONG"].ToString();
                    qr.TenCong = dr["TENCONG"].ToString();
                    qr.Logo= dr["Logo"].ToString();
                    qr.TenCuaHang = dr["TEN_CUAHANG"].ToString();
                    qr.Header = dr["Header"].ToString();
                    qr.Footer = dr["Footer"].ToString();
                    qr.TEXT7= dr["TEXT7"].ToString();
                    qr.TUNGAY = (DateTime)dr["TUNGAY"];
                    qr.DENNGAY = (DateTime)dr["DENNGAY"];
                    lstQr.Add(qr);
                    if (qr.SuDung.Trim().Equals("1"))
                    {
                        veDaDung++;
                    }    
                }
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "TraCuuQRcode : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
    }
}
