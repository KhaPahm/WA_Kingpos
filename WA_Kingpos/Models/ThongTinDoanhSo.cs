namespace WA_Kingpos.Models
{
    public class ThongTinDoanhSo
    {
        public string? MANHANVIEN { get; set; }
        public string? TENNHANVIEN { get; set; }
        public double? HESOKPI { get; set; }
        public double? DOANHSO_KPI { get; set; }
        public string? KHUVUC_KPI { get; set; }
        public double? DOANHSO_TIEUCHUAN { get; set; }
        public string? PHIEUDENGHI { get; set; }
        public int? HANGHOA { get; set; }
        public string? TEN_HANGHOA { get; set; }
        public double? SOLUONG { get; set; }
        public double? DONGIA { get; set; }
        public double? THANHTIEN { get; set; }

        public double? DONGIA_BANGKE { get; set; }

        public double? CHENHLECH_DONGIA { get; set; }

        public double? THANHTIEN_BANGKE { get; set; }

        public double? CHENHLECH_THANHTIEN { get; set; }

        public DateTime? NGAYTHANHTOAN { get; set; }
        public string? TEN_DONVITINH { get; set; }
        public int? MAKHO { get; set; }
        public string? TEN_KHO { get; set; }
        public int? MA_NHOMHANG { get; set; }
        public string? TEN_NHOMHANG { get; set; }
        public string? DIACHI { get; set; }
        public int? SOTHANG_LAMVIEC { get; set; }
        public DateTime? NGAYNHANVIEC { get; set; }

        public string? TENKHACHHANG { get; set; }
        //public string? strNGAYTHANHTOAN { get => NGAYTHANHTOAN?.ToString("dd/MM/yyyy"); }

    }
}
