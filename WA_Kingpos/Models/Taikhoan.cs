using System.ComponentModel.DataAnnotations;

namespace WA_Kingpos.Models
{
    public class clsTaikhoan
    {
        [Required(ErrorMessage = "Tài khoản không được để trống"), MinLength(1)]
        public string? userid { get; set; }

        [Required(ErrorMessage = "Nhóm quyền không được để trống"), MinLength(1)]
        public string? nhomquyen { get; set; }

        [Required(ErrorMessage = "Nhân viên không được để trống"), MinLength(1)]
        public string? nhanvien { get; set; }

        public string? ngaysinh { get; set; }
        public string? gioitinh { get; set; }
        public string? diachi { get; set; }
        public string? dienthoai { get; set; }
        public string? trangthai { get; set; }
        public string? thietbi { get; set; }
        public string? thoiggian { get; set; }
        public string? tientamung { get; set; }
        public string? sudung { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống"), MinLength(1)]
        public string? matkhau { get; set; }
    }
}
