using System.ComponentModel.DataAnnotations;

namespace WA_Kingpos.Models
{
    public class clsDoimatkhau
    {
        [Required(ErrorMessage = "Không được để trống")]
        public string? matkhaucu { get; set; }

        [Required(ErrorMessage = "Không được để trống")]
        public string? matkhaumoi { get; set; }
      
        [Required(ErrorMessage = "Không được để trống"), Compare(nameof(matkhaumoi))]
        public string? xacnhanmatkhau { get; set; }
       
    }
}
