using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.LaiTau
{
    [Authorize]
    public class EditModel : PageModel
    {
        public int sTrangThai = 0; //Trạng thái 0 không hiển thị cho nhập biển số xe, 1 cho nhập hiển thị biển số xe
        
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
        public int VEMOI { get; set; }

        [BindProperty]
        public string? GHICHU { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0")]
        public int GM { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0")]
        public int OT4C { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0")]
        public int OT7C { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0")]
        public int OT16C { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0")]
        public int OT25C { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0")]
        public int OT50C { get; set; }

        //[BindProperty]
        //[Required(ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0")]
        public int XETAI { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0")]
        public int XETAIDUOI3T { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0")]
        public int XETAIDUOI5T { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0")]
        public int XETAIDUOI8T { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0")]
        public int XETAITREN8T { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0")]
        public int XETAIVEMOI { get; set; }

        [BindProperty]
        public string? XETAIGHICHU { get; set; }

        public Item item = new Item();

        public IActionResult OnGet(string id)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowEdit("2023121301", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                LoadData(id);
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "LaiTau Edit : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        public IActionResult OnPost(string id)
        {
            try
            {
                if (VEMOI > 0 && String.IsNullOrEmpty(GHICHU))
                {
                    ModelState.AddModelError("GHICHU", "Cần nhập ghi chú vé mời");
                }
                if (!ModelState.IsValid)
                {
                    LoadData(id);
                    return Page();
                }
                else
                {
                    string sNguoitao = User.FindFirst("Username")?.Value;
                    string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                    string sSQL = "";
                    sSQL += "Update LICHTRINH_BCSL Set SUDUNG=0 Where  MALICHTRINH=" + cls_Main.SQLString(id) + "\n";
                    sSQL += "Insert into LICHTRINH_BCSL(MALICHTRINH,NGUOITAO,NGAYTAO,TREEM,CAOTUOI,VIP,NGUOILON,GANMAY,SUDUNG, VEMOI, GHICHU, SLOTO4C, SLOTO7C,SLOTO16C,SLOTO25C, SLOTO50C, SLXETAI, SLXETAI_DUOI3T, SLXETAI_DUOI5T, SLXETAI_DUOI8T, SLXETAI_TREN8T, SLXETAI_VEMOI, SLXETAI_GHICHU)" + "\n";
                    sSQL += "Values ( ";
                    sSQL += cls_Main.SQLString(id) + ",";
                    sSQL += cls_Main.SQLString(sNguoitao) + ",";
                    sSQL += "GETDATE()" + ",";
                    sSQL += cls_Main.SQLString(TE.ToString()) + ",";
                    sSQL += cls_Main.SQLString(CT.ToString()) + ",";
                    sSQL += cls_Main.SQLString(VIP.ToString()) + ",";
                    sSQL += cls_Main.SQLString(NL.ToString()) + ",";
                    sSQL += cls_Main.SQLString(GM.ToString()) + ",";
                    sSQL += cls_Main.SQLString("1") + ",";
                    sSQL += cls_Main.SQLString(VEMOI.ToString()) + ",";
                    sSQL += cls_Main.SQLStringUnicode(GHICHU) + ",";
                    sSQL += cls_Main.SQLString(OT4C.ToString()) + ",";
                    sSQL += cls_Main.SQLString(OT7C.ToString()) + ",";
                    sSQL += cls_Main.SQLString(OT16C.ToString()) + ",";
                    sSQL += cls_Main.SQLString(OT25C.ToString()) + ",";
                    sSQL += cls_Main.SQLString(OT50C.ToString()) + ",";
                    sSQL += cls_Main.SQLString(XETAI.ToString()) + ",";
                    sSQL += cls_Main.SQLString(XETAIDUOI3T.ToString()) + ",";
                    sSQL += cls_Main.SQLString(XETAIDUOI5T.ToString()) + ",";
                    sSQL += cls_Main.SQLString(XETAIDUOI8T.ToString()) + ",";
                    sSQL += cls_Main.SQLString(XETAITREN8T.ToString()) + ",";
                    sSQL += cls_Main.SQLString(XETAIVEMOI.ToString()) + ",";
                    sSQL += cls_Main.SQLStringUnicode(XETAIGHICHU?.ToString()) + ")";
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
                cls_Main.WriterLog(cls_Main.sFilePath, "LaiTau Edit", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        private void LoadData(string id)
        {
            //lichtrinh
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
                item.SLOTO4C = dr["SLOTO4C"].ToString();
                item.SLOTO7C = dr["SLOTO7C"].ToString();
                item.SLOTO16C = dr["SLOTO16C"].ToString();
                item.SLOTO25C = dr["SLOTO25C"].ToString();
                item.SLOTO50C = dr["SLOTO50C"].ToString();
                item.SLXETAI = dr["SLXETAI"].ToString();
                item.XETAIDUOI3T = dr["SLXETAI_DUOI3T"].ToString();
                item.XETAIDUOI5T = dr["SLXETAI_DUOI5T"].ToString();
                item.XETAIDUOI8T = dr["SLXETAI_DUOI8T"].ToString();
                item.XETAITREN8T = dr["SLXETAI_TREN8T"].ToString();
                item.XETAIVEMOI = dr["SLXETAI_VEMOI"].ToString();
                item.XETAIGHICHU = dr["SLXETAI_GHICHU"].ToString();
            }
            TE = int.Parse(item.TREEM);
            CT = int.Parse(item.CAOTUOI);
            VIP = int.Parse(item.VIP);
            NL = int.Parse(item.NGUOILON);
            GM = int.Parse(item.GANMAY);
            VEMOI = int.Parse(item.VEMOI);
            GHICHU = item.GHICHU;
            OT4C = int.Parse(item.SLOTO4C);
            OT7C = int.Parse(item.SLOTO7C);
            OT16C = int.Parse(item.SLOTO16C);
            OT25C = int.Parse(item.SLOTO25C);
            OT50C = int.Parse(item.SLOTO50C);
            XETAI = int.Parse(item.SLXETAI);
            XETAIDUOI3T = int.Parse(item.XETAIDUOI3T);
            XETAIDUOI5T = int.Parse(item.XETAIDUOI5T);
            XETAIDUOI8T = int.Parse(item.XETAIDUOI8T);
            XETAITREN8T = int.Parse(item.XETAITREN8T);
            XETAIVEMOI = int.Parse(item.XETAIVEMOI);
            XETAIGHICHU = item.XETAIGHICHU;
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
            public string VEMOI { get; set; }
            public string GHICHU { get; set; }
            public string SLOTO4C { get; set; }
            public string SLOTO7C { get; set; }
            public string SLOTO16C { get; set; }
            public string SLOTO25C { get; set; }
            public string SLOTO50C { get; set; }
            public string SLXETAI { get; set; }
            public string XETAIDUOI3T { get; set; }
            public string XETAIDUOI5T { get; set; }
            public string XETAIDUOI8T { get; set; }
            public string XETAITREN8T { get; set; }
            public string XETAIVEMOI { get; set; }
            public string XETAIGHICHU { get; set; }
        }
    }
}
