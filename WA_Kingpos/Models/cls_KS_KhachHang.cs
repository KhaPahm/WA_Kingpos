using System.Diagnostics.CodeAnalysis;

namespace WA_Kingpos.Models
{
    public class cls_KS_KhachHang
    {
        [AllowNull]
        public int MA_KH { get; set; } = 0;
        [AllowNull]
        public string TEN { get; set; }
        [AllowNull]
        public string NGAYSINH { get; set; }
        [AllowNull]
        public string GIOITINH { get; set; }
        [AllowNull]
        public string DIACHI { get; set; }
        [AllowNull]
        public string DIENTHOAI { get; set; }
        [AllowNull]
        public string CCCD { get; set; }
        [AllowNull]
        public string EMAIL { get; set; }
        [AllowNull]
        public string QUOCTICH { get; set; }
        [AllowNull]
        public DateTime NGAYTAO { get; set; }
        [AllowNull]
        public string TENNHANVIEN { get; set; }
        [AllowNull]
        public string FACE_PHOTO { get; set; }
    }
}
