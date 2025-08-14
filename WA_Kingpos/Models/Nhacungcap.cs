using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WA_Kingpos.Models
{
    public class clsNhacungcap
    {
        public string? MA { get; set; }

        [Required(ErrorMessage = "Mã đại lý không được để trống")]
        public string? ID { get; set; }

        [Required(ErrorMessage = "Tên không được để trống")]
        public string? TEN { get; set; }

        [Required(ErrorMessage = "Loại không được để trống")]
        public string? LOAI { get; set; }
        public string? TENLOAI { get; set; }
        public string? HANMUC_CONGNO { get; set; }
        public string? HANTHANHTOAN { get; set; }
        public string? TIENDATCOC { get; set; }
        public string? ISDATCOC { get; set; }
        public string? DIACHI { get; set; }
        public string? DIENTHOAI { get; set; }
        public string? FAX { get; set; }
        public string? EMAIL { get; set; }
        public string? WEBSITE { get; set; }
        public string? NGUOILIENHE { get; set; }
        public string? GHICHU { get; set; }
        public string? SUDUNG { get; set; }
        public string? CMND { get; set; }

        [Required(ErrorMessage = "Ngày sinh không được để trống")]
        public string? NGAYSINH { get; set; }
        public string? GIOITINH { get; set; }

        [Required(ErrorMessage = "Loại đại lý không được để trống")]
        public string? LOAIDAILY { get; set; }

        [Required(ErrorMessage = "Cấp đại lý không được để trống")]
        public string? CAPDO { get; set; }
        public string? LOAIDAILY1 { get; set; }
        public string? CAPDAILY { get; set; }
        public string? CHIETKHAU { get; set; }
    }
}
