using System.ComponentModel.DataAnnotations;

namespace WA_Kingpos.Models
{
    public class PhieuDuyetThu
    {
        [Required(ErrorMessage = "Mã phiếu không được để trống")]
        public string? SO_PHIEU { get; set; }

        [Required(ErrorMessage = "Lý do không được để trống")]
        public string? LY_DO { get; set; }

        [Required(ErrorMessage = "Ngày lập phiếu không được để trống")]
        public string? NGAYLAPPHIEU { get; set; }
        public string? SOTIEN { get; set; }
        public List<DONDATModels>? listDONDAT { get; set; }
        public string? MA_HOADON { get; set; }
        public string? TRANG_THAI1 { get; set; }
        public string? TRANG_THAI { get; set; }
        public string? NHAN_VIEN { get; set; }
    }

    public class DONDATModels
    {
        public string? CHON { get; set; }
        public string? STT { get; set; }
        public string? PHIEUDATHANG { get; set; }
        public string? KHACHHANG { get; set; }
        public string? NGAYLAP { get; set; }
        public string? THANHTIEN { get; set; }
        public string? NHANVIEN_DATHANG { get; set; }
        public string? DATRA { get; set; }
        public string? NHAPTHANHTOAN { get; set; }
        public string? CONLAI { get; set; }
    }
}
