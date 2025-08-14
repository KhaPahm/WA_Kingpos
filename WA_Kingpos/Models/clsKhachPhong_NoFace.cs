namespace WA_Kingpos.Models
{
    /// <summary>
    /// Thông tin khách đã đăng ký vào phòng, nhưng chưa đăng ký khuôn mặt
    /// </summary>
    public class clsKhachPhong_NoFace
    {
        public string? MA_HOADON { get; set; }
        public int? MAPHONG { get; set; }
        public int? MA_KH { get; set; }
        public string? TEN { get; set; }
        public DateTime? NGAYSINH { get; set; }
        public bool? GIOITINH { get; set; }
        public string? DIACHI { get; set; }
        public string? DIENTHOAI { get; set; }
        public string? MADATPHONG { get; set; }
        public DateTime? NGAYNHANPHONG { get; set; }
        public DateTime? NGAYTRAPHONG { get; set; }
        public string? TENPHONG { get; set; }
        public int? DAILY { get; set; }
        public string? TEN_DAILY { get; set; }

        public string strGioiTinh { get { return GIOITINH == true ? "Nam" : GIOITINH == false ? "Nữ" : ""; } }

        /// <summary>
        /// Đánh dấu khuôn mặt chỉ dùng 1 lần
        /// </summary>
        public bool ONLY_1 { get; set; }

        public bool SUDUNG { get; set; } = true;
    }
}
