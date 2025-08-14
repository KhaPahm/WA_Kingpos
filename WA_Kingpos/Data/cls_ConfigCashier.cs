using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace WA_Kingpos.Data
{
    public class cls_ConfigCashier
    {
        /// <summary>
        /// id Shop
        /// </summary>
        public static string idShop = "";
        /// <summary>
        /// Name Shop
        /// </summary>
        public static string nameShop = "";
        ///<summary>
        ///id Cashier 
        ///
        ///</summary>
        public static string idCashier = "";
        ///<summary>
        ///Name Cashier 
        ///
        ///</summary>
        public static string nameCashier = ""; 
        ///<summary>
        ///id warehouse
        ///</summary>
        public static string idWarehouse = "";
        ///<summary>
        ///name warehouse
        ///</summary>
        public static string nameWarehouse = "";
        ///<summary>
        ///id sector
        ///
        ///</summary>
        public static string idSector = "";
        /// <summary>
        /// id member 
        /// </summary>
        public static string idmember = "";
        /// <summary>
        /// Ten ca ban
        /// </summary>
        public static string sTenCB = "";
        /// <summary>
        /// Gio bat dau ca ban
        /// </summary>
        public static string sGioBD = "";
        /// <summary>
        /// Gio ket thuc ca ban
        /// </summary>
        public static string sGioKT = "";
        /// <summary>
        /// Check full ca ban
        /// </summary>
        public static string scheckCaban = "";
        /// <summary>
        /// Chuyen ban co in xuong bep
        /// </summary>
        public static string sInbep_Chuyenban = "0";
        /// <summary>
        /// Có dùng mật khẩu bán hàng hay không
        /// </summary>
        public static string sDungmatkhaubanhang = "0";
        /// <summary>
        /// Có in mã vạch khi bán hàng hay không
        /// </summary>
        public static string sIn_Mavach_Fastfood = "0";
        /// <summary>
        /// Có in số number khi bán hàng hay không
        /// </summary>
        public static string sIn_Number_Fastfood = "0";
        /// <summary>
        /// Chi mã vạch khi bán hàng có giao hàng
        /// </summary>
        public static string sIn_Mavach_Fastfood_Giaohang = "0";
        /// <summary>
        /// Load giá 2 khi bán hàng có giao hàng
        /// </summary>
        public static string sGiaban2_Giaohang = "0";
        /// <summary>
        /// In tạm tính ngay không cần hiện form in tam tinh nữa
        /// </summary>
        public static string sShow_Intamtinh = "0";
        /// <summary>
        /// Auto hiện form ghi chú mà ko cần bấm nút nữa
        /// </summary>
        public static string sShow_Ghichu = "0";
        /// <summary>
        /// cấu hình thời gian ngồi free buffet
        /// </summary>
        public static string sCauHinhThoiGian = "0";
        /// <summary>
        /// auto day bill ve cho huy viet nam
        /// </summary>
        public static string sAutodaybill = "0";
        /// <summary>
        /// auto day bill ve cho huy viet nam
        /// </summary>
        public static string sInbilltong = "0";
        /// Có dùng mật khẩu IN LẠI HÓA ĐƠN
        /// </summary>
        public static string sINLAIHOADON = "0";
        /// Có dùng mật khẩu LẤY LẠI HÓA ĐƠN
        /// </summary>
        public static string sLAYLAIHOADON = "0";
        /// Có dùng mật khẩu HỦY BÀN
        /// </summary>
        public static string sHUYBAN = "0";
        /// Có dùng mật khẩu HỦY MÓN
        /// </summary>
        public static string sHUYMON = "0";
        /// Có dùng mật khẩu MỞ KÉT
        /// </summary>
        public static string sMOKET = "0";
        /// Có dùng mật khẩu SỬA GIÁ
        /// </summary>
        public static string sSUAGIA = "0";
        /// Có dùng mật khẩu CHỌN GIÁ
        /// </summary>
        public static string sCHONGIA = "0";
        /// Có dùng mật khẩu BỎ KHÓA BÀN
        /// </summary>
        public static string sBOKHOABAN = "0";
        /// Có dùng mật khẩu CHIẾT KHẤU
        /// </summary>
        public static string sCHIETKHAU = "0";
        /// Có dùng mật khẩu KẾT CA
        /// </summary>
        public static string sKETCA = "0";
        /// Có dùng mật khẩu BÁN GIÁ 2
        /// </summary>
        public static string sBANGIA2 = "0";
        /// <summary>
        /// ID cua hang huy viet nam 202: Foodhall,203: IKI,204: Shilla
        /// </summary>
        public static string sStore_ID = "0";
        /// <summary>
        /// SOLUONGKHACH có lưu thông tin số lượng khách hay không
        /// </summary>
        public static string sSOLUONGKHACH = "0";
        /// <summary>
        /// HUYMONNHANH có cho huy mon nhanh hay ko
        /// </summary>
        public static string sHUYMONNHANH = "0";
        /// <summary>
        /// KHOANHAPTHE có cho huy mon nhanh hay ko
        /// </summary>
        public static string sKHOANHAPTHE = "0";
        /// <summary>
        /// ID menu  cua hang huy viet nam 202: Foodhall,203: IKI,204: Shilla
        /// </summary>
        public static string sMenuID = "0";
        /// <summary>
        /// IP Thanh toan VN PAY
        /// </summary>
        public static string VNPAY_IP = "0";
        /// <summary>
        /// VN PAY
        /// </summary>
        public static string VNPAY_secretKey = "0";
        /// <summary>
        /// VN PAY
        /// </summary>
        public static string VNPAY_appId = "0";
        /// <summary>
        /// VN PAY
        /// </summary>
        public static string VNPAY_merchantName = "0";
        /// <summary>
        /// VN PAY
        /// </summary>
        public static string VNPAY_merchantCode = "0";
        /// <summary>
        /// VN PAY
        /// </summary>
        public static string VNPAY_terminalId = "0";
        /// <summary>
        /// 07/06/2019: 1:Cộng điểm trước khi chiết khấu. 0:Cộng điểm sau khi chiết khấu
        /// </summary>
        public static string sCONGDIEM_KHTT = "0";
        /// <summary>
        /// Mode STARLIGHT khách hàng thân thiết rule khác
        /// </summary>
        public static string sSTARLIGHT = "0";
        /// <summary>
        /// Mode helio dung the noi bo qua api
        /// </summary>
        public static string sHelio = "0";
        /// <summary>
        /// Ten ban co in dam chu to ra hay khong
        /// </summary>
        public static string sIndam = "0";
        /// <summary>
        /// Mode quan ly hay ban hang
        /// </summary>
        public static string sBANHANG = "0";
        /// <summary>
        /// Ten ban co in dam chu to ra hay khong
        /// </summary>
        public static string sInhuyban = "0";
        /// <summary>
        /// GIABANTRUOCTHUE
        /// </summary>
        public static string sGIABANTRUOCTHUE = "0";
        /// <summary>
        /// APDUNGBANGGIATHEOKHUVUC
        /// </summary>
        public static string sAPDUNGBANGGIATHEOKHUVUC = "0";
        /// <summary>
        /// IN A5
        /// </summary>
        public static string sIN_GIAYLON = "0";
        /// Thoi gian bao co bill dat hang moi
        /// </summary>
        public static string sThoigianbao_Billdoi = "60000";
        /// <summary>
        /// 1-Imgur,2-ImageShack
        /// </summary>
        public static string sApi_upload = "0";
        /// <summary>
        /// key dùng upload lên ImageShack
        /// </summary>
        public static string sKEY_ImageShack = "0";
        /// <summary>
        /// key dùng upload lên Imgur
        /// </summary>
        public static string sKEY_Imgur = "0";
        /// <summary>
        /// ZALOPAY App_id
        /// </summary>
        public static string sApp_id = "0";
        /// <summary>
        /// ZALOPAY Key1
        /// </summary>
        public static string sKey1 = "";
        /// <summary>
        /// ZALOPAY CreateOrderUrl
        /// </summary>
        public static string sCreateOrderUrl = "";
        /// <summary>
        /// ZALOPAY QueryOrderUrl
        /// </summary>
        public static string sQueryOrderUrl = "";
        /// <summary>
        /// ZALOPAY RefundUrl
        /// </summary>
        public static string sRefundUrl = "";
        /// <summary>
        /// ZALO Access_token
        /// </summary>
        public static string sAccess_token = "";
        /// <summary>
        /// ZALO SendMessageUrl
        /// </summary>
        public static string sSendMessageUrl = "";
        /// <summary>
        /// ZALO GetprofileUrl
        /// </summary>
        public static string sGetProfileUrl = "";
        /// <summary>
        /// ZALO GetfollowersUrl
        /// </summary>
        public static string sGetFollowersUrl = "";
        /// <summary>
        /// ZALO UploadImageUrl
        /// </summary>
        public static string sUploadImageUrl = "";
        /// <summary>
        /// ZALO UploadFileUrl
        /// </summary>
        public static string sUploadFileUrl = "";
        /// </summary>
        /// MOMO partnerCode
        public static string partnerCode = "0";
        /// </summary>
        /// MOMO publicKey
        public static string publicKey = "0";
        /// </summary>
        /// MOMO secretKey
        public static string secretKey = "0";
        /// </summary>
        /// MOMO url_momo
        public static string url_momo = "0";
        /// </summary>
        /// MOMO storeSlug
        public static string storeSlug = "0";
        /// <summary>
        /// cho phep auto monthem khi vua chon mon hay ko
        public static string sAUTOMONTHEM = "0";
        /// <summary>
        ///  /// <summary>
        /// set mode khi load phần mềm
        public static string sMode = "0";
        /// <summary>
        ///   /// set show from tạm ứng khi mở bán hàng
        public static string sShowTamUng = "0";
        /// <summary>
        /// Ẩn/Hiện form nhập ghi chú chiết khấu
        /// </summary>
        public static string sShow_Ghichu_ChietKhau = "0";
        /// <summary>
        /// Mode tách bida và karaoke, 0.karaoke, 1.Bida
        /// </summary>
        public static string sMode_Bida_Kara = "0";
        /// <summary>
        /// Mode tách bida và karaoke, 0.karaoke, 1.Bida
        /// </summary>
        public static string sMode_Phanquyennhanvien = "0";
        /// <summary>
        /// VN PAY
        /// </summary>
        public static string VNPAY_merchantType = "0";
        /// </summary>
        /// MOMO accessKey
        public static string accessKey = "0";
        /// MOMO: orderGroupId phân biệt của hàng tạo hóa đơn
        /// </summary>
        public static string sOrderGroupId = "0";
        /// </summary>
        /// MOMO storeId
        public static string storeId = "0";
        /// </summary>
        /// MOMO URLKiemTraGD
        public static string sURLKiemTraGD = "0";
        /// </summary>
        /// MOMO URLTaoQRCODE
        public static string sURLTaoQRCODE = "0";
        /// <summary>
        /// HOA DON DIEN TU
        /// </summary>
        public static string sHoaDon_DienTu = "0";
        public static string sWebServiceURL = "";
        public static string sCmdType = "101";
        public static string sVATType = "";
        public static string sThuMuc_HoaDon = "";
        public static string sBkavPartnerGUID = "";
        public static string sBkavPartnerToken = "";
        public static bool HDDT_eHOADON = false;
        public static string sLOAI_HDDT = "";
        public static string sUsername_service = "";
        public static string sPass_service = "";
        public static string sUsername_nhanvien = "";
        public static string sPass_nhanvien = "";
        public static string sPattern = "";
        public static string sAPI_Khachhang = "btnthanhthanhphatadmindemo";
        public static string sTenHangHoa_HDDT = "";
        public static string sLayTenHanhKhach_HDDT = "";
        //public static string sGIABANTRUOCTHUE = "";
        public static string sTACHVE_HDDT = "";
        //public static string idShop = "";
        //public static String nameShop = "";
        //public static string idCashier = "";
        //public static string nameCashier = "";
        //public static string idWarehouse = "";
        //public static string nameWarehouse = "";
        public static string sBkavPartnerGUID2 = "";
        public static string sBkavPartnerToken2 = "";
        public static string sXuatHDTheoDiemDi = "";
        public static string sPartnerName = "";
        public static string sTen_KhachKhongLayHoaDon = "";
        public static string sEmail_KhachKhongLayHoaDon = "";
        public static string sLAY_TOKEN = "";
        public static string sLAY_THONGTIN_PHATHANH = "";
        public static string sTAO_SUA_HD = "";
        public static string sDOWNLOAD_HD_CHUAPHATHANH = "";
        public static string sDIACHI_SERVER_KYHD = "";
        public static string sPHATHANH_HD = "";
        public static string sLAY_THONGTIN_HD = "";
        public static string sDOWNLOAD_HD_DAPHATHANH = "";
        public static string sLAY_TRANGTHAI_HD = "";
        public static string sGUIEMAIL_HD_DAPHATHANH = "";
        public static string sCHUYENDOI_HD_DAPHATHANH = "";
        public static string sHUY_HD_DAPHATHANH = "";
        public static string sPINCODE = "";
        public static string sMST = "";
        public static string sCONGTY = "";
        public static string sDIACHI = "";
        public static string Token = "";
        public static string sTAO_VA_KY_HD = "";
        public static string sKY_HIEU_HD_VE = "";
        public static string sKY_HIEU_HD = "";
        public static string sLAY_HD_PDF = "";
        public static string sLOAI_KYPHATHANH = "0";
        public static string sXOA_HUY_HD = "";
        public static string sUPDATE_HD = "";
        public static string sDIEUCHINH_HD = "";
        public static string sTHAY_THE_HD = "";
        public static string sGetQRCode_VNPAY = "";
        public static string sCheckTrans_VNPAY = "";
        public static string sSecretKey_CheckTrans = "";
        public static string sKY_HIEU_HD_COMBO = "";
        public static string sDUNG_RIENG_MAT_KHAU = "0";
        public static string sTHEMHANGHOA = "0";
        public static string sTRAHANG = "0";
        public static string sTAMUNG = "0";
        public static string sTRAHANGQRCODE = "0";
        public static string sCauhinhsudung = "0";
        /// <summary>
        /// TÙY CHỌN IN QR CODE RA GIẤY								  
        /// </summary>
        public static string sIN_QRCODE = "0";
        /// <summary>
        /// TÙY CHỌN IN QR CODE RA GIẤY								  
        /// </summary>
        public static string sTTNB_TIENTOITHIEU = "0";
        /// <summary>
        /// ten khach hang
        /// </summary>
        public static string sTENKHACHHANG = "";
        //header report
        public static string sTENCONGTY = "";
        public static string sDIACHICONGTY = "";
        public static string sSODIENTHOAICONGTY = "";





        /// <summary>
        /// Check full ca ban
        /// </summary>
        public static bool checkCaban()
        {
            bool  bResult = false;
            if (scheckCaban == "")
            {
                try
                {
                    string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                    scheckCaban = WA_Kingpos.Data.cls_Main.ReturnDataTable_NoLock("Select GIATRI From CAUHINH where TEN='CAUHINHCABAN'", sConnectionString_live).Rows[0][0].ToString();
                }
                catch
                {
                }
            }
            bResult = scheckCaban == "1" ? true : false;
            return bResult;
        }
        /// <summary>
        /// kiểm tra có đủ thông tin trước khi vô bán hàng
        /// </summary>
        public static bool checkBanHang()
        {
            //check cấu hình quầy
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            if (cls_ConfigCashier.idShop == "")
            {
                LoadCauHinh();
                if (cls_ConfigCashier.idShop == "")
                {
                    return false;
                }
            }
            //Check cửa hàng , kho , quầy
            string sql = "";
            sql += "select a.ten_cuahang,b.ten_kho,c.ten_quay" + "\n";
            sql += "from cuahang a, kho b, quay c" + "\n";
            sql += "where a.ma_cuahang=b.ma_cuahang" + "\n";
            sql += "and b.ma_kho=c.ma_kho" + "\n";
            sql += "and a.ma_cuahang=" + cls_Main.SQLString(cls_ConfigCashier.idShop) + "\n";
            sql += "and b.ma_kho=" + cls_Main.SQLString(cls_ConfigCashier.idWarehouse) + "\n";
            sql += "and c.ma_quay=" + cls_Main.SQLString(cls_ConfigCashier.idCashier) + "\n";
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sql, sConnectionString_live);
            if (dt.Rows.Count <= 0)
            {
                return false ;
            }
            //Check ca bán
            if (checkCaban())
                cls_ConfigCashier.sTenCB = "";
                cls_ConfigCashier.sGioBD = "";
                cls_ConfigCashier.sGioKT = "";
            return true ;
        }
        /// <summary>
        /// Lay thong tin cau hinh ban hang
        /// </summary>
        public static void LoadCauHinh()
        {
            try
            {
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                if (File.Exists("/systeminfo.txt"))
                {
                    StreamReader str = new StreamReader("/systeminfo.txt");
                    string line = str.ReadLine();
                    string[] value = line.Split(',');
                    cls_ConfigCashier.idShop = value[0].ToString();
                    cls_ConfigCashier.nameShop = value[1].ToString();
                    cls_ConfigCashier.idCashier = value[2].ToString();
                    cls_ConfigCashier.nameCashier = value[3].ToString();
                    cls_ConfigCashier.idWarehouse = value[4].ToString();
                    cls_ConfigCashier.nameWarehouse = value[5].ToString();
                }
                DataTable dtCauhinh = cls_Main.ReturnDataTable_NoLock("Select * From CAUHINH", sConnectionString_live);
                try
                {
                    //08/07/2016 them lay thong tin co in bep khi chuyen ban
                    //sInbep_Chuyenban = clsMain.ReturnDataTable_NoLock("Select GIATRI From CAUHINH where TEN='INBEP_CHUYENBAN'").Rows[0][0].ToString();
                    sInbep_Chuyenban = dtCauhinh.Select("TEN='INBEP_CHUYENBAN'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    //20/12/2016 them lay thong tin co dung mat khau ban hang hay khong
                    //sDungmatkhaubanhang = clsMain.ReturnDataTable_NoLock("Select GIATRI From CAUHINH where TEN='DUNGMATKHAUBANHANG'").Rows[0][0].ToString();
                    sDungmatkhaubanhang = dtCauhinh.Select("TEN='DUNGMATKHAUBANHANG'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    //31/08/2017 them lay thong tin co in ma vach hang khi ban fastfood hay khong
                    // sIn_Mavach_Fastfood = clsMain.ReturnDataTable_NoLock("Select GIATRI From CAUHINH where TEN='INMAVACHFASTFOOD'").Rows[0][0].ToString();
                    sIn_Mavach_Fastfood = dtCauhinh.Select("TEN='INMAVACHFASTFOOD'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    //31/08/2017 them lay thong tin co in ma vach hang khi ban fastfood hay khong
                    //sIn_Number_Fastfood = clsMain.ReturnDataTable_NoLock("Select GIATRI From CAUHINH where TEN='INNUMBERFASTFOOD'").Rows[0][0].ToString();
                    sIn_Number_Fastfood = dtCauhinh.Select("TEN='INNUMBERFASTFOOD'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    //14/06/2018 cau hinh chi in ma vach khi có giao hàng 
                    // sIn_Mavach_Fastfood_Giaohang = clsMain.ReturnDataTable_NoLock("Select GIATRI From CAUHINH where TEN='INMAVACHFASTFOOD_GIAOHANG'").Rows[0][0].ToString();
                    sIn_Mavach_Fastfood_Giaohang = dtCauhinh.Select("TEN='INMAVACHFASTFOOD_GIAOHANG'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    //14/06/2018 cau hinh chi in ma vach khi có giao hàng 
                    // sGiaban2_Giaohang = clsMain.ReturnDataTable_NoLock("Select GIATRI From CAUHINH where TEN='GIABAN2_GIAOHANG'").Rows[0][0].ToString();
                    sGiaban2_Giaohang = dtCauhinh.Select("TEN='GIABAN2_GIAOHANG'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    //21/06/2018 show from in tạm tính hay không
                    //sShow_Intamtinh = clsMain.ReturnDataTable_NoLock("Select GIATRI From CAUHINH where TEN='SHOW_INTAMTINH'").Rows[0][0].ToString();
                    sShow_Intamtinh = dtCauhinh.Select("TEN='SHOW_INTAMTINH'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    //05/07/2018 auto show from ghi chú hay không
                    //sShow_Ghichu = clsMain.ReturnDataTable_NoLock("Select GIATRI From CAUHINH where TEN='LUONHIENGHICHU'").Rows[0][0].ToString();
                    sShow_Ghichu = dtCauhinh.Select("TEN='LUONHIENGHICHU'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    //13/06/2018 them lay thong tin co cau hinh thoi gian hay khong
                    // sCauHinhThoiGian = clsMain.ReturnDataTable_NoLock("Select GIATRI From CAUHINH where TEN='CAUHINHTHOIGIAN'").Rows[0][0].ToString();
                    sCauHinhThoiGian = dtCauhinh.Select("TEN='CAUHINHTHOIGIAN'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    //31/07/2018 them lay thong tin auto day bill 
                    //sAutodaybill = clsMain.ReturnDataTable_NoLock("Select GIATRI From CAUHINH where TEN='AUTODAYBILL'").Rows[0][0].ToString();
                    sAutodaybill = dtCauhinh.Select("TEN='AUTODAYBILL'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    //23/08/2018 them lay thong tin in bill tong khi order 
                    // sInbilltong = clsMain.ReturnDataTable_NoLock("Select GIATRI From CAUHINH where TEN='INBILLTONG'").Rows[0][0].ToString();
                    sInbilltong = dtCauhinh.Select("TEN='INBILLTONG'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    //29/08/2018 them lay thong tin co dung MẬT KHẨU IN LẠI HÓA ĐƠN
                    // sINLAIHOADON = clsMain.ReturnDataTable_NoLock("Select GIATRI From CAUHINH where TEN='MK_INLAIHD'").Rows[0][0].ToString();
                    sINLAIHOADON = dtCauhinh.Select("TEN='MK_INLAIHD'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    //29/08/2018 them lay thong tin co dung MẬT KHẨU LẤY LẠI HÓA ĐƠN
                    //sLAYLAIHOADON = clsMain.ReturnDataTable_NoLock("Select GIATRI From CAUHINH where TEN='MK_LAYLAIHD'").Rows[0][0].ToString();
                    sLAYLAIHOADON = dtCauhinh.Select("TEN='MK_LAYLAIHD'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    //29/08/2018 them lay thong tin co dung MẬT KHẨU HỦY BÀN
                    //sHUYBAN = clsMain.ReturnDataTable_NoLock("Select GIATRI From CAUHINH where TEN='MK_HUYBAN'").Rows[0][0].ToString();
                    sHUYBAN = dtCauhinh.Select("TEN='MK_HUYBAN'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    //29/08/2018 them lay thong tin co dung MẬT KHẨU HỦY MÓN
                    //sHUYMON = clsMain.ReturnDataTable_NoLock("Select GIATRI From CAUHINH where TEN='MK_HUYMON'").Rows[0][0].ToString();
                    sHUYMON = dtCauhinh.Select("TEN='INBEP_CHUYENBAN'")[0]["MK_HUYMON"].ToString();
                }
                catch
                {
                }
                try
                {
                    //29/08/2018 them lay thong tin co dung MẬT KHẨU MỞ KÉT
                    // sMOKET = clsMain.ReturnDataTable_NoLock("Select GIATRI From CAUHINH where TEN='MK_MOKET'").Rows[0][0].ToString();
                    sMOKET = dtCauhinh.Select("TEN='MK_MOKET'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    //29/08/2018 them lay thong tin co dung MẬT KHẨU SỬA GIÁ
                    //  sSUAGIA = clsMain.ReturnDataTable_NoLock("Select GIATRI From CAUHINH where TEN='MK_SUAGIA'").Rows[0][0].ToString();
                    sSUAGIA = dtCauhinh.Select("TEN='MK_SUAGIA'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    //29/08/2018 them lay thong tin co dung MẬT KHẨU CHỌN GIÁ
                    // sCHONGIA = clsMain.ReturnDataTable_NoLock("Select GIATRI From CAUHINH where TEN='MK_CHONGIA'").Rows[0][0].ToString();
                    sCHONGIA = dtCauhinh.Select("TEN='MK_CHONGIA'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    //29/08/2018 them lay thong tin co dung MẬT KHẨU BỎ KHÓA BÀN
                    //sBOKHOABAN = clsMain.ReturnDataTable_NoLock("Select GIATRI From CAUHINH where TEN='MK_BOKHOABAN'").Rows[0][0].ToString();
                    sBOKHOABAN = dtCauhinh.Select("TEN='MK_BOKHOABAN'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    //29/08/2018 them lay thong tin co dung MẬT KHẨU CHIẾT KHẤU
                    // sCHIETKHAU = clsMain.ReturnDataTable_NoLock("Select GIATRI From CAUHINH where TEN='MK_CHIETKHAU'").Rows[0][0].ToString();
                    sCHIETKHAU = dtCauhinh.Select("TEN='MK_CHIETKHAU'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    //29/08/2018 them lay thong tin co dung MẬT KHẨU KẾT CA
                    //   sKETCA = clsMain.ReturnDataTable_NoLock("Select GIATRI From CAUHINH where TEN='MK_KETCA'").Rows[0][0].ToString();
                    sKETCA = dtCauhinh.Select("TEN='MK_KETCA'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    //29/08/2018 them lay thong tin co dung MẬT KHẨU BÁN GIÁ 2
                    // sBANGIA2 = clsMain.ReturnDataTable_NoLock("Select GIATRI From CAUHINH where TEN='MK_BANGIA2'").Rows[0][0].ToString();
                    sBANGIA2 = dtCauhinh.Select("TEN='MK_BANGIA2'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    //12/09/2018 them lay thong tin Store_ID cho API huy viet nam
                    //  sStore_ID = clsMain.ReturnDataTable_NoLock("Select GIATRI From CAUHINH where TEN='Store_ID'").Rows[0][0].ToString();
                    sStore_ID = dtCauhinh.Select("TEN='Store_ID'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    //22/09/2018 them lay thong tin so luong khach
                    //  sSOLUONGKHACH = clsMain.ReturnDataTable_NoLock("Select GIATRI From CAUHINH where TEN='SOLUONGKHACH'").Rows[0][0].ToString();
                    sSOLUONGKHACH = dtCauhinh.Select("TEN='SOLUONGKHACH'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    //27/09/2018 them lay thong tin huy mon nhanh
                    //   sHUYMONNHANH = clsMain.ReturnDataTable_NoLock("Select GIATRI From CAUHINH where TEN='HUYMONNHANH'").Rows[0][0].ToString();
                    sHUYMONNHANH = dtCauhinh.Select("TEN='HUYMONNHANH'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    //03/10/2018 khóa nhập tay thẻ KHTT, TTNB
                    //  sKHOANHAPTHE = clsMain.ReturnDataTable_NoLock("Select GIATRI From CAUHINH where TEN='KHOANHAPTHE'").Rows[0][0].ToString();
                    sKHOANHAPTHE = dtCauhinh.Select("TEN='KHOANHAPTHE'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    //15/10/2018 menu id của cua hang huy vit nam
                    //   sMenuID = clsMain.ReturnDataTable_NoLock("Select GIATRI From CAUHINH where TEN='MenuID'").Rows[0][0].ToString();
                    sMenuID = dtCauhinh.Select("TEN='MenuID'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    //08/04/2019 VN PAY
                    VNPAY_IP = dtCauhinh.Select("TEN='VNPAY_IP'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    //08/04/2019 VN PAY
                    VNPAY_secretKey = dtCauhinh.Select("TEN='VNPAY_secretKey'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    //08/04/2019 VN PAY
                    VNPAY_appId = dtCauhinh.Select("TEN='VNPAY_appId'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    //08/04/2019 VN PAY
                    VNPAY_merchantName = dtCauhinh.Select("TEN='VNPAY_merchantName'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    //08/04/2019 VN PAY
                    VNPAY_merchantCode = dtCauhinh.Select("TEN='VNPAY_merchantCode'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    //08/04/2019 VN PAY
                    VNPAY_terminalId = dtCauhinh.Select("TEN='VNPAY_terminalId'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    //07/06/2019: 1:Cộng điểm trước khi chiết khấu. 0:Cộng điểm sau khi chiết khấu
                    sCONGDIEM_KHTT = dtCauhinh.Select("TEN='CONGDIEM_KHTT'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    //17/01/2019 them lay thong tin co chay mode STARLIGHT  khong quy dinh diem phu thuoc loai cap do
                    //sSTARLIGHT = clsMain.ReturnDataTable_NoLock("Select GIATRI From CAUHINH where TEN='STARLIGHT'").Rows[0][0].ToString();
                    sSTARLIGHT = dtCauhinh.Select("TEN='STARLIGHT'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    //06/09/2018 them lay thong tin co chay mode helio khong
                    //sHelio = clsMain.ReturnDataTable_NoLock("Select GIATRI From CAUHINH where TEN='HELIO'").Rows[0][0].ToString();
                    sHelio = dtCauhinh.Select("TEN='HELIO'")[0]["GIATRI"].ToString();
                    sHelio = "0";
                }
                catch
                {
                }
                try
                {
                    //08/05/2020 them cho phep in dam chu hay ko               
                    sIndam = dtCauhinh.Select("TEN='INDAM'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    //09/06/2020 set vao ban hang hay quan ly           
                    sBANHANG = dtCauhinh.Select("TEN='DATABANHANG'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    //21/07/2020 them cho phep in bill huy ban hay ko       
                    sInhuyban = dtCauhinh.Select("TEN='INBILLHUYBAN'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    //25/07/2020 GIABANTRUOCTHUE
                    sGIABANTRUOCTHUE = dtCauhinh.Select("TEN='GIABANTRUOCTHUE'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    //25/07/2020 GIABANTRUOCTHUE
                    sAPDUNGBANGGIATHEOKHUVUC = dtCauhinh.Select("TEN='APDUNGBANGGIATHEOKHUVUC'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    //31/07/2020 GIABANTRUOCTHUE
                    sIN_GIAYLON = dtCauhinh.Select("TEN='IN_GIAYLON'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    //17/08/2020 sThoigianbao_Billdoi
                    sThoigianbao_Billdoi = dtCauhinh.Select("TEN='GIAYBILLDOI'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    //01/05/2020: 1-Imgur,2-ImageShack
                    sApi_upload = dtCauhinh.Select("TEN='Api_upload'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    //01/05/2020: key dùng upload lên ImageShack
                    sKEY_ImageShack = dtCauhinh.Select("TEN='KEY_ImageShack'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    //01/05/2020: key dùng upload lên Imgur
                    sKEY_Imgur = dtCauhinh.Select("TEN='KEY_Imgur'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    //20/08/2020 App_id
                    sApp_id = dtCauhinh.Select("TEN='App_id'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    //20/08/2020 Key1
                    sKey1 = dtCauhinh.Select("TEN='Key1'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    //20/08/2020 CreateOrderUrl
                    sCreateOrderUrl = dtCauhinh.Select("TEN='CreateOrderUrl'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    //20/08/2020 QueryOrderUrl
                    sQueryOrderUrl = dtCauhinh.Select("TEN='QueryOrderUrl'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    //20/08/2020 RefundUrl
                    sRefundUrl = dtCauhinh.Select("TEN='RefundUrl'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    //20/08/2020 Access_token
                    sAccess_token = dtCauhinh.Select("TEN='Access_token'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    //09/09/2020 partnerCode dùng cho MoMo
                    partnerCode = dtCauhinh.Select("TEN='partnerCode'")[0]["GIATRI"].ToString();
                }
                catch
                {

                }
                try
                {
                    //09/09/2020 publicKey dùng cho MoMo
                    publicKey = dtCauhinh.Select("TEN='publicKey'")[0]["GIATRI"].ToString();
                }
                catch
                {

                }
                try
                {
                    //09/09/2020 secretKey dùng cho MoMo
                    secretKey = dtCauhinh.Select("TEN='secretKey'")[0]["GIATRI"].ToString();
                }
                catch
                {

                }
                try
                {
                    //09/09/2020 url_momo dùng cho MoMo
                    url_momo = dtCauhinh.Select("TEN='url_momo'")[0]["GIATRI"].ToString();
                }
                catch
                {

                }
                try
                {
                    //09/09/2020 storeSlug dùng cho MoMo
                    storeSlug = dtCauhinh.Select("TEN='storeSlug'")[0]["GIATRI"].ToString();
                }
                catch
                {

                }
                try
                {
                    //24/09/2020 auto click món thêm
                    sAUTOMONTHEM = dtCauhinh.Select("TEN='AUTOMONTHEM'")[0]["GIATRI"].ToString();
                }
                catch
                {

                }
                try
                {
                    //13/10/2020 mode bán hàng
                    sMode = dtCauhinh.Select("TEN='MODE'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    //20/11/2020 show form nhập tiền tạm ứng
                    sShowTamUng = dtCauhinh.Select("TEN='SHOWTAMUNG'")[0]["GIATRI"].ToString();
                }
                catch
                {

                }
                try
                {
                    //21/01/2021 show form nhập ghi chú - chiết khấu
                    sShow_Ghichu_ChietKhau = dtCauhinh.Select("TEN='SHOWGHICHUCHIETKHAU'")[0]["GIATRI"].ToString();
                }
                catch
                {

                }
                try
                {
                    //21/01/2021 mode cấu hình bida hay karaoke
                    sMode_Bida_Kara = dtCauhinh.Select("TEN='MODEBIDAKARA'")[0]["GIATRI"].ToString();
                }
                catch
                {

                }
                try
                {
                    //04/12/2021 mode cấu hình bida hay karaoke
                    sMode_Phanquyennhanvien = dtCauhinh.Select("TEN='PHANQUYENTHEONHANVIEN'")[0]["GIATRI"].ToString();
                }
                catch
                {

                }
                try
                {
                    //18/05/2022 VN PAY
                    VNPAY_merchantType = dtCauhinh.Select("TEN='VNPAY_merchantType'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                //MOMO 18/05/2022
                try
                {
                    //18/05/2022 accessKey dùng cho MoMo
                    accessKey = dtCauhinh.Select("TEN='accessKey'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    //31/05/2022 orderGroupId MOMO phân biệt cửa hàng tạo hóa đơn
                    //sOrderGroupId = cls_Main.ReturnDataTable_NoLock("Select GIATRI From CAUHINH where TEN='orderGroupId'", sConnectionString_live).Rows[0][0].ToString();
                    sOrderGroupId = dtCauhinh.Select("TEN='orderGroupId'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    //18/05/2022 storeId dùng cho MoMo
                    storeId = dtCauhinh.Select("TEN='storeId'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    //18/05/2022 URLTaoQRCODE dùng cho MoMo
                    sURLTaoQRCODE = dtCauhinh.Select("TEN='URLTaoQRCODE'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                ///////HOA DON
                // sBkavPartnerGUID -TO EDIT- (2) Do Bkav cấp cho từng đối tác
                try
                {
                    sBkavPartnerGUID = dtCauhinh.Select("TEN='BkavPartnerGUID'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                // sBkavPartnerToken -TO EDIT- (2) Do Bkav cấp cho từng đối tác
                try
                {
                    sBkavPartnerToken = dtCauhinh.Select("TEN='BkavPartnerToken'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                //sHoaDon_DienTu: Có dùng hóa đơn điện tử không
                try
                {
                    sHoaDon_DienTu = dtCauhinh.Select("TEN='HOADON_DIENTU'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                //sWebServiceURL: URL kết nối đến WebService eHoaDon
                try
                {
                    sWebServiceURL = dtCauhinh.Select("TEN='WEB_SERVICE_URL'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                //sCmdType: Loại khởi tạo hóa đơn
                try
                {
                    sCmdType = dtCauhinh.Select("TEN='COMMAND_TYPE'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                //sVATType: Loại thuế VAT
                try
                {
                    sVATType = dtCauhinh.Select("TEN='VAT_TYPE'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                //sThuMuc_HoaDon: Nơi lưu các file pdf hóa đơn
                try
                {
                    //dt_cauhinh = clsMain.ReturnDataTable_NoLock("SELECT GIATRI FROM CAUHINH WHERE TEN='THUMUC_HOADON'");
                    //if (dt_cauhinh.Rows.Count > 0)
                    //{
                    //    sThuMuc_HoaDon = dt_cauhinh.Rows[0]["GIATRI"].ToString();
                    //}
                    sThuMuc_HoaDon = "\\HoaDon\\";
                    //kiểm tra nếu thư mục "HoaDon" chưa tồn tại thì tạo mới
                    string directoryPath = "\\HoaDon";
                    if (!System.IO.Directory.Exists(directoryPath))
                    {
                        System.IO.Directory.CreateDirectory(directoryPath);
                        DirectoryInfo theFolder = new DirectoryInfo("\\HoaDon");
                    }
                }
                catch
                {
                }
                //sLOAI_HDGTGT: Cty cung cấp HDGTGT:1-BKAV,2-VNPT
                try
                {
                    sLOAI_HDDT = dtCauhinh.Select("TEN='LOAI_HDDT'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                //sUsername_service: tài khoản để gọi service
                try
                {
                    sUsername_service = dtCauhinh.Select("TEN='Username_service'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                //sPass_service: tài khoản để gọi service
                try
                {
                    sPass_service = dtCauhinh.Select("TEN='Pass_service'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                //sUsername_nhanvien: tài khoản để gọi service
                try
                {
                    sUsername_nhanvien = dtCauhinh.Select("TEN='Username_nhanvien'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                //sPass_nhanvien: tài khoản để gọi service
                try
                {
                    sPass_nhanvien = dtCauhinh.Select("TEN='Pass_nhanvien'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                //sPattern: Mẫu số hóa đơn
                try
                {
                    sPattern = dtCauhinh.Select("TEN='Pattern'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                //sAPI_Khachhang : VNPT mỗi khách hàng 1 link nên thêm cấu hình để chạy cho khách nào thì dùng service của khách đó
                try
                {
                    sAPI_Khachhang = dtCauhinh.Select("TEN='API_KHACHHANG'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                //Tên hàng hóa trong HDDT
                try
                {
                    sTenHangHoa_HDDT = dtCauhinh.Select("TEN='TENHANGHOA_HDDT'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                //Cấu hình lấy tên hành khách từ vé
                try
                {
                    sLayTenHanhKhach_HDDT = dtCauhinh.Select("TEN='LAYTENHANHKHACH_HDDT'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                //Cấu hình giá bán trước thuế
                try
                {
                    sGIABANTRUOCTHUE = dtCauhinh.Select("TEN='GIABANTRUOCTHUE'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                //Cấu hình tách vé tàu theo chuyến
                try
                {
                    sTACHVE_HDDT = dtCauhinh.Select("TEN='TACHVE_HDDT'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                // sBkavPartnerGUID2 -TO EDIT- (2) Do Bkav cấp cho từng đối tác
                try
                {
                    sBkavPartnerGUID2 = dtCauhinh.Select("TEN='BkavPartnerGUID2'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                // sBkavPartnerToken2 -TO EDIT- (2) Do Bkav cấp cho từng đối tác
                try
                {
                    sBkavPartnerToken2 = dtCauhinh.Select("TEN='BkavPartnerToken2'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                // sXuatHDTheoDiemDi: Tùy theo từng điểm đi chọn partner phù hợp
                try
                {
                    sXuatHDTheoDiemDi = dtCauhinh.Select("TEN='XuatHDTheoDiemDi'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    sTen_KhachKhongLayHoaDon = dtCauhinh.Select("TEN='Ten_KhachKhongLayHoaDon'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    sEmail_KhachKhongLayHoaDon = dtCauhinh.Select("TEN='Email_KhachKhongLayHoaDon'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                //MISA
                try
                {
                    sLAY_TOKEN = dtCauhinh.Select("TEN='LAY_TOKEN'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    sLAY_THONGTIN_PHATHANH = dtCauhinh.Select("TEN='LAY_THONGTIN_PHATHANH'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    sTAO_SUA_HD = dtCauhinh.Select("TEN='TAO_SUA_HD'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    sDOWNLOAD_HD_CHUAPHATHANH = dtCauhinh.Select("TEN='DOWNLOAD_HD_CHUAPHATHANH'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    sDIACHI_SERVER_KYHD = dtCauhinh.Select("TEN='DIACHI_SERVER_KYHD'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    sPHATHANH_HD = dtCauhinh.Select("TEN='PHATHANH_HD'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    sLAY_THONGTIN_HD = dtCauhinh.Select("TEN='LAY_THONGTIN_HD'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    sDOWNLOAD_HD_DAPHATHANH = dtCauhinh.Select("TEN='DOWNLOAD_HD_DAPHATHANH'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    sLAY_TRANGTHAI_HD = dtCauhinh.Select("TEN='LAY_TRANGTHAI_HD'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    sGUIEMAIL_HD_DAPHATHANH = dtCauhinh.Select("TEN='GUIEMAIL_HD_DAPHATHANH'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    sCHUYENDOI_HD_DAPHATHANH = dtCauhinh.Select("TEN='CHUYENDOI_HD_DAPHATHANH'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    sHUY_HD_DAPHATHANH = dtCauhinh.Select("TEN='HUY_HD_DAPHATHANH'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    sPINCODE = dtCauhinh.Select("TEN='PINCODE'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    DataTable dt_Cty = cls_Main.ReturnDataTable_NoLock("SELECT MASOTHUE FROM SYS_CONFIGREPORT", sConnectionString_live);
                    if (dt_Cty.Rows.Count > 0)
                    {
                        sMST = dt_Cty.Rows[0][0].ToString();
                    }
                }
                catch
                {
                }
                try
                {
                    DataTable dt_Cty = cls_Main.ReturnDataTable_NoLock("SELECT TENCONGTY FROM SYS_CONFIGREPORT", sConnectionString_live);
                    if (dt_Cty.Rows.Count > 0)
                    {
                        sCONGTY = dt_Cty.Rows[0][0].ToString();
                    }
                }
                catch
                {
                }
                try
                {
                    DataTable dt_Cty = cls_Main.ReturnDataTable_NoLock("SELECT DIACHI FROM SYS_CONFIGREPORT", sConnectionString_live);
                    if (dt_Cty.Rows.Count > 0)
                    {
                        sDIACHI = dt_Cty.Rows[0][0].ToString();
                    }
                }
                catch
                {
                }
                try
                {
                    if (sLOAI_HDDT == "7")
                    {
                        string msg = "";
                        //XuLyHDDT_MISA.getLAY_TOKEN(out msg);
                    }
                }
                catch
                {
                }
                if ((cls_ConfigCashier.sHoaDon_DienTu == "1" & (cls_ConfigCashier.sCmdType == "100" || cls_ConfigCashier.sCmdType == "101") & cls_ConfigCashier.sLOAI_HDDT == "1") || cls_ConfigCashier.sHoaDon_DienTu == "1" & cls_ConfigCashier.sLOAI_HDDT == "2" || cls_ConfigCashier.sHoaDon_DienTu == "1" & cls_ConfigCashier.sLOAI_HDDT == "7" || cls_ConfigCashier.sHoaDon_DienTu == "1" & cls_ConfigCashier.sLOAI_HDDT == "10")
                {
                    HDDT_eHOADON = true;
                }
                try
                {
                    sTAO_VA_KY_HD = dtCauhinh.Select("TEN='TAO_VA_KY_HD'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    sKY_HIEU_HD_VE = dtCauhinh.Select("TEN='KY_HIEU_HD_VE'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    sKY_HIEU_HD_COMBO = dtCauhinh.Select("TEN='KY_HIEU_HD_COMBO'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    sLAY_HD_PDF = dtCauhinh.Select("TEN='LAY_HD_PDF'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    sLOAI_KYPHATHANH = dtCauhinh.Select("TEN='LOAI_KYPHATHANH'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    sXOA_HUY_HD = dtCauhinh.Select("TEN='XOA_HUY_HD'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    sUPDATE_HD = dtCauhinh.Select("TEN='UPDATE_HD'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    sDIEUCHINH_HD = dtCauhinh.Select("TEN='DIEUCHINH_HD'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    sTHAY_THE_HD = dtCauhinh.Select("TEN='THAY_THE_HD'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    sGetQRCode_VNPAY = dtCauhinh.Select("TEN='GetQRCode_VNPAY'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    sCheckTrans_VNPAY = dtCauhinh.Select("TEN='CheckTrans_VNPAY'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    sSecretKey_CheckTrans = dtCauhinh.Select("TEN='SecretKey_CheckTrans'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    //16/08/2022 Load report theo ten khach hang(report bill, report qrcode congxoay)
                    sTENKHACHHANG = dtCauhinh.Select("TEN='TENKHACHHANG'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    sDUNG_RIENG_MAT_KHAU = dtCauhinh.Select("TEN='DUNG_RIENG_MAT_KHAU'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    sTHEMHANGHOA = dtCauhinh.Select("TEN='MK_THEMHANGHOA'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    sTRAHANG = dtCauhinh.Select("TEN='MK_TRAHANG'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    sTAMUNG = dtCauhinh.Select("TEN='MK_TAMUNG'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    sTRAHANGQRCODE = dtCauhinh.Select("TEN='MK_TRAHANGQRCODE'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    sCauhinhsudung = dtCauhinh.Select("TEN='CAUHINHSUDUNG'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    sIN_QRCODE = dtCauhinh.Select("TEN='IN_QRCODE'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
                try
                {
                    sTTNB_TIENTOITHIEU = dtCauhinh.Select("TEN='TTNB_TIENTOITHIEU'")[0]["GIATRI"].ToString();
                }
                catch
                {
                }
            }
            catch
            {
            }
        }
    } 
}