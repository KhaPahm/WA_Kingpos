using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
namespace WA_Kingpos.Models
{
    public class PhieuDatHangModels
    {
        [Required(ErrorMessage = "Kho không được để trống")]
        public string? MA_KHO { get; set; }
        public string? TEN_KHO { get; set; }

        [Required(ErrorMessage = "Ngày lập phiếu không được để trống")]
        public string? NGAYLAPPHIEU { get; set; }

        [Required(ErrorMessage = "Mã phiếu không được để trống")]
        public string? MAPDNDC { get; set; }
        public string? SOTIEN { get; set; }
        public string? MANHANVIEN { get; set; }
        public string? TENNHANVIEN { get; set; }
        public string? TRANGTHAI1 { get; set; }
        public int? TRANGTHAI { get; set; }
        public string? NGUOIDUYET { get; set; }
        public string? NGAYDUYET { get; set; }
        public string? MA_HOADON { get; set; }
        public string? NGUOISUA { get; set; }
        public string? NGAYSUA { get; set; }
        public string? NGAYBOCHANG { get; set; }
        public string? NGUOIGIAOHANG { get; set; }
        public string? WEBSITE { get; set; }

        [Required(ErrorMessage = "Khách hàng không được để trống")]
        public string? MADOITUONG { get; set; }
        public string? TENKHACHHANG { get; set; }
        public string? DIACHI { get; set; }
        public string? CMND { get; set; }
        public string? EMAIL { get; set; }
        public string? DIENTHOAI { get; set; }
        public string? MST { get; set; }
        public string? BIENSOXE { get; set; }
        public string? TEN_TAIXE { get; set; }
        public string? DIENTHOAI_TAIXE { get; set; }
        public string? CMND_TAIXE { get; set; }
        public DateTime? TU_NGAY { get; set; }
        public string ? MAPHIENLAMVIEC { get; set; }
        public DateTime? DEN_NGAY { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string? NGAYLAPPHIEU_OLD { get; set; }
        public List<HangHoaModels>?listHANGHOA { get; set; }
        public string? MA_HANGHOA { get; set; }
        //HANG HOA
        //public string? MA_VACH { get; set; }
        //public string? TEN { get; set; }
        //public string? DVT { get; set; }
        //public string? GIANHAP { get; set; }
        //public string? SL { get; set; }
        //public string? THANHTIEN { get; set; }
        //public string? DUYET { get; set; }
    }
}
