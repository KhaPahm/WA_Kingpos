
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data;
using WA_Kingpos.Data;

namespace WA_Kingpos.Pages.Nghiepvu.KhaiBaoNhienLieuQuocChanh
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


        public string? name { get; set; }

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
                    

                    string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                    string sSQL = "";
                    sSQL += "update DM_DOITUONG_BCNL SET ";
                    sSQL += " DAUTON = " + cls_Main.SQLString(DAUTON.ToString()) + ",";
                    sSQL += " CAPTHEM = " + cls_Main.SQLString(DAUTHEM.ToString()) + ",";
                    sSQL += " TONGDAU = " + cls_Main.SQLString(TONGDAU.ToString()) + ",";
                    sSQL += " TIEUTHU = " + cls_Main.SQLString(TONGTIEUTHU.ToString()) + ",";
                    sSQL += " CONLAI = " + cls_Main.SQLString((TONCUOI).ToString()) + ",";
                    sSQL += " TIEUTHUMAYCHINH = " + cls_Main.SQLString(TIEUTHUMAYCHINH.ToString()) + ",";
                    sSQL += " TIEUTHUTUNGCHUYEN = " + cls_Main.SQLString(TIEUTHUTUNGCHUYEN.ToString()) + ",";
                    sSQL += " TRADAU = " + cls_Main.SQLString(TRADAU.ToString()) + ",";
                    sSQL += " GHICHU = " + cls_Main.SQLStringUnicode(GHICHU.ToString()) + ",";
                    sSQL += " NGUOISUA = " + cls_Main.SQLString(sNguoitao) + ",";
                    sSQL += " NGAYSUA = GETDATE() where ID = " + id;
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
                cls_Main.WriterLog(cls_Main.sFilePath, "khaibaonguyenlieu Edit", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        private void LoadData(string id)
        {
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
                    TONGTIEUTHU = int.Parse(dr["TIEUTHU"].ToString());
                    TIEUTHUMAYCHINH = int.Parse(dr["TIEUTHUMAYCHINH"].ToString());
                    TIEUTHUTUNGCHUYEN = int.Parse(dr["TIEUTHUTUNGCHUYEN"].ToString());
                    TRADAU = int.Parse(dr["TRADAU"].ToString());
                    GHICHU = dr["GHICHU"].ToString();
                    TONCUOI = int.Parse(dr["CONLAI"].ToString());
                }
                
            }

        }
        public class DOITUONG
        {
            public string? ID { get; set; }
            public string? NAME { get; set; }
        }

        
    }
}