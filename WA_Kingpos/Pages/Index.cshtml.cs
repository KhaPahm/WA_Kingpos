using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using WA_Kingpos.Data;

namespace WA_Kingpos.Pages
{
    public class IndexModel : PageModel
    {
        public List<Thuyentruong.ViewModel.Item> listitemSoluongthucte = new List<Thuyentruong.ViewModel.Item>();
        public List<Nghiepvu.Khaibaonhienlieu.ViewModel.DOITUONG> listitemNhienlieu { get; set; } = new List<Nghiepvu.Khaibaonhienlieu.ViewModel.DOITUONG>();

        public IActionResult OnGet()
        {
            try
            {
                // Verification.  
                if (this.User.Identity.IsAuthenticated)
                {
                    //xu ly trang
                    //Load cau hinh chung
                    cls_ConfigCashier.LoadCauHinh();
                    LoadData_ConfigReport();
                    LoadData_Thanhthoi();
                    // Home Page.
                    return Page();
                }
                else
                {
                    return RedirectToPage("Login");
                }
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "Main", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        private void LoadData_ConfigReport()
        {
            //load tiêu đề báo cáo
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "SELECT * FROM SYS_CONFIGREPORT  ";
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                cls_ConfigCashier.sTENCONGTY = dr["TENCONGTY"].ToString();
                cls_ConfigCashier.sDIACHICONGTY = dr["DIACHI"].ToString();
                cls_ConfigCashier.sSODIENTHOAICONGTY = dr["SODIENTHOAI"].ToString();
            }
        }
        private void LoadData_Thanhthoi()
        {
            //load danh sach phieu khai bao so luong
            if (cls_UserManagement.AllowView("2023112201", HttpContext.Session.GetString("Permission")) & cls_AppSettings.GetValue["Config:RunMode"].ToString() == "vetau")
            {
                string nguoitao = User.FindFirst("Username")?.Value;
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string sSQL = "Exec GetLICHTRINH_BCSL " + cls_Main.SQLString(nguoitao);
                DataTable dt1 = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                foreach (DataRow dr in dt1.Rows)
                {
                    Thuyentruong.ViewModel.Item item = new Thuyentruong.ViewModel.Item();
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
                    item.XETAIDUOI3T = dr["SLXETAI_DUOI3T"].ToString();
                    item.XETAIDUOI5T = dr["SLXETAI_DUOI5T"].ToString();
                    item.XETAIDUOI8T = dr["SLXETAI_DUOI8T"].ToString();
                    item.XETAITREN8T = dr["SLXETAI_TREN8T"].ToString();
                    item.XETAIVEMOI = dr["SLXETAI_VEMOI"].ToString();
                    item.XETAIGHICHU = dr["SLXETAI_GHICHU"].ToString();

                    List<string> list = new List<string>();
                    if (item.OTO4C != "")
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
                    if (item.XETAIDUOI3T != "")
                    {
                        list = item.XETAIDUOI3T.Split(new char[] { '\n' }).ToList();
                        item.XETAIDUOI3T = list.Count.ToString();
                    }
                    if (item.XETAIDUOI5T != "")
                    {
                        list = item.XETAIDUOI5T.Split(new char[] { '\n' }).ToList();
                        item.XETAIDUOI5T = list.Count.ToString();
                    }
                    if (item.XETAIDUOI8T != "")
                    {
                        list = item.XETAIDUOI8T.Split(new char[] { '\n' }).ToList();
                        item.XETAIDUOI8T = list.Count.ToString();
                    }
                    if (item.XETAIVEMOI != "")
                    {
                        list = item.XETAIVEMOI.Split(new char[] { '\n' }).ToList();
                        item.XETAIVEMOI = list.Count.ToString();
                    }
                    if (item.XETAIGHICHU != "")
                    {
                        list = item.XETAIGHICHU?.Split(new char[] { '\n' }).ToList();
                        item.XETAIGHICHU = list.Count.ToString();
                    }

                    listitemSoluongthucte.Add(item);

                }
            }
            //load danh sach phieu khai bao so luong QC
            if (cls_UserManagement.AllowView("2023121301", HttpContext.Session.GetString("Permission")) & cls_AppSettings.GetValue["Config:RunMode"].ToString() == "vetau")
            {
                string nguoitao = User.FindFirst("Username")?.Value;
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string sSQL = "Exec GetLICHTRINH_BCSL " + cls_Main.SQLString(nguoitao);
                DataTable dt1 = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                foreach (DataRow dr in dt1.Rows)
                {
                    Thuyentruong.ViewModel.Item item = new Thuyentruong.ViewModel.Item();
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
                    item.iOTO4C = dr["SLOTO4C"].ToString();
                    item.iOTO7C = dr["SLOTO7C"].ToString();
                    item.iOTO16C = dr["SLOTO16C"].ToString();
                    item.iOTO25C = dr["SLOTO25C"].ToString();
                    item.iOTO50C = dr["SLOTO50C"].ToString();
                    item.iXETAI = dr["SLXETAI"].ToString();
                    item.VEMOI = dr["VEMOI"].ToString();
                    item.XETAIDUOI3T = dr["SLXETAI_DUOI3T"].ToString();
                    item.XETAIDUOI5T = dr["SLXETAI_DUOI5T"].ToString();
                    item.XETAIDUOI8T = dr["SLXETAI_DUOI8T"].ToString();
                    item.XETAITREN8T = dr["SLXETAI_TREN8T"].ToString();
                    item.XETAIVEMOI = dr["SLXETAI_VEMOI"].ToString();
                    item.XETAIGHICHU = dr["SLXETAI_GHICHU"].ToString();
                    listitemSoluongthucte.Add(item);
                }


            }
            //load danh sach phieu khai nhien lieu
            if (cls_UserManagement.AllowView("24030608", HttpContext.Session.GetString("Permission")) & cls_AppSettings.GetValue["Config:RunMode"].ToString() == "vetau")
            {
                string nguoitao = User.FindFirst("Username")?.Value;
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string sSQL = "";
                sSQL += "select NL.*, DT.TENDOITUONG from DM_DOITUONG_BCNL NL, DM_DOITUONG DT where NL.MADOITUONG = DT.MADOITUONG and NL.SUDUNG = 1 AND CONVERT (DATE,NL.NGAY )= CONVERT (DATE,GETDATE()) and NL.NGUOITAO= " + cls_Main.SQLString(nguoitao) + " Order by NL.NGAYTAO DESC ";
                DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        Nghiepvu.Khaibaonhienlieu.ViewModel.DOITUONG DT = new Nghiepvu.Khaibaonhienlieu.ViewModel.DOITUONG();
                        DT.ID = dr["ID"].ToString();
                        DT.NAME = dr["TENDOITUONG"].ToString();
                        DT.SOCHUYEN = int.Parse(dr["SOCHUYEN"].ToString());
                        DT.DAUTON = int.Parse(dr["DAUTON"].ToString());
                        DT.CAPTHEM = int.Parse(dr["CAPTHEM"].ToString());
                        DT.TONGDAU = int.Parse(dr["TONGDAU"].ToString());
                        DT.TIEUTHU = int.Parse(dr["TIEUTHU"].ToString());
                        DT.CONLAI = int.Parse(dr["CONLAI"].ToString());
                        listitemNhienlieu.Add(DT);
                    }
                }
            }
        }

    }
}