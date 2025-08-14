using System.ComponentModel.DataAnnotations;

namespace WA_Kingpos.Models
{
    public class clsConfigreport
    {
        public string? tencongty { get; set; }
        public string? diachi { get; set; }
        public string? dienthoai { get; set; }
        public string? fax { get; set; }
        public string? email { get; set; }
        public string? sotaikhoan { get; set; }
        public string? masothue { get; set; }
        public string? mausohd { get; set; }
        public string? nguoigiao { get; set; }
        public string? ketoan { get; set; }
        public string? truongphong { get; set; }
        public string? kyhieu { get; set; }
        public string? nguoinhan { get; set; }
        public string? thukho { get; set; }
        public string? giamdoc { get; set; }
        public Byte[]? logo1 { get; set; }
        public IFormFile? logo1_UPLOAD { get; set; }
        public string? logo1_STRING { get; set; }
        public Byte[]? logo2 { get; set; }
        public IFormFile? logo2_UPLOAD { get; set; }
        public string? logo2_STRING { get; set; }
    }
}
