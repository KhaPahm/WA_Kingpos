using System.ComponentModel.DataAnnotations;

namespace WA_Kingpos.Models
{
    public class clsMayChamCong
    {
        //[Required(ErrorMessage = "Số Serial không được để trống")]
        public string? SerialNumber { get; set; }
        public string? DeviceName { get; set; }

        [Required(ErrorMessage = "Địa chỉ IP không được để trống")]
        public string? IPAddress { get; set; }

        [Required(ErrorMessage = "Gateway không được để trống")]
        public string? GATEIPAddress { get; set; }
        public string? DateTime { get; set; }
        public string? ServerTZ { get; set; }

        [Required(ErrorMessage = "Subnet Mask không được để trống")]
        public string? NetMask { get; set; }
        public string? AutoServerFunOn { get; set; }
        public DateTime? LastConnected { get; set; }

    }
}
