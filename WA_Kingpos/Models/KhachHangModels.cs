using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
namespace WA_Kingpos.Models
{
    public class KhachHangModels
    {
        public string? MA { get; set; }

        [Required(ErrorMessage = "ID không được để trống")]
        [MinLength(1)]
        public string? ID { get; set; }

        [Required(ErrorMessage = "Tên không được để trống")]
        public string? TEN { get; set; }
        public string? CMND { get; set; }

        [Display(Name ="Đây là ngày sinh")]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? NGAYSINH { get; set; }
        public string? GIOITINH { get; set; }

        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        public string? DIACHI { get; set; }

        [Required(ErrorMessage = "Điện thoại không được để trống")]
        public string? DIENTHOAI { get; set; }
        public string? EMAIL { get; set; }
        public string? FAX { get; set; }
        public string? MAKHGIOITHIEU { get; set; }
        public string? TENKHGIOITHIEU { get; set; }
        public string? SUDUNG { get; set; }
        public DateTime? LastUpdate { get; set; }
    }
}
