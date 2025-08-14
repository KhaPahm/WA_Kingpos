namespace WA_Kingpos.Models
{
    public class ThongTinBangLuong
    {
        public int? MANHANVIEN { get; set; }
        public string? DIACHI { get; set; }
        public string? TENNHANVIEN { get; set; }
        public int? MATRANGTHAI_NHANVIEN { get; set; }
        public string? TRANGTHAI_NHANVIEN { get; set; }
        public double? DOANHSO_KPI { get; set; }
        public string? KHUVUC_KPI { get; set; }
        public double? DOANHSO_TIEUCHUAN { get; set; }
        public double? TYLE_HOANTHANH { get; set; }
        public double? DONVI_HH { get; set; }
        public double? HH_DOANHSO { get; set; }
        public double? TONGLUONG { get; set; }
        public double? LUONG_CANBAN { get; set; }
        public double? LUONG_QUANLY { get; set; }
        public double? CHENHLECH_BANHANG { get; set; }
        public DateTime? NGAYNHANVIEC { get; set; }
        public int? SOTHANG_LAMVIEC { get; set; }
        public string? MANHANVIEN_CAPDUOI { get; set; }
        public string? TENNHANVIEN_CAPDUOI { get; set; }
        public double? DONVI_HH_QUANLY { get; set; }
        public double? HH_QUANLY { get; set; }

        public bool IS_QUANLY { get => !string.IsNullOrEmpty(MANHANVIEN_CAPDUOI); }

        public string VAITRO_GROUP { get => IS_QUANLY ? "Quản lý" : "Nhân viên"; }
        public string? NHOMNHANVIEN { get; set; }

    }
}
