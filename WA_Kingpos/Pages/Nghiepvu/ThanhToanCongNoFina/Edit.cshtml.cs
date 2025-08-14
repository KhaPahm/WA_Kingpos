using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.Nghiepvu.ThanhToanCongNoFina
{
    [Authorize]
    public class EditModel : PageModel
    {
        [BindProperty]
        public PhieuDuyetThu PHIEUDUYETTHU { get; set; } = new PhieuDuyetThu();

        [BindProperty]
        public List<DONDATModels> ListDONDAT { get; set; } = new List<DONDATModels>();

        long sTongTien = 0;
        public IActionResult OnGet(string id, string lydo, string trangthai)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowEdit("14102304", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                LoadData(id, lydo, trangthai);
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "ThanhToanCongNoFina Edit", ex.ToString(), "0");
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
                    if (PHIEUDUYETTHU.listDONDAT != null)
                    {
                        TempData["PHIEUTHANHTOAN_HOADON"] = JsonConvert.SerializeObject(PHIEUDUYETTHU.listDONDAT);
                    }
                    if (ListDONDAT != null)
                    {
                        TempData["PHIEUTHANHTOAN_DONDAT"] = JsonConvert.SerializeObject(ListDONDAT);
                    }
                    return RedirectToPage("Edit", new { id = PHIEUDUYETTHU.SO_PHIEU, lydo = PHIEUDUYETTHU.LY_DO, trangthai = "1" });
                }
                if (submitButton == "btn3Create")
                {
                    if (PHIEUDUYETTHU.listDONDAT == null || PHIEUDUYETTHU.listDONDAT.Count == 0)
                    {
                        ModelState.AddModelError(string.Empty, "Phiếu đặt không được để trống");
                    }
                    bool kt = KiemTra(PHIEUDUYETTHU.listDONDAT);
                    if (!ModelState.IsValid)
                    {
                        //xu ly trang
                        if (PHIEUDUYETTHU.listDONDAT != null)
                        {
                            TempData["PHIEUTHANHTOAN_HOADON"] = JsonConvert.SerializeObject(PHIEUDUYETTHU.listDONDAT);
                        }
                        if (ListDONDAT != null)
                        {
                            TempData["PHIEUTHANHTOAN_DONDAT"] = JsonConvert.SerializeObject(ListDONDAT);
                        }
                        LoadData(PHIEUDUYETTHU.SO_PHIEU, PHIEUDUYETTHU.LY_DO, "1");
                        return Page();
                    }
                    else
                    {
                        string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                        string sSQL = "";
                        string manhanvien = User.FindFirst("UserId")?.Value;

                        //Nếu phiếu đã duyệt rồi thì không cho  sửa
                        sSQL = "";
                        sSQL += "select top 1 * from CN_CONGNO_PHIEUDENGHIDUYETTHU where TRANGTHAI<>0 and MAPDNDT =" + cls_Main.SQLString(PHIEUDUYETTHU.SO_PHIEU);
                        DataTable dtCheck = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                        if (dtCheck.Rows.Count > 0)
                        {
                            ViewData["Message"] = string.Format("Phiếu này đã được duyệt không thể thay đổi");
                            if (PHIEUDUYETTHU.listDONDAT != null)
                            {
                                TempData["PHIEUTHANHTOAN_HOADON"] = JsonConvert.SerializeObject(PHIEUDUYETTHU.listDONDAT);
                            }
                            LoadData(PHIEUDUYETTHU.SO_PHIEU, PHIEUDUYETTHU.LY_DO, "1");
                            return Page();
                        }
                        //
                        string SQL_XoaPhieu = "";
                        SQL_XoaPhieu += "DELETE FROM CHITIETPHIEUDENGHIDUYETTHU WHERE PHIEUDENGHI =" + cls_Main.SQLString(PHIEUDUYETTHU.SO_PHIEU) + "\n";
                        SQL_XoaPhieu += "DELETE FROM CN_CONGNO_PHIEUDENGHIDUYETTHU WHERE MAPDNDT = " + cls_Main.SQLString(PHIEUDUYETTHU.SO_PHIEU);
                        SQL_XoaPhieu += "DELETE FROM CHITIETTHANHTOAN WHERE MA_HOADON = " + cls_Main.SQLString(PHIEUDUYETTHU.SO_PHIEU);
                        string SQL_ChiTietPhieuChi = ChiTietPhieuThu();
                        string SQL_PhieuChi = string.Format("EXEC sp_CN_CONGNO_PHIEUDENGHIDUYETTHU_INSERT_NS @MAPDNDT='{0}',@MADOITUONG='{1}',@MA_HOADON='{2}',@PHIEUKHO_ID='{3}',@MANV='{4}',@MALOAI='{5}',@LYDO=N'{6}',@SOTIEN='{7}',@KEMTHEO=N'{8}',@TONGTIEN='{9}',@HOTEN=N'{10}',@DIACHI=N'{11}',@MANVMUON='{12}',@MATT='{13}',@NGAYTHANHTOAN='{14:yyyyMMdd HH:mm:ss}',@TENNGANHANGCHUYEN=N'{15}',@SOTAIKHOANCHUYEN=N'{16}',@TENNGANHANGNHAN=N'{17}',@SOTAIKHOANNHAN=N'{18}',@SOGIAODICH=N'{19}',@HOTENGIAODICH=N'{20}',@NGUONTHU='{21}'", PHIEUDUYETTHU.SO_PHIEU, "0", "", "", manhanvien, "02", PHIEUDUYETTHU.LY_DO, sTongTien, "", sTongTien, "", "", "", "02", DateTime.Now, "", "", "", "", "", "", "1");
                        sSQL = "";
                        sSQL += SQL_XoaPhieu + "\n";
                        sSQL += SQL_PhieuChi + "\n";
                        sSQL += SQL_ChiTietPhieuChi + "\n";

                        bool bRunSQL = cls_Main.ExecuteSQL(sSQL, sConnectionString_live);
                        if (bRunSQL)
                        {
                            return RedirectToPage("View", new { id = PHIEUDUYETTHU.SO_PHIEU });
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
        private void LoadData(string id = "", string lydo = "", string trangthai = "")
        {
            if (string.IsNullOrEmpty(trangthai))
            {
                //List HoaDon
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string manhanvien = User.FindFirst("UserId")?.Value;
                string sSQL = "EXEC SP_GET_PHIEUDAT " + cls_Main.SQLString(manhanvien) + ", " + cls_Main.SQLString(id);
                DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                foreach (DataRow dr in dt.Rows)
                {
                    DONDATModels item = new DONDATModels();
                    item.PHIEUDATHANG = dr["MAPDNDC"].ToString();
                    item.NHANVIEN_DATHANG = dr["TENNHANVIEN"].ToString();
                    item.NGAYLAP = String.Format("{0:dd/MM/yyyy}", dr["NGAYLAPPHIEU"]);
                    item.THANHTIEN = int.Parse(dr["TONGTIEN"].ToString()).ToString("N0");
                    item.KHACHHANG = dr["KHACHHANG"].ToString();
                    item.CHON = "false";
                    item.DATRA = "0";
                    item.NHAPTHANHTOAN = dr["DATRA"].ToString().Replace(",", "");
                    item.CONLAI = (double.Parse(item.THANHTIEN.Replace(",", "")) - double.Parse(item.DATRA.Replace(",", "")) - double.Parse(item.NHAPTHANHTOAN.Replace(",", ""))).ToString("N0");
                    ListDONDAT.Add(item);
                }
                // Phieu thanh toan
                sSQL = "EXEC SP_FINA_SELECT_DATA @LOAI=66, @MAPHIEU=" + cls_Main.SQLString(id);
                dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                PHIEUDUYETTHU.SO_PHIEU = dt.Rows[0]["MAPDNDT"].ToString();
                PHIEUDUYETTHU.NHAN_VIEN = dt.Rows[0]["TENNHANVIEN"].ToString();
                PHIEUDUYETTHU.NGAYLAPPHIEU = String.Format("{0:dd/MM/yyyy}", dt.Rows[0]["NGAYLAPPHIEU"]);
                PHIEUDUYETTHU.LY_DO = dt.Rows[0]["LYDO"].ToString();
                PHIEUDUYETTHU.TRANG_THAI = dt.Rows[0]["TRANGTHAI"].ToString();
                PHIEUDUYETTHU.SOTIEN = int.Parse(dt.Rows[0]["TONGTIEN"].ToString()).ToString("N0");
                PHIEUDUYETTHU.listDONDAT = new List<DONDATModels>();
                foreach (DataRow dr in dt.Rows)
                {
                    DONDATModels item = new DONDATModels();
                    item.PHIEUDATHANG = dr["HANGHOA"].ToString();
                    item.NHANVIEN_DATHANG = dr["TENNHANVIEN_DH"].ToString();
                    item.THANHTIEN = int.Parse(dr["THANHTIEN"].ToString()).ToString("N0");
                    item.KHACHHANG = dr["TEN"].ToString();
                    item.NGAYLAP = String.Format("{0:dd/MM/yyyy}", dr["NGAYLAPPHIEU"]);
                    item.DATRA = "0";
                    item.NHAPTHANHTOAN = dr["DATRA"].ToString().Replace(",", "");
                    item.CONLAI = (double.Parse(item.THANHTIEN.Replace(",", "")) - double.Parse(item.DATRA.Replace(",", "")) - double.Parse(item.NHAPTHANHTOAN.Replace(",", ""))).ToString("N0");
                    item.STT = (1 + dt.Rows.IndexOf(dr)).ToString();

                    PHIEUDUYETTHU.listDONDAT.Add(item);

                    DONDATModels item1 = ListDONDAT.Find(_item => _item.PHIEUDATHANG == item.PHIEUDATHANG);
                    if (item1 != null)
                        item1.CHON = "true";
                }
            }
            else
            {
                PHIEUDUYETTHU.SO_PHIEU = id;
                PHIEUDUYETTHU.NGAYLAPPHIEU = DateTime.Now.ToString("dd/MM/yyyy");
                PHIEUDUYETTHU.LY_DO = lydo;
                if (TempData["PHIEUTHANHTOAN_HOADON"] != null)
                {
                    PHIEUDUYETTHU.listDONDAT = JsonConvert.DeserializeObject<List<DONDATModels>>(TempData["PHIEUTHANHTOAN_HOADON"].ToString());
                }
                else
                {
                    PHIEUDUYETTHU.listDONDAT = new List<DONDATModels>();
                }
                if (TempData["PHIEUTHANHTOAN_DONDAT"] != null)
                {
                    ListDONDAT = JsonConvert.DeserializeObject<List<DONDATModels>>(TempData["PHIEUTHANHTOAN_DONDAT"].ToString());
                }
                PHIEUDUYETTHU.listDONDAT.Clear();
                foreach (var item in ListDONDAT)
                {
                    item.CONLAI = (double.Parse(item.THANHTIEN.Replace(",", "")) - double.Parse(item.NHAPTHANHTOAN.Replace(",", ""))).ToString("N0");
                    item.NHAPTHANHTOAN = double.Parse(item.NHAPTHANHTOAN.Replace(",", "")).ToString("N0");
                    if (item.CHON == "true")
                    {
                        PHIEUDUYETTHU.listDONDAT.Add(item);
                    }
                }
            }

            PHIEUDUYETTHU.SOTIEN = "0";
            foreach (var item in PHIEUDUYETTHU.listDONDAT)
            {
                if (item.PHIEUDATHANG != null)
                {
                    item.STT = PHIEUDUYETTHU.listDONDAT.IndexOf(item).ToString();
                    PHIEUDUYETTHU.SOTIEN = (int.Parse(PHIEUDUYETTHU.SOTIEN.Replace(",", "")) + int.Parse(item.NHAPTHANHTOAN.Replace(",", ""))).ToString("N0");
                }
            }
        }
        private string ChiTietPhieuThu()
        {
            sTongTien = 0;
            string sSQL = "";
            string manhanvien = User.FindFirst("UserId")?.Value;
            foreach (var item in PHIEUDUYETTHU.listDONDAT)
            {
                if (item.PHIEUDATHANG != "")
                {
                    sTongTien += long.Parse(item.NHAPTHANHTOAN.Replace(",", ""));
                    long sTienPhaiThanhToan = long.Parse(item.THANHTIEN.Replace(",", "")) - long.Parse(item.DATRA.Replace(",", ""));
                    sSQL += "INSERT INTO CHITIETPHIEUDENGHIDUYETTHU(PHIEUDENGHI,HANGHOA,SOLUONG,DONGIA,THANHTIEN,TEN_DONVITINH, DUYET, NHANVIEN,GHICHU)" + "\n";
                    sSQL += "VALUES(";
                    sSQL += cls_Main.SQLString(PHIEUDUYETTHU.SO_PHIEU) + ",";
                    sSQL += cls_Main.SQLString(item.PHIEUDATHANG) + ",";
                    sSQL += cls_Main.SQLString("1") + ",";
                    sSQL += cls_Main.SQLString(item.THANHTIEN.Replace(",", "")) + ",";
                    sSQL += cls_Main.SQLString(item.THANHTIEN.Replace(",", "")) + ",";
                    sSQL += cls_Main.SQLStringUnicode("") + ",";
                    sSQL += cls_Main.SQLBit(false) + ",";
                    sSQL += cls_Main.SQLStringUnicode(manhanvien) + ",";
                    sSQL += cls_Main.SQLStringUnicode("") + ")";
                    sSQL += "\n";

                    sSQL += "INSERT INTO CHITIETTHANHTOAN(PK_PHIEUKHOID,TONGTIENCANTT,SOTIENTHANHTOAN,SOTIENCONLAI,DOTTHANHTOAN,NGAYTHANHTOAN, FLAG, THU_CHI,MA_HOADON,ISTREHEN)" + "\n";
                    sSQL += "VALUES(";
                    sSQL += cls_Main.SQLString(item.PHIEUDATHANG) + ",";
                    sSQL += cls_Main.SQLString(sTienPhaiThanhToan.ToString()) + ",";
                    sSQL += cls_Main.SQLString(item.NHAPTHANHTOAN.Replace(",", "")) + ",";
                    sSQL += cls_Main.SQLString(item.CONLAI.Replace(",", "")) + ",";
                    sSQL += cls_Main.SQLString("0") + ",";
                    sSQL += cls_Main.SQLString(String.Format("{0:yyyyMMdd HH:mm:ss}", cls_Global.gdServerDate)) + ",";
                    sSQL += cls_Main.SQLBit(false) + ",";
                    sSQL += cls_Main.SQLString("1") + ",";
                    sSQL += cls_Main.SQLString(PHIEUDUYETTHU.SO_PHIEU) + ",";
                    sSQL += cls_Main.SQLString("0") + ")";
                    sSQL += "\n";

                    if (item.CONLAI != "0")
                    {
                        sSQL += "UPDATE PHIEUDATHANG SET SOGIAODICH=" + cls_Main.SQLString(item.CONLAI.Replace(",", ""));
                        sSQL += "WHERE MAPDNDC = " + cls_Main.SQLString(item.PHIEUDATHANG);
                        sSQL += "\n";
                    }
                    else if (item.CONLAI == "0")
                    {
                        sSQL += "UPDATE PHIEUDATHANG SET SOGIAODICH=" + cls_Main.SQLString("");
                        sSQL += "WHERE MAPDNDC = " + cls_Main.SQLString(item.PHIEUDATHANG);
                        sSQL += "\n";
                    }
                }
            }
            return sSQL;
        }

        private bool KiemTra(List<DONDATModels> L)
        {
            bool kq = false;
            foreach (var item in L)
            {
                if (item.NHAPTHANHTOAN == "0")
                {
                    kq = true;
                    ModelState.AddModelError(string.Empty, "Đơn hàng " + item.PHIEUDATHANG + " chưa Xác Nhận số tiền nộp");
                }
                else if (double.Parse(item.NHAPTHANHTOAN.Replace(",", "")) + double.Parse(item.DATRA.Replace(",", "")) > double.Parse(item.THANHTIEN.Replace(",", "")))
                {
                    kq = true;
                    ModelState.AddModelError(string.Empty, "Đơn hàng " + item.PHIEUDATHANG + " nhập số tiền nộp lớn hơn số tiền phải thu");
                }
            }

            return kq;
        }
    }
}
