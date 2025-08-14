using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.Nhanvien
{
    [Authorize]
    public class EditModel : PageModel
    {
        public clsNhanvien item = new clsNhanvien();
        public List<DM_CHUCVU> ListChucvu { set; get; } = new List<DM_CHUCVU>();
        public List<GIOITINH> ListGioitinh { set; get; } = new List<GIOITINH>();

        public List<clsNhanvien> ListNhanVien { set; get; } = new();
        public IActionResult OnGet(string id)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowEdit("11", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                LoadData(id);

                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "DM_NHANVIEN", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        public IActionResult OnPost(string id, clsNhanvien item)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    LoadData(id);
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
                sSQL += cls_Main.SQLStringUnicode(id) + ",";
                sSQL += item.TRANGTHAI.toSqlPar() + ",";
                if (item.NGAYNHANVIEC == null)
                {
                    sSQL += DateTime.Now.toSqlPar("{0:dd/MM/yyyy}") + ",";
                }
                else
                {
                    sSQL += item.NGAYNHANVIEC.toSqlPar();
                }
                //if (item.NGUOIQUANLY > 0)
                //{
                //    sSQL += item.NGUOIQUANLY.toSqlPar();
                //}
                //else
                //{
                //    sSQL += DBNull.Value.toSqlPar();
                //}
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
                cls_Main.WriterLog(cls_Main.sFilePath, "DM_NHANVIEN Edit : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        private void LoadData(string id)
        {
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "EXEC SP_W_GETNHANVIEN " + cls_Main.SQLString(id);
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                item.manhanvien = dr["MANHANVIEN"].ToString();
                item.tennhanvien = dr["TENNHANVIEN"].ToString();
                item.ngaysinh = dr["NGAYSINH"].ToString();
                item.gioitinh = dr["GIOITINH1"].ToString();
                item.diachi = dr["DIACHI"].ToString();
                item.dienthoai = dr["DIENTHOAI"].ToString();
                item.email = dr["EMAIL"].ToString();
                item.phongban = dr["CHUCVU"].ToString();
                item.ghichu = dr["GHICHU"].ToString();
                item.sudung = dr["SUDUNG"].ToString();

                item.NGAYNHANVIEC = dr["NGAYNHANVIEC"]?.ToStrDate();
                //item.NGUOIQUANLY = dr["NGUOIQUANLY"]?.ChangeType<int?>();
                //item.TEN_NGUOIQUANLY = dr["TEN_NGUOIQUANLY"]?.ChangeType<string?>();
                item.TRANGTHAI = dr["TRANGTHAI"]?.ChangeType<int>();
            }
            //chuc vu             
            sSQL = "Select * From DM_CHUCVU";
            dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                DM_CHUCVU Chucvu = new DM_CHUCVU();
                Chucvu.MACHUCVU = dr["MACHUCVU"].ToString();
                Chucvu.TENCHUCVU = dr["TENCHUCVU"].ToString();
                if (item.phongban == dr["MACHUCVU"].ToString())
                {
                    Chucvu.Macdinh = "1";
                }
                else
                {
                    Chucvu.Macdinh = "0";
                }
                ListChucvu.Add(Chucvu);
            }
            //gioi tinh
            DataTable dtGioitinh = new DataTable();
            dtGioitinh.Columns.Add("MA", typeof(bool));
            dtGioitinh.Columns.Add("TEN", typeof(string));
            dtGioitinh.Rows.Add(true, "Nam");
            dtGioitinh.Rows.Add(false, "Nữ");
            foreach (DataRow dr in dtGioitinh.Rows)
            {
                GIOITINH gioitinh = new GIOITINH();
                gioitinh.MA = dr["MA"].ToString();
                gioitinh.TEN = dr["TEN"].ToString();
                if (item.gioitinh == dr["MA"].ToString())
                {
                    gioitinh.Macdinh = "1";
                }
                else
                {
                    gioitinh.Macdinh = "0";
                }
                ListGioitinh.Add(gioitinh);
            }

            // list NhanVien
            sSQL = "EXEC SP_W_GETNHANVIEN";
            dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            ListNhanVien = dt.ToList<clsNhanvien>();
            ListNhanVien.RemoveAll(r => r.TRANGTHAI == 2);
            ListNhanVien.Insert(0, new clsNhanvien() { manhanvien = null, tennhanvien = "Không có quản lý" });
        }
        public class DM_CHUCVU
        {
            public string MACHUCVU { get; set; }
            public string TENCHUCVU { get; set; }
            public string Macdinh { get; set; }
        }
        public class GIOITINH
        {
            public string MA { get; set; }
            public string TEN { get; set; }
            public string Macdinh { get; set; }
        }
    }
}
