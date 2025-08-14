using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data;
using WA_Kingpos.Data;

namespace WA_Kingpos.Pages.Nghiepvu.Khaibaonhienlieu
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

        [BindProperty]
        public List<ITEM> LIST_ITEM { get; set; } = new List<ITEM>();

        public IActionResult OnGet(string id)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowView("24030608", HttpContext.Session.GetString("Permission")))
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
                        string sql1 = "";
                        double TIEUTHU = 0;
                        foreach (ITEM dr in LIST_ITEM)
                        {
                            TIEUTHU += dr.TONGTIEUTHU;
                        }

                        string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                        string sSQL = "";
                        sSQL += "insert DM_DOITUONG_BCNL(MADOITUONG, NGAY, SOCHUYEN, DAUTON, CAPTHEM, TONGDAU, TIEUTHU, CONLAI, NGUOITAO, NGAYTAO, SUDUNG)" + "\n";
                        sSQL += "Values ( ";
                        sSQL += cls_Main.SQLString(TAU.ToString()) + ",";
                        sSQL += "GETDATE()" + ",";
                        sSQL += cls_Main.SQLString(SOLUONG.ToString()) + ",";
                        sSQL += cls_Main.SQLString(DAUTON.ToString()) + ",";
                        sSQL += cls_Main.SQLString(DAUTHEM.ToString()) + ",";
                        sSQL += cls_Main.SQLString(TONGDAU.ToString()) + ",";
                        sSQL += cls_Main.SQLString(TIEUTHU.ToString()) + ",";
                        sSQL += cls_Main.SQLString((TONGDAU - TIEUTHU).ToString()) + ",";
                        sSQL += cls_Main.SQLString(sNguoitao) + ",";
                        sSQL += "GETDATE()" + ",";
                        sSQL += cls_Main.SQLString("1") + ")";
                        bool bRunSQL = cls_Main.ExecuteSQL(sSQL, sConnectionString_live);
                        string ID = "";
                        if (bRunSQL)
                        {
                            sSQL = "select Top 1 * from DM_DOITUONG_BCNL where SUDUNG=1 and MADOITUONG = " + cls_Main.SQLString(TAU.ToString()) + " and convert(varchar(20),NGAY,103) = convert(varchar(20),GETDATE(),103) ";
                            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                            ID = dt.Rows[0]["ID"].ToString();
                            sql1 = "";
                            foreach (ITEM dr in LIST_ITEM)
                            {
                                sql1 += "insert DM_DOITUONG_BCNLCT(ID_BCSL, MALICHTRINH, VONGTUAMAY, DINHMUC, DAUMAYCHINH, DAUMAYDEN, DAUPHATSINH, TONGTIEUTHU, DAUTON, GHICHU)" + "\n";
                                sql1 += "Values ( ";
                                sql1 += cls_Main.SQLString(dt.Rows[0]["ID"].ToString()) + ",";
                                sql1 += cls_Main.SQLString(dr.MALICHTRINH.ToString()) + ",";
                                sql1 += cls_Main.SQLString(dr.VONGTUAMAY.ToString()) + ",";
                                sql1 += cls_Main.SQLString(dr.DINHMUC.ToString()) + ",";
                                sql1 += cls_Main.SQLString(dr.DAUMAYCHINH.ToString()) + ",";
                                sql1 += cls_Main.SQLString(dr.DAUMAYDEN.ToString()) + ",";
                                sql1 += cls_Main.SQLString(dr.DAUPHATSINH.ToString()) + ",";
                                sql1 += cls_Main.SQLString(dr.TONGTIEUTHU.ToString()) + ",";
                                sql1 += cls_Main.SQLString(dr.DAUTON.ToString()) + ",";
                                sql1 += cls_Main.SQLStringUnicode(dr.GHICHU) + ")" + "\n";
                            }

                            bRunSQL = cls_Main.ExecuteSQL(sql1, sConnectionString_live);
                            if (bRunSQL)
                            {
                                return RedirectToPage("View", new { id = ID });
                            }
                            else
                            {
                                return RedirectToPage("/ErrorPage", new { id = "Lưu không thành công : " + sSQL });
                            }
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
            LIST_ITEM = new List<ITEM>();
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "select distinct DT.MADOITUONG, DT.TENDOITUONG from DM_DOITUONG DT, LICHTRINH LT where DT.MADOITUONG = LT.MADOITUONG and convert(varchar(20),LT.NGAYDI,103) = convert(varchar(20),GETDATE(),103) AND LT.SUDUNG =1 AND DT.MADOITUONG NOT IN (SELECT NL.MADOITUONG FROM DM_DOITUONG_BCNL NL WHERE convert(varchar(20),NL.NGAYTAO,103) = convert(varchar(20),GETDATE(),103) AND NL.SUDUNG = 1) Order by TENDOITUONG ";
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                DOITUONG item = new DOITUONG();
                item.ID = dr["MADOITUONG"].ToString();
                item.NAME = dr["TENDOITUONG"].ToString();
                listitem.Add(item);
            }
            if (!string.IsNullOrEmpty(id))
            {
                TAU = int.Parse(id);
                sSQL = "select MALICHTRINH, GIODI from LICHTRINH where SUDUNG =1 and MADOITUONG = " + TAU + "and convert(varchar(20),NGAYDI,103) = convert(varchar(20),GETDATE(),103) Order by GIODI";
                DataTable dt_tuyen = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                foreach (DataRow dr in dt_tuyen.Rows)
                {
                    ITEM item = new ITEM();
                    item.MALICHTRINH = dr["MALICHTRINH"].ToString();
                    item.GIODI = dr["GIODI"].ToString();
                    item.TONGTIEUTHU = 0;
                    LIST_ITEM.Add(item);
                }
                SOLUONG = dt_tuyen.Rows.Count;
            }

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