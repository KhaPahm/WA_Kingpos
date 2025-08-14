using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WA_Kingpos.Models
{
    public class clsBep
    {
        public string? ma_bep { get; set; }

        [Required(ErrorMessage = "Tên không được để trống")]
        public string? ten_bep { get; set; }
        public string? mayinbep { get; set; }
        public string? ip { get; set; }
        public string? ghichu { get; set; }
        public string? sudung { get; set; }

        [Required(ErrorMessage = "Cửa hàng không được để trống")]
        public string? ma_cuahang { get; set; }
        public string? ten_cuahang { get; set; }


    }
}
