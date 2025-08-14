using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.Nghiepvu.PhieuDatHangFina
{
    [Authorize]
    public class EditModel : PageModel
    {
        [BindProperty]
        public PhieuDatHangModels PHIEUDATHANG { get; set; } = new PhieuDatHangModels();
        public List<SelectListItem> ListKho = new List<SelectListItem>();
        public List<KhachHangModels> ListKhachang = new List<KhachHangModels>();
        public List<HangHoaModels> ListHanghoa = new List<HangHoaModels>();
        public IActionResult OnGet(string id,string kh, string kho, string hh)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowEdit("14102301", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                LoadData(id,kh, kho, hh);
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "PhieuDatHangFina Edit", ex.ToString(), "0");
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
                    if (PHIEUDATHANG.listHANGHOA != null)
                    {
                        TempData["PHIEUDATHANG_HANGHOA"] = JsonConvert.SerializeObject(PHIEUDATHANG.listHANGHOA);
                    }
                    if (PHIEUDATHANG.MAPHIENLAMVIEC != null && PHIEUDATHANG.MAPHIENLAMVIEC !="")
                    {
                        TempData["PHIEUDATHANG_MAPHIENLAMVIEC"] = PHIEUDATHANG.MAPHIENLAMVIEC;
                    }
                    return RedirectToPage("Edit", new { kh = PHIEUDATHANG.MADOITUONG, kho = PHIEUDATHANG.MA_KHO, hh = PHIEUDATHANG.MA_HANGHOA });
                }
                if (submitButton.Contains("btn2Delete"))
                {
                    string[] wordsArray = submitButton.Split("_");
                    string sSTT = wordsArray[1];
                    HangHoaModels item = PHIEUDATHANG.listHANGHOA.Find(item => item.STT == sSTT);
                    if (item != null)
                    {
                        PHIEUDATHANG.listHANGHOA.Remove(item);

                    }
                    if (PHIEUDATHANG.listHANGHOA != null)
                    {
                        TempData["PHIEUDATHANG_HANGHOA"] = JsonConvert.SerializeObject(PHIEUDATHANG.listHANGHOA);
                    }
                    if (PHIEUDATHANG.MAPHIENLAMVIEC != null && PHIEUDATHANG.MAPHIENLAMVIEC != "")
                    {
                        TempData["PHIEUDATHANG_MAPHIENLAMVIEC"] = PHIEUDATHANG.MAPHIENLAMVIEC;
                    }
                    return RedirectToPage("Edit", new { kh = PHIEUDATHANG.MADOITUONG, kho = PHIEUDATHANG.MA_KHO, hh = PHIEUDATHANG.MA_HANGHOA });
                }
                if (submitButton == "btn3Create")
                {
                    if (PHIEUDATHANG.listHANGHOA == null || PHIEUDATHANG.listHANGHOA.Count == 0)
                    {
                        ModelState.AddModelError(string.Empty, "Sản phẩm không được để trống");
                    }
                    if (PHIEUDATHANG.MAPHIENLAMVIEC == null || PHIEUDATHANG.MAPHIENLAMVIEC == "")
                    {
                        ModelState.AddModelError(string.Empty, "Chưa có mả phiếu củ");
                    }
                    if (!ModelState.IsValid)
                    {
                        //xu ly trang
                        if (PHIEUDATHANG.listHANGHOA != null)
                        {
                            TempData["PHIEUDATHANG_HANGHOA"] = JsonConvert.SerializeObject(PHIEUDATHANG.listHANGHOA);
                        }
                        LoadData();
                        return Page();
                    }
                    else
                    {
                        string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                        string sSQL = "";
                        string manhanvien = User.FindFirst("UserId")?.Value;
                        // cap nhat lai gia tri luoi chi tiet
                        PHIEUDATHANG.SOTIEN = "";
                        PHIEUDATHANG.MAPDNDC = PHIEUDATHANG.MAPHIENLAMVIEC;
                        foreach (var item in PHIEUDATHANG.listHANGHOA)
                        {
                            if (item.MA_HANGHOA != null)
                            {
                                item.STT = PHIEUDATHANG.listHANGHOA.IndexOf(item).ToString();
                                item.DONGIA = int.Parse(item.DONGIA.Replace(",", "")).ToString("N0");
                                item.SL = int.Parse(item.SL.Replace(",", "")).ToString("N0");
                                item.THANHTIEN = (int.Parse(item.DONGIA.Replace(",", "")) * int.Parse(item.SL.Replace(",", ""))).ToString("N0");
                                if (string.IsNullOrEmpty(PHIEUDATHANG.SOTIEN))
                                {
                                    PHIEUDATHANG.SOTIEN = "0";
                                }
                                PHIEUDATHANG.SOTIEN = (int.Parse(PHIEUDATHANG.SOTIEN.Replace(",", "")) + int.Parse(item.THANHTIEN.Replace(",", ""))).ToString("N0");
                            }
                        }
                        //Nếu phiếu đã duyệt rồi thì không cho  sửa
                        sSQL = "";
                        sSQL += "select top 1 * from PHIEUDATHANG where TRANGTHAI<>0 and MAPDNDC =" + cls_Main.SQLString(PHIEUDATHANG.MAPDNDC);
                        DataTable dtCheck = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                        if (dtCheck.Rows.Count > 0)
                        {
                            ViewData["Message"] = string.Format("Phiếu này đã được duyệt không thể thay đổi");
                            if (PHIEUDATHANG.listHANGHOA != null)
                            {
                                TempData["PHIEUDATHANG_HANGHOA"] = JsonConvert.SerializeObject(PHIEUDATHANG.listHANGHOA);
                            }
                            LoadData();
                            return Page();
                        }
                        //
                        string SQL_XoaPhieuCu = "";
                        string SQL_PhieuChi = "";
                        string SQL_ChiTietPhieuChi = "";

                        SQL_XoaPhieuCu = string.Format("EXEC SP_FINA_SELECT_DATA @LOAI=11, @MAPHIEU= " + cls_Main.SQLString(PHIEUDATHANG.MAPDNDC));

                        SQL_PhieuChi = string.Format("EXEC SP_INSERT_PHIEUDATHANG @MAPDNDC='{0}',@MADOITUONG='{1}',@MA_HOADON='{2}',@PHIEUKHO_ID='{3}',@MANV='{4}',@MALOAI='{5}',@LYDO=N'{6}',@SOTIEN='{7}',@KEMTHEO=N'{8}',@TONGTIEN='{9}',@HOTEN=N'{10}',@DIACHI=N'{11}',@MANVMUON='{12}',@MATT='{13}',@NGAYTHANHTOAN='{14:yyyyMMdd HH:mm:ss}',@TENNGANHANGCHUYEN=N'{15}',@SOTAIKHOANCHUYEN=N'{16}',@TENNGANHANGNHAN=N'{17}',@SOTAIKHOANNHAN=N'{18}',@SOGIAODICH=N'{19}',@HOTENGIAODICH=N'{20}',@NGUONCHI='{21}'"
                        , PHIEUDATHANG.MAPDNDC, PHIEUDATHANG.MADOITUONG, "", PHIEUDATHANG.MA_KHO, manhanvien, "05", "TẠO TỪ WEB", PHIEUDATHANG.SOTIEN.Replace(",", ""), "", PHIEUDATHANG.SOTIEN.Replace(",", ""), PHIEUDATHANG.MANHANVIEN, "", "", "01", DateTime.Now, "", "", "", "", "", manhanvien, "1");

                        foreach (var item in PHIEUDATHANG.listHANGHOA)
                        {
                            if (item.MA_HANGHOA != null)
                            {
                                SQL_ChiTietPhieuChi += "INSERT INTO CT_PHIEUDATHANG(PHIEUDENGHI,HANGHOA,SOLUONG,DONGIA,THANHTIEN,TEN_DONVITINH, DUYET, NHANVIEN, LastUpdate,GHICHU)" + "\n";
                                SQL_ChiTietPhieuChi += "VALUES(";
                                SQL_ChiTietPhieuChi += cls_Main.SQLString(PHIEUDATHANG.MAPDNDC) + ",";
                                SQL_ChiTietPhieuChi += cls_Main.SQLString(item.MA_HANGHOA) + ",";
                                SQL_ChiTietPhieuChi += cls_Main.SQLString(item.SL.Replace(",", "")) + ",";
                                SQL_ChiTietPhieuChi += cls_Main.SQLString(item.DONGIA.Replace(",", "")) + ",";
                                SQL_ChiTietPhieuChi += cls_Main.SQLString(item.THANHTIEN.Replace(",", "")) + ",";
                                SQL_ChiTietPhieuChi += cls_Main.SQLStringUnicode(item.DVT) + ",";
                                SQL_ChiTietPhieuChi += cls_Main.SQLBit(false) + ",";
                                SQL_ChiTietPhieuChi += cls_Main.SQLStringUnicode(manhanvien) + ",";
                                SQL_ChiTietPhieuChi += "GETDATE()" + ",";
                                SQL_ChiTietPhieuChi += cls_Main.SQLStringUnicode("") + ")";
                                SQL_ChiTietPhieuChi += "\n";
                            }
                        }
                        sSQL = "";
                        sSQL += SQL_XoaPhieuCu + "\n";
                        sSQL += SQL_PhieuChi + "\n";
                        sSQL += SQL_ChiTietPhieuChi + "\n";

                        bool bRunSQL = cls_Main.ExecuteSQL(sSQL, sConnectionString_live);
                        if (bRunSQL)
                        {
                            return RedirectToPage("View", new { id = PHIEUDATHANG.MAPDNDC });
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
                cls_Main.WriterLog(cls_Main.sFilePath, "PhieuDatHangFina Edit", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        private void LoadData(string id="",string kh = "", string kho = "", string hh = "")
        {
            //SoPhieu
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "SELECT [dbo].[fc_NewCode_PHIEUDATHANG]()";
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            if (dt.Rows.Count > 0)
            {
                PHIEUDATHANG.MAPDNDC = dt.Rows[0][0].ToString();
            }
            else
            {
                PHIEUDATHANG.MAPDNDC = "";
            }
            //Ngaydat
            PHIEUDATHANG.NGAYLAPPHIEU = DateTime.Now.ToString("dd/MM/yyyy");
            
            string maKhoC = string.Empty;
            DataTable dtC = new DataTable();
            if (kho == null || kho.Equals(""))
            {
                string sSQLC = "EXEC SP_FINA_SELECT_DATA @LOAI=3, @MAPHIEU=" + cls_Main.SQLString(id);
                dtC = cls_Main.ReturnDataTable_NoLock(sSQLC, sConnectionString_live);
                foreach (DataRow dr in dtC.Rows)
                {
                    maKhoC = dr["MA_KHO"].ToString();

                    //List Hanghoa
                    //sSQL = "EXEC SP_FINA_SELECT_DATA @LOAI=35";
                }
            }
            else
            {
                maKhoC = kho;
            }
            
            sSQL = "SELECT MA_HANGHOA AS MA, TEN_HANGHOA AS TEN, THUE, GIANHAP,GIABAN1, TEN_DONVITINH, NULL AS TONHIENTAI, MAVACH , CONVERT(BIT,0) AS CHON "
                    + " FROM HANGHOA h " + "\n"
                    + " JOIN DONVITINH d ON h.MA_DONVITINH = d.MA_DONVITINH " + "\n"
                    + " JOIN dbo.CAUHINHKHUVUC ch ON ch.MANHOM = h.MA_HANGHOA  AND ch.MODE = 20240326 AND ch.MAQUAY = " + "'" + maKhoC + "'"
                    + " ORDER BY TEN_HANGHOA";
            dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);

            foreach (DataRow dr in dt.Rows)
            {
                HangHoaModels item = new HangHoaModels();
                item.MA_HANGHOA = dr["MA"].ToString();
                item.TEN_HANGHOA = dr["TEN"].ToString();
                item.DVT = dr["TEN_DONVITINH"].ToString();
                item.DONGIA = dr["GIABAN1"].ToString();
                item.SL = "1";
                item.THANHTIEN = dr["GIABAN1"].ToString();
                ListHanghoa.Add(item);
            }

            ////List Kho
            //
            string manhanvien = User.FindFirst("UserId")?.Value;
            sSQL = "EXEC SP_GET_CUAHANG_KHO_QUAY_THEO_NHANVIEN " + cls_Main.SQLString(manhanvien) + ", 1" + ", @MACHON=" + cls_Main.SQLString("");
            dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                SelectListItem item = new SelectListItem();
                item.Value = dr["MA"].ToString();
                item.Text = dr["TEN"].ToString();

                ListKho.Add(item);
            }
            //List Khachhang
            manhanvien = User.FindFirst("UserId")?.Value;
            sSQL = "EXEC SP_GET_CUAHANG_KHO_QUAY_THEO_NHANVIEN @NHANVIEN=" + cls_Main.SQLString(manhanvien) + ", @TYPE=12" + ", @MACHON=" + cls_Main.SQLString("");
            dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                KhachHangModels item = new KhachHangModels();
                item.MA = dr["MA"].ToString();
                item.TEN = dr["TEN"].ToString();
                item.DIACHI = dr["DIACHI"].ToString();
                item.DIENTHOAI = dr["DIENTHOAI"].ToString();
                ListKhachang.Add(item);
            }
            //kh
            if (!string.IsNullOrEmpty(kh))
            {
                KhachHangModels item = ListKhachang.Find(item => item.MA == kh);
                if (item != null)
                {
                    PHIEUDATHANG.MADOITUONG = item.MA;
                    PHIEUDATHANG.DIACHI = item.DIACHI;
                    PHIEUDATHANG.DIENTHOAI = item.DIENTHOAI;
                }
            }
            //Kho
            if (!string.IsNullOrEmpty(kho))
            {
                PHIEUDATHANG.MA_KHO = kho;
            }
            //hh
            if (TempData["PHIEUDATHANG_HANGHOA"] != null)
            {
                PHIEUDATHANG.listHANGHOA = JsonConvert.DeserializeObject<List<HangHoaModels>>(TempData["PHIEUDATHANG_HANGHOA"].ToString());
            }
            else
            {
                PHIEUDATHANG.listHANGHOA = new List<HangHoaModels>();
            }
            if (!string.IsNullOrEmpty(hh))
            {
                PHIEUDATHANG.MA_HANGHOA = "0";
                HangHoaModels item = ListHanghoa.Find(item => item.MA_HANGHOA == hh);
                if (item != null)
                {
                    PHIEUDATHANG.listHANGHOA.Add(item);

                }
            }
            //id
            if (!string.IsNullOrEmpty(id))
            {
                
                foreach (DataRow dr in dtC.Rows)
                {
                    PHIEUDATHANG.MAPDNDC = dr["MAPDNDC"].ToString();
                    PHIEUDATHANG.MAPHIENLAMVIEC = dr["MAPDNDC"].ToString();
                    PHIEUDATHANG.NGAYLAPPHIEU = dr["NGAYLAPPHIEU"].ToString();
                    PHIEUDATHANG.MA_KHO = dr["MA_KHO"].ToString();
                    PHIEUDATHANG.MADOITUONG = dr["MADOITUONG"].ToString();
                    PHIEUDATHANG.TENKHACHHANG = dr["TENKHACHHANG"].ToString();
                    PHIEUDATHANG.DIACHI = dr["DIACHI"].ToString();
                    PHIEUDATHANG.DIENTHOAI = dr["DIENTHOAI"].ToString();
                    PHIEUDATHANG.MA_HANGHOA = dr["MA"].ToString();
                    HangHoaModels item = ListHanghoa.Find(item => item.MA_HANGHOA == PHIEUDATHANG.MA_HANGHOA);
                    if (item != null)
                    {
                        HangHoaModels item1 = new HangHoaModels();
                        item1.MA_HANGHOA = dr["MA"].ToString();
                        item1.TEN_HANGHOA = dr["TEN_HANGHOA"].ToString();
                        item1.DVT = dr["DVT"].ToString();
                        item1.DONGIA = dr["GIANHAP"].ToString();
                        item1.SL = dr["SL"].ToString();
                        item1.THANHTIEN = "0";

                        PHIEUDATHANG.listHANGHOA.Add(item1);
                    }
                    PHIEUDATHANG.MA_HANGHOA = "0";
                }
            }
            else
            {
                if (TempData["PHIEUDATHANG_MAPHIENLAMVIEC"] != null)
                {
                    PHIEUDATHANG.MAPHIENLAMVIEC = TempData["PHIEUDATHANG_MAPHIENLAMVIEC"].ToString ();
                }
            }
            //
            foreach (var item in PHIEUDATHANG.listHANGHOA)
            {
                if (item.MA_HANGHOA != null)
                {
                    item.STT = PHIEUDATHANG.listHANGHOA.IndexOf(item).ToString();
                    item.DONGIA = int.Parse(item.DONGIA.Replace(",", "")).ToString("N0");
                    item.SL = int.Parse(item.SL.Replace(",", "")).ToString("N0");
                    item.THANHTIEN = (int.Parse(item.DONGIA.Replace(",", "")) * int.Parse(item.SL.Replace(",", ""))).ToString("N0");
                    if (string.IsNullOrEmpty(PHIEUDATHANG.SOTIEN))
                    {
                        PHIEUDATHANG.SOTIEN = "0";
                    }
                    PHIEUDATHANG.SOTIEN = (int.Parse(PHIEUDATHANG.SOTIEN.Replace(",", "")) + int.Parse(item.THANHTIEN.Replace(",", ""))).ToString("N0");
                }
            }
            





        }
    }
}
