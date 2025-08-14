using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.Nghiepvu.PhieuDatXeFina
{
    [Authorize]
    public class EditModel : PageModel
    {
        [BindProperty]
        public PhieuDatXeModels PHIEUDATXE { get; set; } = new PhieuDatXeModels();
        public List<SelectListItem> ListKho = new List<SelectListItem>();
        public List<TaiXeModels> ListTaixe = new List<TaiXeModels>();
        [BindProperty]
        public List<clsDathang> ListPhieudathang { get; set; } = new List<clsDathang>();
        public IActionResult OnGet(string id, string tx, string k, string n, string bs)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowEdit("14102310", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                LoadData(id, tx, k,n,bs);
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "PhieuDatXeFina Edit", ex.ToString(), "0");
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
                    if (PHIEUDATXE.MAPDNDC != null && PHIEUDATXE.MAPDNDC != "")
                    {
                        TempData["PHIEUDATXE_MAPHIENLAMVIEC"] = PHIEUDATXE.MAPDNDC;
                    }
                    return RedirectToPage("Edit", new { tx = PHIEUDATXE.MADOITUONG, k = PHIEUDATXE.MA_KHO, n = PHIEUDATXE.NGAYGIAOHANG, bs = PHIEUDATXE.BIENSOXE });
                }
                if (submitButton == "btn3Create")
                {
                    int iDem = 0;
                    foreach (var item in ListPhieudathang)
                    {
                        if (bool.Parse(item.chon))
                        {
                            iDem = 1;
                            break;
                        }
                    }
                    if (iDem == 0)
                    {
                        ModelState.AddModelError(string.Empty, "Phải chọn 1 phiếu");
                    }
                    if (!ModelState.IsValid)
                    {
                        //xu ly trang
                        LoadData();
                        return Page();
                    }
                    else
                    {
                        string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                        string sSQL = "";
                        string manhanvien = User.FindFirst("UserId")?.Value;

                        //Nếu phiếu đã duyệt rồi thì không cho  sửa
                        sSQL = "";
                        sSQL += "select top 1 * from PHIEUDATXE where TRANGTHAI<>0 and MAPDNDC =" + cls_Main.SQLString(PHIEUDATXE.MAPDNDC);
                        DataTable dtCheck = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                        if (dtCheck.Rows.Count > 0)
                        {
                            ViewData["Message"] = string.Format("Phiếu này đã được duyệt không thể thay đổi");
                            LoadData();
                            return Page();
                        }
                        //
                        string SQL_XoaPhieuCu = "";
                        string SQL_PhieuChi = "";
                        string SQL_ChiTietPhieuChi = "";
                        PHIEUDATXE.SOTIEN = "0";
                        SQL_XoaPhieuCu = string.Format("EXEC SP_FINA_SELECT_DATA @LOAI=52, @MAPHIEU= " + cls_Main.SQLString(PHIEUDATXE.MAPDNDC));
                        foreach (var item in ListPhieudathang)
                        {
                            if (bool.Parse(item.chon))
                            {
                                SQL_ChiTietPhieuChi += "INSERT INTO CT_PHIEUDATXE(PHIEUDENGHI,HANGHOA,SOLUONG,DONGIA,THANHTIEN,TEN_DONVITINH, DUYET, NHANVIEN, LastUpdate,GHICHU)" + "\n";
                                SQL_ChiTietPhieuChi += "VALUES(";
                                SQL_ChiTietPhieuChi += cls_Main.SQLString(PHIEUDATXE.MAPDNDC) + ",";
                                SQL_ChiTietPhieuChi += cls_Main.SQLString(item.maphieu) + ",";
                                SQL_ChiTietPhieuChi += cls_Main.SQLString(item.soluong.Replace(",", "")) + ",";
                                SQL_ChiTietPhieuChi += cls_Main.SQLString("0") + ",";
                                SQL_ChiTietPhieuChi += cls_Main.SQLString("0") + ",";
                                SQL_ChiTietPhieuChi += cls_Main.SQLStringUnicode("Phiếu") + ",";
                                SQL_ChiTietPhieuChi += cls_Main.SQLBit(false) + ",";
                                SQL_ChiTietPhieuChi += cls_Main.SQLStringUnicode(manhanvien) + ",";
                                SQL_ChiTietPhieuChi += "GETDATE()" + ",";
                                SQL_ChiTietPhieuChi += cls_Main.SQLStringUnicode("") + ")";
                                SQL_ChiTietPhieuChi += "\n";
                                PHIEUDATXE.SOTIEN = (int.Parse(PHIEUDATXE.SOTIEN.Replace(",", "")) + int.Parse(item.soluong.Replace(",", ""))).ToString("N0");
                            }
                        }
                        SQL_PhieuChi = string.Format("EXEC SP_INSERT_PHIEUDATXE @MAPDNDC='{0}',@MADOITUONG='{1}',@BIENSOXE='{2}',@PHIEUKHO_ID='{3}',@MANV='{4}',@LYDO=N'{5}',@SOTIEN='{6}',@TONGTIEN='{7}',@NGAYTHANHTOAN='{8:yyyyMMdd HH:mm:ss}'"
                        , PHIEUDATXE.MAPDNDC, PHIEUDATXE.MADOITUONG, PHIEUDATXE.BIENSOXE, PHIEUDATXE.MA_KHO, manhanvien, "TẠO TỪ WEB", PHIEUDATXE.SOTIEN.Replace(",", ""), PHIEUDATXE.SOTIEN.Replace(",", ""), PHIEUDATXE.NGAYGIAOHANG);

                        sSQL = "";
                        sSQL += SQL_XoaPhieuCu + "\n";
                        sSQL += SQL_PhieuChi + "\n";
                        sSQL += SQL_ChiTietPhieuChi + "\n";

                        bool bRunSQL = cls_Main.ExecuteSQL(sSQL, sConnectionString_live);
                        if (bRunSQL)
                        {
                            return RedirectToPage("View", new { id = PHIEUDATXE.MAPDNDC });
                        }
                        else
                        {
                            return RedirectToPage("/ErrorPage", new { id = "Lưu không thành công : " + sSQL });
                        }
                    }
                }
                LoadData();
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "PhieuDatXeFina Edit", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        private void LoadData(string id = "", string tx = "", string k = "0", string n = "", string bs = "")
        {
            //SoPhieu
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "SELECT [dbo].[fc_NewCode_PHIEUDATXE]()";
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            if (dt.Rows.Count > 0)
            {
                PHIEUDATXE.MAPDNDC = dt.Rows[0][0].ToString();
            }
            else
            {
                PHIEUDATXE.MAPDNDC = "";
            }
            //Ngaydat
            PHIEUDATXE.NGAYLAPPHIEU = DateTime.Now.ToString("dd/MM/yyyy");
            //List Phieu dat hang
            string nguoitao = User.FindFirst("Username")?.Value;
            string manhanvien = User.FindFirst("UserId")?.Value;
            DataTable dtKHO = cls_Main.ReturnDataTable_NoLock("SELECT MA_KHO,MADOITUONG,BIENSOXE,MAPDNDC,NGAYTHANHTOAN from PHIEUDATXE where MAPDNDC=" + cls_Main.SQLString(id), sConnectionString_live);
            if (dtKHO.Rows.Count > 0)
            {
                k = dtKHO.Rows[0]["MA_KHO"].ToString();
                tx = dtKHO.Rows[0]["MADOITUONG"].ToString();
                n = string.Format("{0:dd/MM/yyyy}", DateTime.Parse(dtKHO.Rows[0]["NGAYTHANHTOAN"].ToString()));
                bs = dtKHO.Rows[0]["BIENSOXE"].ToString();
                PHIEUDATXE.BIENSOXE = dtKHO.Rows[0]["BIENSOXE"].ToString();
                PHIEUDATXE.MAPHIENLAMVIEC = dtKHO.Rows[0]["MAPDNDC"].ToString();
                PHIEUDATXE.NGAYGIAOHANG = string.Format("{0:dd/MM/yyyy}", DateTime.Parse(dtKHO.Rows[0]["NGAYTHANHTOAN"].ToString()));
            }
            //id
            if (!string.IsNullOrEmpty(id))
            {
                PHIEUDATXE.MAPDNDC = id;
            }
            else
            {
                if (TempData["PHIEUDATXE_MAPHIENLAMVIEC"] != null)
                {
                    PHIEUDATXE.MAPDNDC = TempData["PHIEUDATXE_MAPHIENLAMVIEC"].ToString();
                }
            }
            sSQL = "EXEC SP_FINA_SELECT_DATA @LOAI=51,@MAPHIEU=" + cls_Main.SQLString(PHIEUDATXE.MAPDNDC) + ", @USERLOGIN =" + cls_Main.SQLString(manhanvien) + ",@MA_KHO=" + cls_Main.SQLString(k);
            dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                clsDathang item = new clsDathang();
                item.chon = dr["CHON"].ToString();
                item.maphieu = dr["MAPDNDC"].ToString();
                item.ngaylap = string.Format("{0:dd/MM/yyyy}", DateTime.Parse(dr["NGAYLAPPHIEU"].ToString()));
                item.trangthai = dr["TRANGTHAI1"].ToString();
                item.nhanvien = dr["TENNHANVIEN"].ToString();
                item.khachhang = dr["TENKHACHHANG"].ToString();
                item.soluong = decimal.Parse(dr["SOLUONG"].ToString()).ToString("N0");
                ListPhieudathang.Add(item);
            }
            //List Kho
            sSQL = "EXEC SP_GET_CUAHANG_KHO_QUAY_THEO_NHANVIEN " + cls_Main.SQLString(manhanvien) + ", 1" + ", @MACHON=" + cls_Main.SQLString("");
            dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                SelectListItem item = new SelectListItem();
                item.Value = dr["MA"].ToString();
                item.Text = dr["TEN"].ToString();

                ListKho.Add(item);
            }
            //List tai xe
            manhanvien = User.FindFirst("UserId")?.Value;
            sSQL = "EXEC SP_GET_CUAHANG_KHO_QUAY_THEO_NHANVIEN @NHANVIEN=" + cls_Main.SQLString(manhanvien) + ", @TYPE=14" + ", @MACHON=" + cls_Main.SQLString("");
            dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                TaiXeModels item = new TaiXeModels();
                item.MA = dr["MA"].ToString();
                item.TEN = dr["TEN"].ToString();
                item.CMND = dr["CMND"].ToString();
                item.DIENTHOAI = dr["DIENTHOAI"].ToString();
                item.WEBSITE = dr["WEBSITE"].ToString();
                ListTaixe.Add(item);
            }
            //tx
            if (!string.IsNullOrEmpty(tx))
            {
                TaiXeModels item = ListTaixe.Find(item => item.MA == tx);
                if (item != null)
                {
                    PHIEUDATXE.MADOITUONG = item.MA;
                    PHIEUDATXE.TEN_TAIXE = item.TEN;
                    PHIEUDATXE.CMND_TAIXE = item.CMND;
                    PHIEUDATXE.DIENTHOAI = item.DIENTHOAI;
                    PHIEUDATXE.BIENSOXE = item.WEBSITE;
                }
            }
            //Kho
            if (!string.IsNullOrEmpty(k))
            {
                PHIEUDATXE.MA_KHO = k;
            }
            //Ngay
            if (!string.IsNullOrEmpty(n))
            {
                PHIEUDATXE.NGAYGIAOHANG = n;
            }
            //Bienso
            if (!string.IsNullOrEmpty(bs))
            {
                PHIEUDATXE.BIENSOXE = bs;
            }

        }
    }
}
