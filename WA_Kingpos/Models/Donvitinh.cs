using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WA_Kingpos.Models
{
    public class clsDonvitinh
    {
        public string? ma_donvitinh { get; set; }
        [Required(ErrorMessage = "Tên không được để trống")]
        public string? ten_donvitinh { get; set; }
        public string? ghichu { get; set; }
        public string? sudung { get; set; }

        [Range(0, 100)]
        public double? chietkhau { get; set; }
    }
    
}
