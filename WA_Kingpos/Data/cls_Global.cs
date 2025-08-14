using System.Data;

namespace WA_Kingpos.Data
{
    public class cls_Global
    {
        #region "Biến toàn cục"
        ///<summary>
        ///Tên server
        ///</summary>
        public static string gsServer = "";
        ///<summary>
        ///Tên user để login vào server
        ///</summary>
        public static string gsUserName = "";
        ///<summary>
        ///Password của user để login vào server
        ///</summary>
        public static string gsPassword = "";
        ///<summary>
        ///Tên database
        ///</summary>
        public static string gsDBName = "";
        ///<summary>
        ///Chuổi kết nối
        ///</summary>
        public static string gsConnectionString = "";
        ///<summary>
        ///Tên user đăng nhập vào phần mềm          
        ///</summary>
        public static string gsUserID = "";
        ///<summary>
        ///Password của user đăng nhập vào phần mềm
        ///</summary>
        public static string gsUserPassword = "";
        ///<summary>
        ///Ngày trên server
        ///</summary>
        public static DateTime gdServerDate = DateTime.Now;
        ///<summary>
        ///Bảng lưu thông tin phân quyền
        ///</summary>
        public static DataTable dtPermission = null;
        ///<summary>
        ///Bảng lưu thông tin kích hoạt
        ///</summary>
        public static DataTable dtActive = null;
        ///<summary>
        ///Tên của userlogin (Tên gọi không phải tên login)
        ///</summary>
        public static string gsNameOfUserLogin = "";
        ///<summary>
        ///Mã nhân viên của userlogin
        ///</summary>
        public static string gsMaNVOfUserLogin = "";
        ///<summary>
        ///Bàn phím 
        ///</summary>
        public static bool bkeyboard = true;
        ///<summary>
        ///Kiểm tra đăng nhập
        ///</summary>
        public static bool FinishLogin = false;
        ///<summary>
        ///Mã ngôn ngữ
        ///</summary>
        public static string gsLanguage = "vi-VN";
        ///<summary>
        ///Tiền tạm ứng
        ///</summary>
        public static string gsTientamung = "0";
        ///<summary>
        ///Thời gian đăng nhập gần nhất
        ///</summary>
        public static DateTime gdDateTime_Log = DateTime.Now;
        #endregion
    }
}
