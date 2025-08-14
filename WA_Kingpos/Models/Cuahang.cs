using System.ComponentModel.DataAnnotations;

namespace WA_Kingpos.Models
{
    public class Cuahang
    {
        public string? ma_cuahang { get; set; }

        [Required(ErrorMessage = "Tên không được để trống")]
        public string? ten_cuahang { get; set; }
        public string? ghichu { get; set; }
        public string? sudung { get; set; }
        public string? header { get; set; }
        public string? footer { get; set; }
        public string? header_ve { get; set; }
        public string? footer_ve { get; set; }
        public string? image_ve { get; set; }
        public string? madiadiem { get; set; }
    }
}
