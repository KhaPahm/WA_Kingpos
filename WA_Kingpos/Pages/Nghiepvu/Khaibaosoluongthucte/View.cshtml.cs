using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.Thuyentruong
{
    [Authorize]
    public class ViewModel : PageModel
    {
        public Item item = new Item();
        public IActionResult OnGet(string id)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowView("2023112201", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string sSQL = " ";
                sSQL += "SELECT DM_TUYEN.TENTUYEN,LICHTRINH.NGAYDI,LICHTRINH.GIODI,DM_DOITUONG.TENDOITUONG, LICHTRINH_BCSL.* " + "\n";
                sSQL += "FROM  LICHTRINH_BCSL " + "\n";
                sSQL += "INNER JOIN LICHTRINH ON LICHTRINH_BCSL.MALICHTRINH=LICHTRINH .MALICHTRINH  " + "\n";
                sSQL += "INNER JOIN DM_TUYEN ON DM_TUYEN .MATUYEN = LICHTRINH .MATUYEN  " + "\n";
                sSQL += "INNER JOIN DM_DOITUONG ON DM_DOITUONG .MADOITUONG = LICHTRINH .MADOITUONG  " + "\n";
                sSQL += "WHERE LICHTRINH_BCSL.SUDUNG=1 " + "\n";
                sSQL += "AND LICHTRINH_BCSL.MALICHTRINH=" +cls_Main .SQLString (id)+ "\n";
                DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                foreach (DataRow dr in dt.Rows)
                {
                    item.MALICHTRINH = dr["MALICHTRINH"].ToString();
                    item.TENTUYEN = dr["TENTUYEN"].ToString();
                    item.NGAYDI = ((DateTime) dr["NGAYDI"]).ToString("dd/MM/yyyy");
                    item.GIODI = dr["GIODI"].ToString();
                    item.TENDOITUONG = dr["TENDOITUONG"].ToString();
                    item.NGUOITAO = dr["NGUOITAO"].ToString();
                    item.NGAYTAO = ((DateTime)dr["NGAYTAO"]).ToString("dd/MM/yyyy HH:mm");
                    item.TREEM = dr["TREEM"].ToString();
                    item.CAOTUOI = dr["CAOTUOI"].ToString();
                    item.VIP = dr["VIP"].ToString();
                    item.NGUOILON = dr["NGUOILON"].ToString();
                    item.GANMAY = dr["GANMAY"].ToString();
                    item.OTO4C = dr["OTO4C"].ToString();
                    item.OTO7C = dr["OTO7C"].ToString();
                    item.OTO16C = dr["OTO16C"].ToString();
                    item.OTO25C = dr["OTO25C"].ToString();
                    item.OTO50C = dr["OTO50C"].ToString();
                    item.XETAI = dr["XETAI"].ToString();
                    item.XETAIDUOI3T = dr["SLXETAI_DUOI3T"].ToString();
                    item.XETAIDUOI5T = dr["SLXETAI_DUOI5T"].ToString();
                    item.XETAIDUOI8T = dr["SLXETAI_DUOI8T"].ToString();
                    item.XETAITREN8T = dr["SLXETAI_TREN8T"].ToString();
                    item.XETAIVEMOI = dr["SLXETAI_VEMOI"].ToString();
                    item.XETAIGHICHU = dr["SLXETAI_GHICHU"].ToString();
                }
                List<string> list = new List<string>();
                if(item.OTO4C!="")
                {
                    list = item.OTO4C.Split(new char[] { '\n' }).ToList();
                    item.iOTO4C = list.Count.ToString();
                }
                if (item.OTO7C != "")
                {
                    list = item.OTO7C.Split(new char[] { '\n' }).ToList();
                    item.iOTO7C = list.Count.ToString();
                }
                if (item.OTO16C != "")
                {
                    list = item.OTO16C.Split(new char[] { '\n' }).ToList();
                    item.iOTO16C = list.Count.ToString();
                }
                if (item.OTO25C != "")
                {
                    list = item.OTO25C.Split(new char[] { '\n' }).ToList();
                    item.iOTO25C = list.Count.ToString();
                }
                if (item.OTO50C != "")
                {
                    list = item.OTO50C.Split(new char[] { '\n' }).ToList();
                    item.iOTO50C = list.Count.ToString();
                }
                if (item.XETAI != "")
                {
                    list = item.XETAI.Split(new char[] { '\n' }).ToList();
                    item.iXETAI = list.Count.ToString();
                }
               

                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "Thuyentruong View : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        public class Item
        {
            public string MALICHTRINH { get; set; }
            public string TENTUYEN { get; set; }
            public string NGAYDI { get; set; }
            public string GIODI { get; set; }
            public string TENDOITUONG { get; set; }
            public string NGUOITAO { get; set; }
            public string NGAYTAO { get; set; }
            public string TREEM { get; set; }
            public string CAOTUOI { get; set; }
            public string VIP { get; set; }
            public string VEMOI { get; set; }
            public string NGUOILON { get; set; }
            public string GANMAY { get; set; }
            public string OTO4C { get; set; }
            public string OTO7C { get; set; }
            public string OTO16C { get; set; }
            public string OTO25C { get; set; }
            public string OTO50C { get; set; }
            public string XETAI { get; set; }
            public string iOTO4C { get; set; }
            public string iOTO7C { get; set; }
            public string iOTO16C { get; set; }
            public string iOTO25C { get; set; }
            public string iOTO50C { get; set; }
            public string iXETAI { get; set; }
            public string XETAIDUOI3T { get; set; }
            public string XETAIDUOI5T { get; set; }
            public string XETAIDUOI8T { get; set; }
            public string XETAITREN8T { get; set; }
            public string XETAIVEMOI { get; set; }
            public string XETAIGHICHU { get; set; }
        }

    }
}
