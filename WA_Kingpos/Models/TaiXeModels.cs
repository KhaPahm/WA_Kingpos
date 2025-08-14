using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
namespace WA_Kingpos.Models
{
    public class TaiXeModels
    {
        public string? MA { get; set; }

        [Required(ErrorMessage = "ID không được để trống")]
        public string? ID { get; set; }

        [Required(ErrorMessage = "Tên không được để trống")]
        public string? TEN { get; set; }

        [Required(ErrorMessage = "CMND không được để trống")]
        public string? CMND { get; set; }

        [Required(ErrorMessage = "Điện thoại không được để trống")]
        public string? DIENTHOAI { get; set; }
        public string? WEBSITE { get; set; }
        public string? MAKHGIOITHIEU { get; set; }
        public string? SUDUNG { get; set; }
        public DateTime? LastUpdate { get; set; }
    }
}
