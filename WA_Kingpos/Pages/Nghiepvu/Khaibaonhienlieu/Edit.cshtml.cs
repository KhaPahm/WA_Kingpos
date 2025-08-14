using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data;
using WA_Kingpos.Data;

namespace WA_Kingpos.Pages.Nghiepvu.Khaibaonhienlieu
{
    [Authorize]
    public class EditModel : PageModel
    {
        public List<DOITUONG> listitem = new List<DOITUONG>();


        [BindProperty]
        [Required(ErrorMessage = "Phải chọn tàu để khai báo")]
        public int TAU { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0")]
        public int SOLUONG { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0")]
        public int DAUTON { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0")]
        public int DAUTHEM { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0")]
        public int TONGDAU { get; set; }

        [BindProperty]
        public List<ITEM> LIST_ITEM { get; set; } = new List<ITEM>();

        public string? name { get; set; }

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
                cls_Main.WriterLog(cls_Main.sFilePath, "khaibaosudungnguyenlieu Edit: " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        public IActionResult OnPost(string id)
        {
            try
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
                    sSQL += "update DM_DOITUONG_BCNL SET ";
                    sSQL += " DAUTON = " + cls_Main.SQLString(DAUTON.ToString()) + ",";
                    sSQL += " CAPTHEM = " + cls_Main.SQLString(DAUTHEM.ToString()) + ",";
                    sSQL += " TONGDAU = " + cls_Main.SQLString(TONGDAU.ToString()) + ",";
                    sSQL += " TIEUTHU = " + cls_Main.SQLString(TIEUTHU.ToString()) + ",";
                    sSQL += " CONLAI = " + cls_Main.SQLString((TONGDAU - TIEUTHU).ToString()) + ",";
                    sSQL += " NGUOISUA = " + cls_Main.SQLString(sNguoitao) + ",";
                    sSQL += " NGAYSUA = GETDATE() where ID = " + id;
                    bool bRunSQL = cls_Main.ExecuteSQL(sSQL, sConnectionString_live);
                    if (bRunSQL)
                    {
                        sql1 = "";
                        foreach (ITEM dr in LIST_ITEM)
                        {
                            sql1 += "UPDATE DM_DOITUONG_BCNLCT SET ";
                            sql1 += "VONGTUAMAY = " + cls_Main.SQLString(dr.VONGTUAMAY.ToString()) + ",";
                            sql1 += "DINHMUC = " + cls_Main.SQLString(dr.DINHMUC.ToString()) + ",";
                            sql1 += "DAUMAYCHINH = " + cls_Main.SQLString(dr.DAUMAYCHINH.ToString()) + ",";
                            sql1 += "DAUMAYDEN = " + cls_Main.SQLString(dr.DAUMAYDEN.ToString()) + ",";
                            sql1 += "DAUPHATSINH = " + cls_Main.SQLString(dr.DAUPHATSINH.ToString()) + ",";
                            sql1 += "TONGTIEUTHU = " + cls_Main.SQLString(dr.TONGTIEUTHU.ToString()) + ",";
                            sql1 += "DAUTON = " + cls_Main.SQLString(dr.DAUTON.ToString()) + ",";
                            sql1 += "GHICHU = " + cls_Main.SQLStringUnicode(dr.GHICHU) + " where ID_BCSL = " + cls_Main.SQLString(dr.ID.ToString()) + " and MALICHTRINH = " + cls_Main.SQLString(dr.MALICHTRINH.ToString()) + "\n";
                        }

                        bRunSQL = cls_Main.ExecuteSQL(sql1, sConnectionString_live);
                        if (bRunSQL)
                        {
                            return RedirectToPage("View", new { id = id });
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
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "khaibaonguyenlieu Edit", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        private void LoadData(string id)
        {
            LIST_ITEM = new List<ITEM>();
            TAU = int.Parse(id);
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = " ";
            sSQL += "select NL.*, DT.TENDOITUONG from DM_DOITUONG_BCNL NL, DM_DOITUONG DT where NL.MADOITUONG = DT.MADOITUONG and NL.SUDUNG = 1 and NL.ID = " + cls_Main.SQLString(id) + "\n";
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    DOITUONG item = new DOITUONG();
                    item.ID = dr["MADOITUONG"].ToString();
                    item.NAME = dr["TENDOITUONG"].ToString();
                    listitem.Add(item);

                    name = dr["TENDOITUONG"].ToString();
                    TAU = int.Parse(dr["ID"].ToString());
                    SOLUONG = int.Parse(dr["SOCHUYEN"].ToString());
                    DAUTON = int.Parse(dr["DAUTON"].ToString());
                    DAUTHEM = int.Parse(dr["CAPTHEM"].ToString());
                    TONGDAU = int.Parse(dr["TONGDAU"].ToString());
                }
                sSQL = "select NL.*, LT.GIODI from DM_DOITUONG_BCNLCT NL, LICHTRINH LT where NL.MALICHTRINH = LT.MALICHTRINH and NL.ID_BCSL = " + cls_Main.SQLString(id) + "\n";
                DataTable dt_tuyen = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                foreach (DataRow dr in dt_tuyen.Rows)
                {
                    ITEM item = new ITEM();
                    item.ID = dr["ID_BCSL"].ToString();
                    item.MALICHTRINH = dr["MALICHTRINH"].ToString();
                    item.GIODI = dr["GIODI"].ToString();
                    item.VONGTUAMAY = double.Parse(dr["VONGTUAMAY"].ToString());
                    item.DINHMUC = double.Parse(dr["DINHMUC"].ToString());
                    item.DAUMAYCHINH = double.Parse(dr["DAUMAYCHINH"].ToString());
                    item.DAUMAYDEN = double.Parse(dr["DAUMAYDEN"].ToString());
                    item.DAUPHATSINH = double.Parse(dr["DAUPHATSINH"].ToString());
                    item.TONGTIEUTHU = double.Parse(dr["TONGTIEUTHU"].ToString());
                    item.DAUTON = double.Parse(dr["DAUTON"].ToString());
                    item.GHICHU = dr["GHICHU"].ToString();
                    LIST_ITEM.Add(item);
                }
            }

        }
        public class DOITUONG
        {
            public string? ID { get; set; }
            public string? NAME { get; set; }
        }

        public class ITEM
        {
            public string? ID { get; set; }
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