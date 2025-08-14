using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using WA_Kingpos.Data;

namespace WA_Kingpos.Pages.LaiTau
{
    [Authorize]
    public class ViewModel : PageModel
    {
        public int sTrangThai = 0; //Trạng thái 0 không hiển thị cho nhập biển số xe, 1 cho nhập hiển thị biển số xe

        public Item item = new Item();
        public IActionResult OnGet(string id)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowView("2023121301", HttpContext.Session.GetString("Permission")))
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
                sSQL += "AND LICHTRINH_BCSL.MALICHTRINH=" + cls_Main.SQLString(id) + "\n";
                DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                foreach (DataRow dr in dt.Rows)
                {
                    item.MALICHTRINH = dr["MALICHTRINH"].ToString();
                    item.TENTUYEN = dr["TENTUYEN"].ToString();
                    item.NGAYDI = ((DateTime)dr["NGAYDI"]).ToString("dd/MM/yyyy");
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
                    item.VEMOI = dr["VEMOI"].ToString();
                    item.GHICHU = dr["GHICHU"].ToString();
                    item.OT4C = dr["SLOTO4C"].ToString();
                    item.OT7C = dr["SLOTO7C"].ToString();
                    item.OT16C = dr["SLOTO16C"].ToString();
                    item.OT25C = dr["SLOTO25C"].ToString();
                    item.OT50C = dr["SLOTO50C"].ToString();
                    item.SLXETAI = dr["SLXETAI"].ToString();
                    item.XETAIDUOI3T = dr["SLXETAI_DUOI3T"].ToString();
                    item.XETAIDUOI5T = dr["SLXETAI_DUOI5T"].ToString();
                    item.XETAIDUOI8T = dr["SLXETAI_DUOI8T"].ToString();
                    item.XETAITREN8T = dr["SLXETAI_TREN8T"].ToString();
                    item.XETAIVEMOI = dr["SLXETAI_VEMOI"].ToString();
                    item.XETAIGHICHU = dr["SLXETAI_GHICHU"].ToString();
                }

                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "LaiTau View : " + id, ex.ToString(), "0");
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
            public string VEMOI { get; set; }
            public string GHICHU { get; set; }
            public string OT4C { get; set; }
            public string OT7C { get; set; }
            public string OT16C { get; set; } 
            public string OT25C { get; set; }
            public string OT50C { get; set; }
            public string SLXETAI { get; set; }
            public string XETAIDUOI3T { get; set; }
            public string XETAIDUOI5T { get; set; }
            public string XETAIDUOI8T { get; set; }
            public string XETAITREN8T { get; set; }
            public string XETAIVEMOI {  get; set; }
            public string XETAIGHICHU { get; set; }
        }
    }
}
