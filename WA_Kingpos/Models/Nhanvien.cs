using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using WA_Kingpos.Data;

namespace WA_Kingpos.Models
{
    public class clsNhanvien
    {
        public string? manhanvien { get; set; }

        [Required(ErrorMessage = "Tên không được để trống")]
        public string? tennhanvien { get; set; }

        [Required(ErrorMessage = "Ngày sinh không được để trống")]
        [JsonConverter(typeof(CustomDateOnlyJsonConverter))]
        public string? ngaysinh { get; set; }

        [Required(ErrorMessage = "Giới tính không được để trống")]
        public string? gioitinh { get; set; }
        public string? diachi { get; set; }
        public string? dienthoai { get; set; }
        [Required(ErrorMessage = "Phòng ban không được để trống")]
        public string? phongban { get; set; }
        public string? email { get; set; }
        public string? ghichu { get; set; }
        public string? sudung { get; set; }

        [JsonConverter(typeof(CustomDateOnlyJsonConverter))]
        public string? NGAYNHANVIEC { get; set; }

        public string? TENCHUCVU { get; set; }


        public string? strGioitinh { get => gioitinh?.ToLower() == "true" || gioitinh == "1" ? "Nam" : gioitinh?.ToLower() == "false" || gioitinh == "0" ? "Nữ" : gioitinh; }


        /// <summary>
        /// 0: Thử việc, 1: chính thức, 2: đã thôi việc
        /// </summary>
        [Required(ErrorMessage = "Trạng thái không được để trống")]
        public int? TRANGTHAI { get; set; } = 0;

        /// <summary>
        /// 0: Thử việc, 1: chính thức, 2: đã thôi việc
        /// </summary>
        public string? HINHTHUC { get => TRANGTHAI != null && listHinhThucNhanVien.ContainsKey(Convert.ToInt32(TRANGTHAI)) ? listHinhThucNhanVien[Convert.ToInt32(TRANGTHAI)] : null; }

        //public int? NGUOIQUANLY { get; set; }

        //public string? TEN_NGUOIQUANLY { get; set; }

        /// <summary>
        /// 0: Thử việc, 1: chính thức, 2: đã thôi việc
        /// </summary>
        public static readonly Dictionary<int, string> listHinhThucNhanVien = new() { { 0, "Thử việc" }, { 1, "Chính thức" }, { 2, "Đã thôi việc" } };
    }
}
