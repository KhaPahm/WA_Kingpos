using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.Nghiepvu.ThanhToanCongNoFina
{
    [Authorize]
    public class IndexModel : PageModel
    {
        [BindProperty]
        public PhieuDuyetThu PHIEUDUYETTHU { get; set; } = new PhieuDuyetThu();

        [BindProperty]
        public List<DONDATModels> ListDONDAT { get; set; } = new List<DONDATModels>();

        long sTongTien = 0;
        public IActionResult OnGet(string lydo)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowAdd("14102304", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang 
                LoadData(lydo);
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "ThanhToanCongNoFina Add", ex.ToString(), "0");
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
                    return RedirectToPage("Index", new {lydo = PHIEUDUYETTHU.LY_DO });
                }
                if (submitButton == "btn3Create")
                {
                    if (PHIEUDUYETTHU.SO_PHIEU == "")
                    {
                        ModelState.AddModelError(string.Empty, "Số phiếu không được để trống");
                    }
                    if (PHIEUDUYETTHU.listDONDAT == null || PHIEUDUYETTHU.listDONDAT.Count == 0)
                    {
                        ModelState.AddModelError(string.Empty, "Phải chọn ít nhất một đơn hàng");
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
                        LoadData();
                        return Page();
                    }
                    else
                    {
                        string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                        string sSQL = "";
                        string manhanvien = User.FindFirst("UserId")?.Value;
                        //check trung so phieu trc khi luu
                        sSQL = "select * from CN_CONGNO_PHIEUDENGHIDUYETTHU where MAPDNDT = " + cls_Main.SQLString(PHIEUDUYETTHU.SO_PHIEU);
                        DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                        if (dt.Rows.Count > 0)
                        {
                            sSQL = "SELECT [dbo].[fc_NewCode_PHIEUDENGHIDUYETTHU]()";
                            dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                            PHIEUDUYETTHU.SO_PHIEU = dt.Rows[0][0].ToString();

                        }
                        //
                        string SQL_ChiTietPhieuChi = ChiTietPhieuThu();
                        string SQL_PhieuChi = string.Format("EXEC sp_CN_CONGNO_PHIEUDENGHIDUYETTHU_INSERT_NS @MAPDNDT='{0}',@MADOITUONG='{1}',@MA_HOADON='{2}',@PHIEUKHO_ID='{3}',@MANV='{4}',@MALOAI='{5}',@LYDO=N'{6}',@SOTIEN='{7}',@KEMTHEO=N'{8}',@TONGTIEN='{9}',@HOTEN=N'{10}',@DIACHI=N'{11}',@MANVMUON='{12}',@MATT='{13}',@NGAYTHANHTOAN='{14:yyyyMMdd HH:mm:ss}',@TENNGANHANGCHUYEN=N'{15}',@SOTAIKHOANCHUYEN=N'{16}',@TENNGANHANGNHAN=N'{17}',@SOTAIKHOANNHAN=N'{18}',@SOGIAODICH=N'{19}',@HOTENGIAODICH=N'{20}',@NGUONTHU='{21}'", PHIEUDUYETTHU.SO_PHIEU, "0", "", "", manhanvien, "02", PHIEUDUYETTHU.LY_DO, sTongTien, "", sTongTien, "", "", "", "02", DateTime.Now, "", "", "", "", "", "", "1");

                        sSQL = "";
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
                cls_Main.WriterLog(cls_Main.sFilePath, "PhieuDatHangFina Add", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        private string ChiTietPhieuThu()
        {
            sTongTien = 0;
            string sSQL = "";
            string manhanvien = User.FindFirst("UserId")?.Value;
            foreach (var item in PHIEUDUYETTHU.listDONDAT)
            {
                if (item.PHIEUDATHANG != "" && item.NHAPTHANHTOAN !="0")
                {
                    sTongTien += long.Parse(item.NHAPTHANHTOAN.Replace(",", ""));
                    long sTienPhaiThanhToan = long.Parse(item.THANHTIEN.Replace(",", "")) - long.Parse(item.DATRA.Replace(",", ""));
                    sSQL += "INSERT INTO CHITIETPHIEUDENGHIDUYETTHU(PHIEUDENGHI,HANGHOA,SOLUONG,DONGIA,THANHTIEN,TEN_DONVITINH, DUYET, NHANVIEN,GHICHU)" + "\n";
                    sSQL += "VALUES(";
                    sSQL += cls_Main.SQLString(PHIEUDUYETTHU.SO_PHIEU) + ",";
                    sSQL += cls_Main.SQLString(item.PHIEUDATHANG) + ",";
                    sSQL += cls_Main.SQLString("1") + ",";
                    sSQL += cls_Main.SQLString(item.NHAPTHANHTOAN.Replace(",", "")) + ",";
                    sSQL += cls_Main.SQLString(item.NHAPTHANHTOAN.Replace(",", "")) + ",";
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
        private void LoadData(string lydo = "")
        {
            //SoPhieu
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "SELECT [dbo].[fc_NewCode_PHIEUDENGHIDUYETTHU]()";
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            if (dt.Rows.Count > 0)
            {
                PHIEUDUYETTHU.SO_PHIEU = dt.Rows[0][0].ToString();
            }
            else
            {
                PHIEUDUYETTHU.SO_PHIEU = "";
            }
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
            //Ds don hang
            if (TempData["PHIEUTHANHTOAN_DONDAT"] != null)
            {
                ListDONDAT = JsonConvert.DeserializeObject<List<DONDATModels>>(TempData["PHIEUTHANHTOAN_DONDAT"].ToString());
            }
            else
            {
                LoadHoaDon();
            }
            PHIEUDUYETTHU.listDONDAT.Clear ();
            foreach (var item in ListDONDAT)
            {
                item.CONLAI = (double.Parse(item.THANHTIEN.Replace(",", "")) - double.Parse(item.DATRA.Replace(",", "")) - double.Parse(item.NHAPTHANHTOAN.Replace(",", ""))).ToString("N0");
                if (item.CHON == "true")
                {
                    PHIEUDUYETTHU.listDONDAT.Add(item);
                }
            }
            PHIEUDUYETTHU.SOTIEN = "0";
            foreach (var item in PHIEUDUYETTHU.listDONDAT)
            {
                if (item.PHIEUDATHANG != null)
                {
                    item.STT = PHIEUDUYETTHU.listDONDAT.IndexOf(item).ToString();
                    PHIEUDUYETTHU.SOTIEN = (double.Parse(PHIEUDUYETTHU.SOTIEN.Replace(",", "")) + double.Parse(item.NHAPTHANHTOAN.Replace(",", ""))).ToString("N0");
                }
            }
        }
        private void LoadHoaDon()
        {
            //List HoaDon
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string manhanvien = User.FindFirst("UserId")?.Value;
            string sSQL = "EXEC SP_GET_PHIEUDAT " + cls_Main.SQLString(manhanvien);
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                DONDATModels item = new DONDATModels();
                item.CHON = dr["CHON"].ToString();
                item.PHIEUDATHANG = dr["MAPDNDC"].ToString();
                item.NHANVIEN_DATHANG = dr["TENNHANVIEN"].ToString();
                item.NGAYLAP = String.Format("{0:dd/MM/yyyy}", dr["NGAYLAPPHIEU"]);
                item.THANHTIEN = int.Parse(dr["TONGTIEN"].ToString()).ToString("N0");
                item.KHACHHANG = dr["KHACHHANG"].ToString();
                item.DATRA = int.Parse(dr["DATRA"].ToString()).ToString("N0");
                item.NHAPTHANHTOAN = "0";
                item.CONLAI = (double.Parse(item.THANHTIEN.Replace(",", "")) - double.Parse(item.DATRA.Replace(",", "")) - double.Parse(item.NHAPTHANHTOAN.Replace(",", ""))).ToString("N0");
                ListDONDAT.Add(item);
            }
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
