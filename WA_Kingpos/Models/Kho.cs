using System.ComponentModel.DataAnnotations;

namespace WA_Kingpos.Models
{
    public class clsKho
    {
        public string? ma_kho { get; set; }

        [Required(ErrorMessage = "Tên không được để trống")]
        public string? ten_kho { get; set; }

        [Required(ErrorMessage = "Cửa hàng không được để trống")]
        public string? ma_cuahang { get; set; }
        public string? ten_cuahang { get; set; }
        public string? ghichu { get; set; }
        public string? sudung { get; set; }
    }
}
