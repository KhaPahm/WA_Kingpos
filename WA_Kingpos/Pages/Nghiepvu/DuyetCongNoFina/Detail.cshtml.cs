using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.Nghiepvu.DuyetCongNoFina
{
    [Authorize]
    public class DetailModel : PageModel
    {
        public PhieuDuyetThu PHIEUDUYETTHU = new PhieuDuyetThu();

        public List<DONDATModels> ListDONDAT = new List<DONDATModels>();
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
                cls_Main.WriterLog(cls_Main.sFilePath, "DuyetCongNoFinaDetail", ex.ToString(), "0");
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
                sSQL += "select top 1 * from CN_CONGNO_PHIEUDENGHIDUYETTHU where TRANGTHAI<>0 and MAPDNDT =" + cls_Main.SQLString(id);
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
                    sSQL = "EXEC SP_FINA_SELECT_DATA @LOAI=59, @MANHANVIEN=" + cls_Main.SQLString(manhanvien) + ", @MAPHIEU = " + cls_Main.SQLString(id);
                    sSQL += "\n";
                    sSQL += "EXEC SP_FINA_SELECT_DATA @LOAI=60,  @MAPHIEU = " + cls_Main.SQLString(id);
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
                    sSQL = "EXEC SP_FINA_SELECT_DATA @LOAI=61,  @MANHANVIEN=" + cls_Main.SQLString(manhanvien) + ", @MAPHIEU = " + cls_Main.SQLString(id);
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
                cls_Main.WriterLog(cls_Main.sFilePath, "DuyetCongNoFinaDetail : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        private void LoadData(string id)
        {
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "select CN.MAPDNDT, NV.TENNHANVIEN, CN.NGAYLAPPHIEU, CN.LYDO, CN.TRANGTHAI, CN.TONGTIEN, CT.HANGHOA, CT.THANHTIEN, C.TEN, DH.NGAYLAPPHIEU, (select TENNHANVIEN FROM DM_NHANVIEN where MANHANVIEN = DH.MANV) AS TENNHANVIEN_DH from CN_CONGNO_PHIEUDENGHIDUYETTHU CN, CHITIETPHIEUDENGHIDUYETTHU CT, PHIEUDATHANG DH, NHACUNGCAP C, DM_NHANVIEN NV where CN.MAPDNDT = CT.PHIEUDENGHI  AND CT.HANGHOA = DH.MAPDNDC and DH.MADOITUONG = C.MA AND CN.MANV = NV.MANHANVIEN AND CN.MAPDNDT = " + cls_Main.SQLString(id);
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            PHIEUDUYETTHU.SO_PHIEU = dt.Rows[0]["MAPDNDT"].ToString();
            PHIEUDUYETTHU.NHAN_VIEN = dt.Rows[0]["TENNHANVIEN"].ToString();
            PHIEUDUYETTHU.NGAYLAPPHIEU = String.Format("{0:dd/MM/yyyy}", dt.Rows[0]["NGAYLAPPHIEU"]);
            PHIEUDUYETTHU.LY_DO = dt.Rows[0]["LYDO"].ToString();
            PHIEUDUYETTHU.TRANG_THAI = dt.Rows[0]["TRANGTHAI"].ToString();
            PHIEUDUYETTHU.SOTIEN = int.Parse(dt.Rows[0]["TONGTIEN"].ToString()).ToString("N0");
            //
            foreach (DataRow dr in dt.Rows)
            {
                DONDATModels item = new DONDATModels();
                item.PHIEUDATHANG = dr["HANGHOA"].ToString();
                item.NHANVIEN_DATHANG = dr["TENNHANVIEN_DH"].ToString();
                item.THANHTIEN = int.Parse(dr["THANHTIEN"].ToString()).ToString("N0");
                item.KHACHHANG = dr["TEN"].ToString();
                item.NGAYLAP = String.Format("{0:dd/MM/yyyy}", dr["NGAYLAPPHIEU"]);
                item.STT = (1 + dt.Rows.IndexOf(dr)).ToString();
                ListDONDAT.Add(item);

            }
        }
    }
}
