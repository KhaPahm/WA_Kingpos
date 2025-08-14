using System.ComponentModel.DataAnnotations;

namespace WA_Kingpos.Models
{
    public class ClsPhongban
    {
        public string? ma_phongban { get; set; }
        [Required(ErrorMessage = "Tên không được để trống")]
        public string? ten_phongban { get; set; }
        public string? ghichu { get; set; }
        public string? sudung { get; set; }
    }
}
