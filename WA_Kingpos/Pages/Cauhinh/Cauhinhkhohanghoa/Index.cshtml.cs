using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.Cauhinh.Cauhinhkhohanghoa
{
    [Authorize]
    public class ConfigModel : PageModel
    {
        [BindProperty]
        [Required(ErrorMessage = "Phải chọn kho")]
        public int MAKHO { get; set; }

        [BindProperty]
        public List<clsKho> listKho { get; set; } = new List<clsKho>();

        [BindProperty]
        public List<CauHinhKhoHangHoa> listHanghoa { get; set; } = new List<CauHinhKhoHangHoa>();
        public IActionResult OnGet(string id)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowAdd("35", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                LoadData(id);
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "Cauhinhkhohanghoa Config : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        public IActionResult OnPost(string submitButton, string id, List<CauHinhKhoHangHoa> listHanghoa)
        {
            try
            {
                if (string.IsNullOrEmpty(submitButton))
                {
                    submitButton = "btn1ReLoad";
                }
                if (submitButton == "btn1ReLoad")
                {
                    return RedirectToPage("Index", new { id = MAKHO });
                }
                if (submitButton == "btn2Save")
                {
                    string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                    string sSQL = "";
                    foreach (CauHinhKhoHangHoa pq in listHanghoa)
                    {
                        if (pq.SUDUNG == false)
                        {
                            sSQL += "DELETE FROM CAUHINHKHUVUC WHERE MAQUAY = " + id + " AND MANHOM = " + pq.MA_HANGHOA + " AND MODE = " + cls_Main.SQLString("20240326") + "\n";
                        }
                        if (pq.SUDUNG == true)
                        {
                            sSQL += "IF NOT EXISTS (SELECT 1 FROM CAUHINHKHUVUC WHERE MAQUAY = " + id + " AND MANHOM = " + "'" + pq.MA_HANGHOA + "'" + " AND MODE = " + cls_Main.SQLString("20240326") + ")" + "\n"
                                + "BEGIN" + "\n"
                                + "INSERT INTO CAUHINHKHUVUC(MAQUAY, MANHOM, MODE, SOLUONG) "
                                + "VALUES(" + "'" + id + "'" + ", " + "'" + pq.MA_HANGHOA + "'" + ", 20240326, NULL) " + "\n"
                                + "END" + "\n";
                        }
                    }

                    bool bRunSQL = cls_Main.ExecuteSQL(sSQL, sConnectionString_live);
                    if (bRunSQL)
                    {
                        TempData["AlertMessage"] = "Lưu thành công";
                        TempData["AlertType"] = "alert-success";
                        return RedirectToPage("Index", new { id = MAKHO });
                    }
                    else
                    {
                        return RedirectToPage("/ErrorPage", new { id = "Lưu không thành công : " + sSQL });
                    }
                }
                LoadData(string.Empty);
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "Cauhinhkhohanghoa Config : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        private void LoadData(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                id = "0";
            }
            //Kho
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "select K.*, C.TEN_CUAHANG from KHO K, CUAHANG C where K.MA_CUAHANG = C.MA_CUAHANG";
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                clsKho kho = new clsKho();
                kho.ma_kho = dr["MA_KHO"].ToString();
                kho.ten_kho = dr["TEN_KHO"].ToString();
                kho.ma_cuahang = dr["MA_CUAHANG"].ToString();
                kho.ten_cuahang = dr["TEN_CUAHANG"].ToString();
                kho.ghichu = dr["GHICHU"].ToString();
                kho.sudung = dr["SUDUNG"].ToString();
                listKho.Add(kho);
            }
            //Cauhinh
            sSQL = "SELECT CAST(ROW_NUMBER() OVER (ORDER BY MA_HANGHOA DESC) AS INT) AS STT, " + "\n" +
                "C.MAQUAY AS KHO, A.MA_HANGHOA, A.TEN_HANGHOA, " + "\n" +
                "IIF(C.MA IS NULL, 0, 1) AS SUDUNG " + "\n" +
                "FROM dbo.HANGHOA A " + "\n" +
                "LEFT JOIN dbo.NHOMHANG B ON B.MA_NHOMHANG = A.MA_HANGHOA " + "\n" +
                "LEFT JOIN dbo.CAUHINHKHUVUC C ON A.MA_HANGHOA = C.MANHOM AND C.MAQUAY = " + cls_Main.SQLString(id) + " AND MODE = " + cls_Main.SQLString("20240326") + "\n";
            if (id == "0")
            {
                sSQL += " WHERE 1 = 2" + "\n";
            }
            sSQL += " ORDER BY STT ASC";
            dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                CauHinhKhoHangHoa khohh = new CauHinhKhoHangHoa();
                khohh.STT = dr["STT"].ToString();
                khohh.KHO = dr["KHO"].ToString();
                khohh.MA_HANGHOA = dr["MA_HANGHOA"].ToString();
                khohh.TEN_HANGHOA = dr["TEN_HANGHOA"].ToString();
                khohh.SUDUNG = Convert.ToBoolean(dr["SUDUNG"]);
                listHanghoa.Add(khohh);
            }
            //makho
            if (!string.IsNullOrEmpty(id))
            {
                MAKHO = int.Parse(id);
            }
        }
     
    }
}
