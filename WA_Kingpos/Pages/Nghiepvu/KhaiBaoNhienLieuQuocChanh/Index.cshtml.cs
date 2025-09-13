using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data;
using WA_Kingpos.Data;

namespace WA_Kingpos.Pages.Nghiepvu.KhaiBaoNhienLieuQuocChanh
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public List<DOITUONG> listitem = new List<DOITUONG>();


        [BindProperty]
        [Required(ErrorMessage = "Phải chọn tàu để khai báo")]
        public int TAU { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0")]
        public int SOLUONG { get; set; }

        [BindProperty]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0")]
        public int DAUTON { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0")]
        public int DAUTHEM { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0")]
        public int TONGDAU { get; set; }

        //[BindProperty]
       // public List<ITEM> LIST_ITEM { get; set; } = new List<ITEM>();

        [BindProperty]
        [Required(ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0")]
        public int TONGTIEUTHU { get; set; }
        [BindProperty]
        [Required(ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0")]
        public int TIEUTHUMAYCHINH { get; set; }
        [BindProperty]
        [Required(ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0")]
        public int TIEUTHUTUNGCHUYEN { get; set; }
        [BindProperty]
        [Required(ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0")]
        public int TRADAU { get; set; }
        [BindProperty]
        [Required(ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0")]
        public int TONCUOI { get; set; }
        [BindProperty]
        public string? GHICHU { get; set; }

        [BindProperty]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public string sTungay { get; set; } = DateTime.Now.ToString("dd/MM/yyyy");

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
                LoadData(id);
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "khaibaosudungnguyenlieu Get", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        public IActionResult OnPost(string submitButton)
        {
            try
            {
                if (string.IsNullOrEmpty(submitButton))
                {
                    submitButton = "btn1ReLoad";
                }
                if (submitButton == "btn1ReLoad")
                {
                    return RedirectToPage("Index", new { id = TAU });
                }
                if (submitButton == "btn1List")
                {
                    return RedirectToPage("List");
                }
                if (submitButton == "btn2Create")
                {
                    if (!ModelState.IsValid)
                    {
                        //xu ly trang
                        LoadData(string.Empty);
                        return Page();
                    }
                    else
                    {
                        string sNguoitao = User.FindFirst("Username")?.Value;                      
                        string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                        string sSQL = "";                                                
                        sSQL = "EXEC SP_INSERT_BCNL " + cls_Main.SQLString(TAU.ToString()) + ",";
                        sSQL += cls_Main.SQLString(sTungay.ToString()) + ",";
                        sSQL += cls_Main.SQLString(SOLUONG.ToString()) + ",";
                        sSQL += cls_Main.SQLString(DAUTON.ToString()) + ",";
                        sSQL += cls_Main.SQLString(DAUTHEM.ToString()) + ",";
                        sSQL += cls_Main.SQLString(TONGDAU.ToString()) + ",";
                        sSQL += cls_Main.SQLString(TONGTIEUTHU.ToString()) + ",";
                        sSQL += cls_Main.SQLString(TONCUOI.ToString()) + ",";
                        sSQL += cls_Main.SQLString(TIEUTHUMAYCHINH.ToString()) + ",";
                        sSQL += cls_Main.SQLString(TIEUTHUTUNGCHUYEN.ToString()) + ",";
                        sSQL += cls_Main.SQLString(TRADAU.ToString()) + ",";
                        sSQL += cls_Main.SQLStringUnicode(GHICHU) + ",";
                        sSQL += cls_Main.SQLString(sNguoitao) ;
                        string ID = "";
                        DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                        if (dt.Rows.Count>0)
                        {
                            //sSQL = "select Top 1 * from DM_DOITUONG_BCNL where SUDUNG=1 and MADOITUONG = " + cls_Main.SQLString(TAU.ToString()) + " and convert(varchar(20),NGAYTAO,103) = convert(varchar(20),GETDATE(),103) ORDER BY NGAYTAO DESC";
                            // dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                            ID = dt.Rows[0]["ID"].ToString();
                            return RedirectToPage("View", new { id = ID });
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
                cls_Main.WriterLog(cls_Main.sFilePath, "khaibaonguyenlieu Add", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        private void LoadData(string id)
        {
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string a = sTungay;
            string sSQL = "";           
            LoadTautheoNgay(sTungay);
            if (!string.IsNullOrEmpty(id))
            {
                TAU = int.Parse(id);
                sSQL = "EXEC SP_SOCHUYENTAU " + cls_Main.SQLString(TAU.ToString()) + " , " + cls_Main.SQLString(sTungay.ToString());
                DataTable dt_tuyen = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
              
                SOLUONG = int.Parse(dt_tuyen.Rows[0]["SOLUONG"].ToString());
            }

        }
        private void LoadTautheoNgay(string ngay)
        {
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "exec SP_GET_TAU_NHAPNL " + cls_Main.SQLString(ngay);
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                DOITUONG item = new DOITUONG();
                item.ID = dr["MADOITUONG"].ToString();
                item.NAME = dr["TENDOITUONG"].ToString();
                listitem.Add(item);
            }
        }

        public JsonResult OnGetTauTheoNgay(string ngay)
        {
            var listitem = new List<DOITUONG>();
            // Load dữ liệu theo ngày
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "exec SP_GET_TAU_NHAPNL " + cls_Main.SQLString(ngay);
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                listitem.Add(new DOITUONG
                {
                    ID = dr["MADOITUONG"].ToString(),
                    NAME = dr["TENDOITUONG"].ToString()
                });
            }

            return new JsonResult(listitem);
        }
        public JsonResult OnGetSoLuongChuyenTau(string idTau)
        {
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");            
            string sSQL = "EXEC SP_SOCHUYENTAU " + cls_Main.SQLString(idTau.ToString()) + " , " + cls_Main.SQLString(sTungay.ToString());
            DataTable dt_tuyen = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);

            SOLUONG =int.Parse(dt_tuyen.Rows[0]["SOLUONG"].ToString());
            return new JsonResult(SOLUONG);
        }    

        public class DOITUONG
        {
            public string? ID { get; set; }
            public string? NAME { get; set; }
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
