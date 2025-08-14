using System.ComponentModel.DataAnnotations;

namespace WA_Kingpos.Models
{
    public class clsQuay
    {
        public string? ma_quay { get; set; }

        [Required(ErrorMessage = "Tên không được để trống")]
        public string? ten_quay { get; set; }

        [Required(ErrorMessage = "Kho không được để trống")]
        public string? ma_kho { get; set; }
        public string? ten_cuahang { get; set; }
        public string? ten_kho { get; set; }
        public string? ghichu { get; set; }
        public string? sudung { get; set; }
    }
}
