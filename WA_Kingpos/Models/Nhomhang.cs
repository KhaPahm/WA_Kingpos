using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WA_Kingpos.Models
{
    public class clsNhomhang
    {
        public string? MA { get; set; }

        [Required(ErrorMessage = "Tên không được để trống")]
        public string? TEN { get; set; }
        public string? GHICHU { get; set; }
        public string? SUDUNG { get; set; }
        public string? THUCDON { get; set; }

        [Required(ErrorMessage = "STT không được để trống")]
        public string? STT { get; set; }
        public Byte[]? HINHANH { get; set; }
        public string? MONTHEM { get; set; }
        public string? INBEP { get; set; }
        public string? MABEP { get; set; }
        public IFormFile? HINHANH_UPLOAD { get; set; }
        public string? CHON { get; set; }
        public string? HINHANH_STRING { get; set; }
        public string? HINHANH_KHONGSUDUNG { get; set; }

    }
}
