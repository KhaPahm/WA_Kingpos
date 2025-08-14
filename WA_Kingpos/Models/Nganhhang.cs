using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WA_Kingpos.Models
{
    public class clsNganhhang
    {
        public string? ID { get; set; }

        [Required(ErrorMessage = "Tên không được để trống")]
        public string? VNValue { get; set; }

        [Required(ErrorMessage = "Không được để trống")]
        public string? OrderBy { get; set; }
    }
}
