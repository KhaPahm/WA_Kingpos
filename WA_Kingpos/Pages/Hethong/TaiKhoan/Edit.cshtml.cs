using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.TaiKhoan
{
    [Authorize]
    public class EditModel : PageModel
    {
        [BindProperty]
        public List<DM_NHANVIEN> Listnhanvien { set; get; } = new List<DM_NHANVIEN>();
        public List<NHOMQUYEN> ListNhomquyen { set; get; } = new List<NHOMQUYEN>();

        public clsTaikhoan item = new clsTaikhoan();
        public IActionResult OnGet(string id)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowEdit("19", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                Loaddata(id);
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "SYS_USER Edit", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        public IActionResult OnPost(string id, clsTaikhoan item)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Loaddata(id);
                    return Page();
                }
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string sSQL = "";
                sSQL += "EXEC SP_W_TAIKHOAN ";
                sSQL += cls_Main.SQLStringUnicode(id) + ",";
                sSQL += cls_Main.SQLStringUnicode(item.matkhau) + ",";
                sSQL += cls_Main.SQLStringUnicode(item.nhanvien) + ",";
                sSQL += cls_Main.SQLStringUnicode(item.nhomquyen) + ",";
                sSQL += cls_Main.SQLBit(bool.Parse(item.sudung)) + ",1";
                bool bRunSQL = cls_Main.ExecuteSQL(sSQL, sConnectionString_live);
                if (bRunSQL)
                {
                    return RedirectToPage("Index");
                }
                else
                {
                    return RedirectToPage("/ErrorPage", new { id = "Lưu không thành công : " + sSQL });
                }
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "SYS_USER Edit", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        private void Loaddata(string id)
        {
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "EXEC SP_W_GETTAIKHOAN " + cls_Main.SQLString(id);
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                item.userid = dr["UserId"].ToString();
                item.nhomquyen = dr["GroupID"].ToString();
                item.matkhau = dr["Password"].ToString();
                item.nhanvien = dr["MaNV"].ToString();
                item.sudung = dr["Active"].ToString();
            }
            //nhanvien
            sSQL = "Select MANHANVIEN,TENNHANVIEN From DM_NHANVIEN where SUDUNG=1 order by TENNHANVIEN";
            dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                DM_NHANVIEN Nhanvien = new DM_NHANVIEN();
                Nhanvien.MANHANVIEN = dr["MANHANVIEN"].ToString();
                Nhanvien.TENNHANVIEN = dr["TENNHANVIEN"].ToString();
                if (item.nhanvien == Nhanvien.MANHANVIEN)
                {
                    Nhanvien.Macdinh = "1";
                }
                else
                {
                    Nhanvien.Macdinh = "0";
                }
                Listnhanvien.Add(Nhanvien);
            }
            //nhomquyen
            sSQL = "select GroupId, GroupName from sys_group where Active=1 order by GroupName";
            dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                NHOMQUYEN Nhomquyen = new NHOMQUYEN();
                Nhomquyen.MA = dr["GroupId"].ToString();
                Nhomquyen.TEN = dr["GroupName"].ToString();
                if (item.nhomquyen == Nhomquyen.MA)
                {
                    Nhomquyen.Macdinh = "1";
                }
                else
                {
                    Nhomquyen.Macdinh = "0";
                }
                ListNhomquyen.Add(Nhomquyen);
            }
        }
        public class DM_NHANVIEN
        {
            public string MANHANVIEN { get; set; }
            public string TENNHANVIEN { get; set; }

            public string Macdinh { get; set; }
        }
        public class NHOMQUYEN
        {
            public string MA { get; set; }
            public string TEN { get; set; }

            public string Macdinh { get; set; }
        }
    }
}

