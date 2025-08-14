using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WA_Kingpos.Models
{
    public class clsHanghoa
    {
        public string? MA { get; set; }

        [Required(ErrorMessage = "Tên không được để trống")]
        public string? TEN { get; set; }
        public string? GHICHU { get; set; }
        public string? SUDUNG { get; set; }

        [Required(ErrorMessage = "Mã nội bộ không được để trống")]
        public string? ID_HANGHOA { get; set; }

        [Required(ErrorMessage = "Đơn vị tính không được để trống")]
        public string? MA_DONVITINH { get; set; }
        public string? TEN_DONVITINH { get; set; }

        [Required(ErrorMessage = "Nhóm hàng không được để trống")]
        public string? MA_NHOMHANG { get; set; }
        public string? TEN_NHOMHANG { get; set; }

        [Required(ErrorMessage = "Mã vạch không được để trống")]
        public string? MAVACH { get; set; }

        [Required(ErrorMessage = "Không được để trống")]
        public string? THUE { get; set; }

        [Required(ErrorMessage = "Không được để trống")]
        public string? GIANHAP { get; set; }

        [Required(ErrorMessage = "Không được để trống")]
        public string? GIABAN1 { get; set; }

        [Required(ErrorMessage = "Không được để trống")]
        public string? GIABAN2 { get; set; }

        [Required(ErrorMessage = "Không được để trống")]
        public string? GIABAN3 { get; set; }

        [Required(ErrorMessage = "Không được để trống")]
        public string? GIABAN4 { get; set; }
        public string? TONKHO { get; set; }

        [Required(ErrorMessage = "Không được để trống")]
        public string? TONTOITHIEU { get; set; }

        [Required(ErrorMessage = "Không được để trống")]
        public string? MA_THANHPHAN { get; set; }
        public string? TEN_THANHPHAN { get; set; }

        [Required(ErrorMessage = "Không được để trống")]
        public string? SOLUONG { get; set; }
        public string? IS_INBEP { get; set; }

        [Required(ErrorMessage = "Không được để trống")]
        public string? MA_BEP { get; set; }
        public string? TEN_BEP { get; set; }
        public string? THUCDON { get; set; }
        public string? GIATHEOTRONGLUONG { get; set; }
        public string? PLU { get; set; }
        public string? HANSUDUNG { get; set; }
        public string? SUAGIA { get; set; }
        public string? SUADINHLUONG { get; set; }
        public string? MONTHEM { get; set; }
        public string? INTEM { get; set; }

        [Required(ErrorMessage = "STT không được để trống")]
        public string? STT { get; set; }
        public Byte[]? HINHANH { get; set; }
        public IFormFile? HINHANH_UPLOAD { get; set; }
        public string? HINHANH_STRING { get; set; }
        public string? HINHANH_KHONGSUDUNG { get; set; }
        public string? CHON { get; set; }
        public string? NOIXUAT { get; set; }
    }
}
