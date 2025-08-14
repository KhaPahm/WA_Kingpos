namespace WA_Kingpos.Models
{
    public class clsKhachHangDangKyFace
    {
        public long ID { get; set; }
        public string? FACE_ID { get; set; }
        public long MA_KH { get; set; }
        public string? TEN_KH { get; set; }
        public string? NGAYSINH { get; set; }
        public bool? GIOITINH { get; set; }
        public string? DIACHI { get; set; }
        public string? DIENTHOAI { get; set; }
        public string? EMAIL { get; set; }
        public string? CMND { get; set; }
        public string? QUOCTICH { get; set; }
        public string? GATE_ID { get; set; }
        public string? GATE_NO { get; set; }
        public string? GATE_NAME { get; set; }
        public string? MADATPHONG { get; set; }
        public string? NGUOITAO { get; set; }
        public DateTime NGAYTAO { get; set; }
        public string? NGUOISUA { get; set; }
        public DateTime? NGAYSUA { get; set; }
        public bool TRANGTHAI { get; set; }
        public DateTime NGAYBATDAU { get; set; }
        public bool IS_ADD { get; set; }
        public DateTime? TIME_ADD { get; set; }
        public string? LOG_ADD { get; set; }
        public DateTime NGAYKETTHUC { get; set; }
        public bool IS_DEL { get; set; }
        public DateTime? TIME_DEL { get; set; }
        public string? LOG_DEL { get; set; }
        public string? FACE_PHOTO { get; set; }
        public bool ONLY_1 { get; set; }
        public DateTime? TIME_REGISTER_FACE { get; set; }
        public bool IS_ADD_FACE { get; set; }
        public DateTime? TIME_ADD_FACE { get; set; }
        public string? LOG_ADD_FACE { get; set; }

        public string? MAPHONG { get; set; }

        public string? TENPHONG { get; set; }

        public string strGioiTinh { get { return GIOITINH == true ? "Nam" : GIOITINH == false ? "Nữ" : ""; } }

    }
}
