using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Data;
using System.Reflection;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.DatHang
{
    [Authorize]
    public class DetailModel : PageModel
    {
        public clsDathang item = new clsDathang();

        public List<clsDathang> listitem = new List<clsDathang>();
        public int? tongsoluong { get; set; } = 0;
        public long? tongthanhtien { get; set; } = 0;
        public IActionResult OnGet(string id)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowAdd("14102302", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                LoadData(id);
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "DuyetPhieuDatHangFina view", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        public IActionResult Onpost(string id, string submitButton)
        {
            try
            {
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string sSQL = "";
                string nguoitao = User.FindFirst("Username")?.Value;
                string manhanvien = User.FindFirst("UserId")?.Value;
                //Nếu phiếu đã duyệt rồi thì không cho  sửa
                sSQL = "";
                sSQL += "select top 1 * from PHIEUDATHANG where TRANGTHAI<>0 and MAPDNDC =" + cls_Main.SQLString(id);
                DataTable dtCheck = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                if (dtCheck.Rows.Count > 0)
                {
                    ViewData["Message"] = string.Format("Phiếu này đã được duyệt không thể thay đổi");
                    LoadData(id);
                    return Page();
                }
                //
                if (submitButton == "approve")
                {
                    //duyet
                    sSQL = "";
                    sSQL = "EXEC SP_FINA_SELECT_DATA @LOAI=4, @MANHANVIEN=" + cls_Main.SQLString(manhanvien) + ", @MAPHIEU = " + cls_Main.SQLString(id);
                    sSQL += "\n";
                    sSQL += "EXEC SP_FINA_SELECT_DATA @LOAI=6,  @MAPHIEU = " + cls_Main.SQLString(id);
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
                else
                {
                    //khong duyet
                    sSQL = "";
                    sSQL = "EXEC SP_FINA_SELECT_DATA @LOAI=7,  @MANHANVIEN=" + cls_Main.SQLString(manhanvien) + ", @MAPHIEU = " + cls_Main.SQLString(id);
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
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "DuyetPhieuDatHangFina : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        private void LoadData(string id)
        {
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "EXEC SP_FINA_SELECT_DATA @LOAI=3, @MAPHIEU=" + cls_Main.SQLString(id);
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            item.kho = dt.Rows[0]["TEN_KHO"].ToString();
            item.maphieu = dt.Rows[0]["MAPDNDC"].ToString();
            item.nhanvien = dt.Rows[0]["TENNHANVIEN"].ToString();
            item.ngaylap = dt.Rows[0]["NGAYLAPPHIEU"].ToString();
            item.khachhang = dt.Rows[0]["TENKHACHHANG"].ToString();
            item.dienthoai = dt.Rows[0]["DIENTHOAI"].ToString();
            item.diachi = dt.Rows[0]["DIACHI"].ToString();
            item.email = dt.Rows[0]["EMAIL"].ToString();
            item.masothue = dt.Rows[0]["MST"].ToString();
            item.cccd = dt.Rows[0]["CMND"].ToString();
            //
            foreach (DataRow dr in dt.Rows)
            {
                clsDathang item = new clsDathang();
                item.masanpham = dr["MA_VACH"].ToString();
                item.tensanpham = dr["TEN"].ToString();
                item.donvitinh = dr["DVT"].ToString();
                item.dongia = decimal.Parse(dr["GIANHAP"].ToString()).ToString("N0");
                item.soluong = decimal.Parse(dr["SL"].ToString()).ToString("N0");
                item.thanhtien = decimal.Parse(dr["THANHTIEN"].ToString()).ToString("N0");
                item.stt = (1 + dt.Rows.IndexOf(dr)).ToString();

                listitem.Add(item);

                tongsoluong = tongsoluong + 1;
                tongthanhtien = tongthanhtien + long.Parse(dr["THANHTIEN"].ToString());
            }
        }
    }
}
