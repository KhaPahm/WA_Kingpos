using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.Nhanvien
{
    [Authorize]
    public class CreateModel : PageModel
    {
        [BindProperty]
        public List<DM_CHUCVU> ListChucvu { set; get; } = new List<DM_CHUCVU>();
        public clsNhanvien item { get; set; } = new() { NGAYNHANVIEC = DateTime.Now.ToString("dd/MM/yyyy") };
        public IActionResult OnGet()
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowAdd("11", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                LoadData();

                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "DM_NHANVIEN Add", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        public IActionResult OnPost(clsNhanvien item)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    //xu ly trang
                    LoadData();
                    return Page();
                }
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string sSQL = "";
                sSQL += "EXEC SP_W_NHANVIEN ";
                sSQL += cls_Main.SQLStringUnicode(item.tennhanvien) + ",";
                if (item.ngaysinh == "")
                {
                    sSQL += string.Format("{0:dd/MM/yyyy}", DateTime.Now.ToString()) + ",";
                }
                else
                {
                    sSQL += cls_Main.SQLString(item.ngaysinh) + ",";
                }
                sSQL += cls_Main.SQLBit(bool.Parse(item.gioitinh)) + ",";
                sSQL += cls_Main.SQLStringUnicode(item.diachi) + ",";
                sSQL += cls_Main.SQLStringUnicode(item.dienthoai) + ",";
                sSQL += cls_Main.SQLStringUnicode(item.email) + ",";
                sSQL += cls_Main.SQLStringUnicode(item.ghichu) + ",";
                sSQL += cls_Main.SQLString(item.phongban) + ",";
                sSQL += cls_Main.SQLBit(bool.Parse(item.sudung)) + ",";
                sSQL += $"0, {ExtensionObject.toSqlPar(item.TRANGTHAI)}, {ExtensionObject.toSqlPar(item.NGAYNHANVIEC)}";
                bool bRunSQL = cls_Main.ExecuteSQL(sSQL, sConnectionString_live);
                if (bRunSQL)
                {
                    TempData["AlertMessage"] = "Lưu thành công";
                    TempData["AlertType"] = "alert-success";
                    return RedirectToPage("Create");
                }
                else
                {
                    return RedirectToPage("/ErrorPage", new { id = "Lưu không thành công : " + sSQL });
                }
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "DM_NHANVIEN Add", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }

        private void LoadData()
        {
            //chuc vu
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "Select * From DM_CHUCVU";
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                DM_CHUCVU Chucvu = new DM_CHUCVU();
                Chucvu.MACHUCVU = dr["MACHUCVU"].ToString();
                Chucvu.TENCHUCVU = dr["TENCHUCVU"].ToString();
                ListChucvu.Add(Chucvu);
            }
        }
        public class DM_CHUCVU
        {
            public string MACHUCVU { get; set; }
            public string TENCHUCVU { get; set; }
        }
    }
}
