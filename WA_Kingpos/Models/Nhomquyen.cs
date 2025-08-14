using System.ComponentModel.DataAnnotations;

namespace WA_Kingpos.Models
{
    public class clsNhomquyen
    {
        public string? ma_nhomquyen { get; set; }

        [Required(ErrorMessage = "Tên không được để trống")]
        public string? ten_nhomquyen { get; set; }
        public string? ghichu { get; set; }
        public string? sudung { get; set; }
    }
}
