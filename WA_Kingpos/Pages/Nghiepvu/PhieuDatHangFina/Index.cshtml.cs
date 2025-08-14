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
    public class IndexModel : PageModel
    {
        [BindProperty]
        public PhieuDatHangModels PHIEUDATHANG { get; set; } = new PhieuDatHangModels();
        public List<SelectListItem> ListKho = new List<SelectListItem>();
        public List<KhachHangModels> ListKhachang = new List<KhachHangModels>();
        public List<HangHoaModels> ListHanghoa = new List<HangHoaModels>();
        public IActionResult OnGet(string kh, string kho, string hh)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowAdd("14102301", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                LoadData(kh, kho, hh);
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "PhieuDatHangFina Add", ex.ToString(), "0");
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
                    return RedirectToPage("Index", new { kh = PHIEUDATHANG.MADOITUONG, kho = PHIEUDATHANG.MA_KHO, hh = PHIEUDATHANG.MA_HANGHOA });
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
                    return RedirectToPage("Index", new { kh = PHIEUDATHANG.MADOITUONG, kho = PHIEUDATHANG.MA_KHO, hh = PHIEUDATHANG.MA_HANGHOA });
                }
                if (submitButton == "btn3Create")
                {
                    if (PHIEUDATHANG.listHANGHOA == null || PHIEUDATHANG.listHANGHOA.Count == 0)
                    {
                        ModelState.AddModelError(string.Empty, "Sản phẩm không được để trống");
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
                        //check trung so phieu trc khi luu
                        sSQL = "select MAPDNDC from PHIEUDATHANG where MAPDNDC=" + cls_Main.SQLString(PHIEUDATHANG.MAPDNDC);
                        DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                        if (dt.Rows.Count > 0)
                        {
                            sSQL = "SELECT [dbo].[fc_NewCode_PHIEUDATHANG]()";
                            dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                            PHIEUDATHANG.MAPDNDC = dt.Rows[0][0].ToString();

                        }
                        // cap nhat lai gia tri luoi chi tiet
                        PHIEUDATHANG.SOTIEN = "";
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
                        //
                        string SQL_PhieuChi = "";
                        string SQL_ChiTietPhieuChi = "";

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
                cls_Main.WriterLog(cls_Main.sFilePath, "PhieuDatHangFina Add", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        private void LoadData(string kh = "", string kho = "", string hh = "")
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
            //Kho
            if (!string.IsNullOrEmpty(kho))
            {
                PHIEUDATHANG.MA_KHO = kho;
            }


            //List Hanghoa
            //sSQL = "EXEC SP_FINA_SELECT_DATA @LOAI=35";
            sSQL = "SELECT MA_HANGHOA AS MA, TEN_HANGHOA AS TEN, THUE, GIANHAP,GIABAN1, GIABAN2, GIABAN3, TEN_DONVITINH, NULL AS TONHIENTAI, MAVACH , CONVERT(BIT,0) AS CHON "
                + " FROM HANGHOA h " + "\n"
                + " JOIN DONVITINH d ON h.MA_DONVITINH = d.MA_DONVITINH " + "\n"
                + " JOIN dbo.CAUHINHKHUVUC ch ON ch.MANHOM = h.MA_HANGHOA  AND ch.MODE = 20240326 AND ch.MAQUAY = " + "'" + kho + "'"
                + " ORDER BY TEN_HANGHOA";

            dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            // kiểm tra nhân viên đang thử việc hay chính thức
            var Username = User.Claims.FirstOrDefault(c => c.Type == "Username")?.Value;
            sSQL = "SELECT b.TRANGTHAI, a.UserID, b.MANHANVIEN "
                + " FROM dbo.SYS_USER a " + "\n"
                + " INNER JOIN dbo.DM_NHANVIEN b ON b.MANHANVIEN = a.MaNV " + "\n"
                + $" WHERE a.UserID = {ExtensionObject.toSqlPar(Username)} ";
            var dtNhanVien = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            var NhanVienThuViec = dtNhanVien.Rows.Count > 0 && dtNhanVien.Rows[0]["TRANGTHAI"]?.ToString() == "0";
            foreach (DataRow dr in dt.Rows)
            {
                HangHoaModels item = new HangHoaModels();
                double.TryParse(dr["GIABAN3"]?.ToString(), out double sl_DongGoi);
                if (sl_DongGoi < 1)
                {
                    sl_DongGoi = 1;
                }
                double.TryParse(dr["GIABAN1"]?.ToString(), out double GIABAN1);
                double.TryParse(dr["GIABAN2"]?.ToString(), out double GIABAN2);
                //thử việc chọn giá bán 2, chính thức chọn giá bán 1
                var GIABAN = NhanVienThuViec && GIABAN2 > 0 ? GIABAN2 : GIABAN1;
                item.MA_HANGHOA = dr["MA"].ToString();
                item.TEN_HANGHOA = dr["TEN"].ToString();
                item.DVT = dr["TEN_DONVITINH"].ToString();
                // giá thành phẩm = giá/kg * số lượng đóng gói, ví dụ 10,000đ/kg * bao 50kg
                item.DONGIA = (GIABAN * sl_DongGoi).ToString(); // dr["GIABAN1"].ToString()
                item.SL = "1";
                item.THANHTIEN = item.DONGIA; // dr["GIABAN1"].ToString()
                ListHanghoa.Add(item);
            }
            //List Kho
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
