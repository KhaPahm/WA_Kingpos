using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.Nghiepvu.DuyetPhieuDatXeFina
{
    [Authorize]
    public class DetailModel : PageModel
    {
        public clsDathang item = new clsDathang();

        public List<clsDathang> listitem = new List<clsDathang>();
        public long? tongsoluong { get; set; } = 0;
        public long? tongthanhtien { get; set; } = 0;
        public IActionResult OnGet(string id)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowAdd("14102311", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                LoadData(id);
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "DuyetPhieuDatXeFina view", ex.ToString(), "0");
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
                sSQL += "select top 1 * from PHIEUDATXE where TRANGTHAI<>0 and MAPDNDC =" + cls_Main.SQLString(id);
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
                    sSQL = "EXEC SP_FINA_SELECT_DATA @LOAI=54, @MANHANVIEN=" + cls_Main.SQLString(manhanvien) + ", @MAPHIEU = " + cls_Main.SQLString(id);
                    sSQL += "\n";
                    sSQL += "EXEC SP_FINA_SELECT_DATA @LOAI=56,  @MAPHIEU = " + cls_Main.SQLString(id);
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
                    sSQL = "EXEC SP_FINA_SELECT_DATA @LOAI=57,  @MANHANVIEN=" + cls_Main.SQLString(manhanvien) + ", @MAPHIEU = " + cls_Main.SQLString(id);
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
                cls_Main.WriterLog(cls_Main.sFilePath, "DuyetPhieuDatXeFina : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        private void LoadData(string id)
        {
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "EXEC SP_FINA_SELECT_DATA @LOAI=50, @MAPHIEU=" + cls_Main.SQLString(id);
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            item.kho = dt.Rows[0]["TEN_KHO"].ToString();
            item.maphieu = dt.Rows[0]["MAPDNDC"].ToString();
            item.nhanvien = dt.Rows[0]["TENNHANVIEN"].ToString();
            item.ngaylap = dt.Rows[0]["NGAYLAPPHIEU"].ToString();
            item.ngaygiaohang = dt.Rows[0]["NGAYTHANHTOAN"].ToString();
            item.khachhang = dt.Rows[0]["TENKHACHHANG"].ToString();
            item.dienthoai = dt.Rows[0]["DIENTHOAI"].ToString();
            item.diachi = dt.Rows[0]["BIENSOXE"].ToString();
            item.cccd = dt.Rows[0]["CMND"].ToString();
            //
            foreach (DataRow dr in dt.Rows)
            {
                clsDathang item = new clsDathang();
                item.masanpham = dr["MA_VACH"].ToString();
                item.tensanpham = dr["TEN"].ToString();
                item.donvitinh = dr["NGAYLAPPHIEU1"].ToString();
                item.dongia = dr["TEN1"].ToString();
                item.thanhtien = decimal.Parse(dr["TONGTIEN"].ToString()).ToString("N0");
                item.soluong = decimal.Parse(dr["SOLUONG"].ToString()).ToString("N0");
                item.stt = (1 + dt.Rows.IndexOf(dr)).ToString();

                listitem.Add(item);
                tongthanhtien = tongthanhtien + long.Parse(dr["TONGTIEN"].ToString());
                tongsoluong = tongsoluong + long.Parse(dr["SOLUONG"].ToString());
            }
        }
    }
}
