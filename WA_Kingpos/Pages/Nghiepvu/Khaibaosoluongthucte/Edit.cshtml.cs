using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data;
using WA_Kingpos.Data;

namespace WA_Kingpos.Pages.Thuyentruong
{
    [Authorize]
    public class EditModel : PageModel
    {
        public Item item = new Item();

        [BindProperty]
        [Required(ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0")]
        public int TE { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0")]
        public int CT { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0")]
        public int VIP { get; set; }

        [BindProperty]
        [Range(1, 5000)]
        [Required(ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0")]
        public int NL { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0")]
        public int GM { get; set; }

        [BindProperty]
        public List<string> OTO4C { get; set; }=new List<string>();

        [BindProperty]
        public List<string> OTO7C { get; set; } = new List<string>();

        [BindProperty]
        public List<string> OTO16C { get; set; } = new List<string>();

        [BindProperty]
        public List<string> OTO25C { get; set; } = new List<string>();

        [BindProperty]
        public List<string> OTO50C { get; set; } = new List<string>();

        [BindProperty]
        public List<string> XT { get; set; } = new List<string>();
        public IActionResult OnGet(string id)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowEdit("2023112201", HttpContext.Session.GetString("Permission")))
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
                }
                TE = int.Parse (item.TREEM);
                CT = int.Parse(item.CAOTUOI);
                VIP = int.Parse(item.VIP);
                NL = int.Parse(item.NGUOILON);
                GM = int.Parse(item.GANMAY);
                for (int i = 0; i < 100; i++)
                {
                    OTO4C.Add("");
                    OTO7C.Add("");
                    OTO16C.Add("");
                    OTO25C.Add("");
                    OTO50C.Add("");
                    XT.Add("");
                }
                List<string> list = new List<string>();
                list = item.OTO4C.Split(new char[] { '\n' }).ToList();
                for (int i = 0; i < list.Count; i++)
                {
                    OTO4C[i] = list[i];
                }
                list = item.OTO7C.Split(new char[] { '\n' }).ToList();
                for (int i = 0; i < list.Count; i++)
                {
                    OTO7C[i] = list[i];
                }
                list = item.OTO16C.Split(new char[] { '\n' }).ToList();
                for (int i = 0; i < list.Count; i++)
                {
                    OTO16C[i] = list[i];
                }
                list = item.OTO25C.Split(new char[] { '\n' }).ToList();
                for (int i = 0; i < list.Count; i++)
                {
                    OTO25C[i] = list[i];
                }
                list = item.OTO50C.Split(new char[] { '\n' }).ToList();
                for (int i = 0; i < list.Count; i++)
                {
                    OTO50C[i] = list[i];
                }
                list = item.XETAI.Split(new char[] { '\n' }).ToList();
                for (int i = 0; i < list.Count; i++)
                {
                    XT[i] = list[i];
                }
                
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "Thuyentruong Edit : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }

        public IActionResult OnPost(string id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }
                else
                {
                    string sNguoitao = User.FindFirst("Username")?.Value;
                    string sOTO4C = "";
                    string sOTO7C = "";
                    string sOTO16C = "";
                    string sOTO25C = "";
                    string sOTO50C = "";
                    string sXT = "";
                    foreach (string dr in OTO4C)
                    {
                        if (!String.IsNullOrEmpty(dr))
                        {
                            sOTO4C += dr.Replace(" ", "") + "\n";
                        }
                    }
                    foreach (string dr in OTO7C)
                    {
                        if (!String.IsNullOrEmpty(dr))
                        {
                            sOTO7C += dr.Replace(" ", "") + "\n";
                        }
                    }
                    foreach (string dr in OTO16C)
                    {
                        if (!String.IsNullOrEmpty(dr))
                        {
                            sOTO16C += dr.Replace(" ", "") + "\n";
                        }
                    }
                    foreach (string dr in OTO25C)
                    {
                        if (!String.IsNullOrEmpty(dr))
                        {
                            sOTO25C += dr.Replace(" ", "") + "\n";
                        }
                    }
                    foreach (string dr in OTO50C)
                    {
                        if (!String.IsNullOrEmpty(dr))
                        {
                            sOTO50C += dr.Replace(" ", "") + "\n";
                        }
                    }
                    foreach (string dr in XT)
                    {
                        if (!String.IsNullOrEmpty(dr))
                        {
                            sXT += dr.Replace(" ", "") + "\n";
                        }
                    }

                    string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                    string sSQL = "";
                    sSQL += "Update LICHTRINH_BCSL Set SUDUNG=0 Where  MALICHTRINH=" + cls_Main.SQLString(id) + "\n";
                    sSQL += "Insert into LICHTRINH_BCSL(MALICHTRINH,NGUOITAO,NGAYTAO,TREEM,CAOTUOI,VIP,NGUOILON,GANMAY,OTO4C,OTO7C,OTO16C,OTO25C,OTO50C,XETAI,SUDUNG)" + "\n";
                    sSQL += "Values ( ";
                    sSQL += cls_Main.SQLString(id) + ",";
                    sSQL += cls_Main.SQLString(sNguoitao) + ",";
                    sSQL += "GETDATE()" + ",";
                    sSQL += cls_Main.SQLString(TE.ToString()) + ",";
                    sSQL += cls_Main.SQLString(CT.ToString()) + ",";
                    sSQL += cls_Main.SQLString(VIP.ToString()) + ",";
                    sSQL += cls_Main.SQLString(NL.ToString()) + ",";
                    sSQL += cls_Main.SQLString(GM.ToString()) + ",";
                    sSQL += cls_Main.SQLString(sOTO4C.ToString()) + ",";
                    sSQL += cls_Main.SQLString(sOTO7C.ToString()) + ",";
                    sSQL += cls_Main.SQLString(sOTO16C.ToString()) + ",";
                    sSQL += cls_Main.SQLString(sOTO25C.ToString()) + ",";
                    sSQL += cls_Main.SQLString(sOTO50C.ToString()) + ",";
                    sSQL += cls_Main.SQLString(sXT.ToString()) + ",";
                    sSQL += cls_Main.SQLString("1") + ")";
                    bool bRunSQL = cls_Main.ExecuteSQL(sSQL, sConnectionString_live);
                    if (bRunSQL)
                    {
                        return RedirectToPage("View", new { id = id });
                    }
                    else
                    {
                        return RedirectToPage("/ErrorPage", new { id = "Lưu không thành công : " + sSQL });
                    }
                }
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "Thuyentruong Edit", ex.ToString(), "0");
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
        }
    }
}
