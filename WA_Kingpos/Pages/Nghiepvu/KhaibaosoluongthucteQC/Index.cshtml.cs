using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data;
using WA_Kingpos.Data;

namespace WA_Kingpos.Pages.LaiTau
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public int sTrangThai = 0; //Trạng thái 0 không hiển thị cho nhập biển số xe, 1 cho nhập hiển thị biển số xe
        public List<Item> listitem = new List<Item>();
        public List<Item> listitem_tuyen = new List<Item>();

        [BindProperty]
        [Required(ErrorMessage = "Phải chọn tuyến để khai báo")]
        public int MATUYEN { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Phải chọn chuyến tàu để khai báo")]
        public int MALICHTRINH { get; set; }

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
        [Required(ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0")]
        public int VEMOI { get; set; }

        [BindProperty]
        public string? GHICHU { get; set; }

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
                LoadData(id);
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "LaiTau Get", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        public IActionResult OnPost(string submitButton)
        {
            try
            {
                if(string.IsNullOrEmpty(submitButton))
                {
                    submitButton = "btn1ReLoad";
                }
                if(submitButton == "btn1ReLoad")
                {
                    return RedirectToPage("Index", new { id = MATUYEN });
                }
                if (submitButton == "btn2Create")
                {
                    if (VEMOI > 0 && String.IsNullOrEmpty(GHICHU))
                    {
                        ModelState.AddModelError("GHICHU", "Cần nhập ghi chú vé mời");
                    }

                    //var result2 = ModelState.SelectMany(m => m.Value.Errors.Select(s => new { m.Key, s.ErrorMessage }));

                    if (!ModelState.IsValid)
                    {
                        LoadData(string.Empty);
                        return Page();
                    }
                    else
                    {
                        string sNguoitao = User.FindFirst("Username")?.Value;
                        string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                        string sSQL = "";
                        sSQL += "Update LICHTRINH_BCSL Set SUDUNG=0 Where  MALICHTRINH=" + cls_Main.SQLString(MALICHTRINH.ToString()) + "\n";
                        sSQL += "Insert into LICHTRINH_BCSL(MALICHTRINH,NGUOITAO,NGAYTAO,TREEM," +
                            "CAOTUOI,VIP,NGUOILON,GANMAY,SUDUNG, VEMOI, GHICHU, SLOTO4C, " +
                            "SLOTO7C,SLOTO16C,SLOTO25C, SLOTO50C, SLXETAI, SLXETAI_DUOI3T, SLXETAI_DUOI5T," +
                            "SLXETAI_DUOI8T, SLXETAI_TREN8T, SLXETAI_VEMOI, SLXETAI_GHICHU)" + "\n";
                        sSQL += "Values ( ";
                        sSQL += cls_Main.SQLString(MALICHTRINH.ToString()) + ",";
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
                            return RedirectToPage("View", new { id = MALICHTRINH });
                        }
                        else
                        {
                            return RedirectToPage("/ErrorPage", new { id = "Lưu không thành công : " + sSQL });
                        }
                    }
                }
                LoadData(string.Empty);
                return Page();

            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "LaiTau Add", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        private void LoadData(string id)
        {
            if(string.IsNullOrEmpty(id)) 
            { 
                id="0"; 
            }
            //lichtrinh
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "Exec SP_GetLichTauTheoNgay_BCSL " + cls_Main.SQLString(DateTime.Now.ToString("dd/MM/yyyy")) + ","+cls_Main .SQLString ("0");
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                Item item = new Item();
                item.ID = dr["ID"].ToString();
                item.ID_Trip = dr["ID_Trip"].ToString();
                item.Name_Trip = dr["Name_Trip"].ToString();
                item.HourToGo = dr["HourToGo"].ToString();
                item.Vessel = dr["Vessel"].ToString();
                item.Note = dr["HourToGo"].ToString() + " - " + dr["Vessel"].ToString();
                if (item.ID_Trip == id)
                {
                    listitem.Add(item);
                }
            }
            //tuyen
            DataView view = new DataView(dt);
            DataTable dt_tuyen = view.ToTable("dt_tuyen", true, "ID_Trip", "Name_Trip");
            foreach (DataRow dr in dt_tuyen.Rows)
            {
                Item item = new Item();
                item.ID_Trip = dr["ID_Trip"].ToString();
                item.Name_Trip = dr["Name_Trip"].ToString();
                listitem_tuyen.Add(item);
            }
            //matuyen
            if (!string.IsNullOrEmpty(id))
            {
                MATUYEN = int.Parse(id);
            }
        }
        public class Item
        {
            public string? ID { get; set; }
            public string? DayToGo { get; set; }
            public string? DayOfArrival { get; set; }
            public string? HourToGo { get; set; }
            public string? HourOfArrival { get; set; }
            public string? ID_Trip { get; set; }
            public string? Name_Trip { get; set; }
            public string? Lock { get; set; }
            public string? Vessel { get; set; }
            public string? Note { get; set; }
        }
    }
}
