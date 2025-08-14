using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
namespace WA_Kingpos.Models
{
    public class PhieuDatXeModels
    {
        [Required(ErrorMessage = "Kho không được để trống")]
        public string? MA_KHO { get; set; }
        public string? TEN_KHO { get; set; }

        [Required(ErrorMessage = "Ngày bốc hàng không được để trống")]
        public string? NGAYGIAOHANG { get; set; }

        [Required(ErrorMessage = "Ngày lập phiếu không được để trống")]
        public string? NGAYLAPPHIEU { get; set; }

        [Required(ErrorMessage = "Mã phiếu không được để trống")]
        public string? MAPDNDC { get; set; }
        public int? TRANGTHAI { get; set; }
        public string? TRANGTHAI1 { get; set; }
        public string? SOTIEN { get; set; }
        public string? MANHANVIEN { get; set; }
        public string? TENNHANVIEN { get; set; }

        [Required(ErrorMessage = "Biển số không được để trống")]
        public string? BIENSOXE { get; set; }
        public string? NGUOIGIAOHANG { get; set; }
        public string? TEN_TAIXE { get; set; }
        public string? DIENTHOAI_TAIXE { get; set; }
        public string? CMND_TAIXE { get; set; }
        public string? CHON { get; set; }
        public string? MAPHIENLAMVIEC { get; set; }

        [Required(ErrorMessage = "Tài xế không được để trống")]
        public string? MADOITUONG { get; set; }
        public string? DIACHI { get; set; }
        public string? DIENTHOAI { get; set; }
    }
}
