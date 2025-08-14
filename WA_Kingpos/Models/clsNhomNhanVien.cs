
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace WA_Kingpos.Models
{
    public class clsNhomNhanVien
    {
        public int MANHOM { get; set; }

        [Required(ErrorMessage = "Tên không được để trống")]
        public string? TENNHOM_NHANVIEN { get; set; }

        [Required(ErrorMessage = "Người quản lý không được để trống")]
        public int? MA_NGUOIQUANLY { get; set; }
        public string? NGUOIQUANLY { get; set; }
        public string? THANHVIEN { get; set; }
        public string? TEN_THANHVIEN { get; set; }

        public string? THEM_THANHVIEN { get; set; }

        public string? THANHVIEN_JSON
        {
            get { return JsonConvert.SerializeObject(listThanhVien); }
            set
            {
                try
                {
                    listThanhVien = JsonConvert.DeserializeObject<List<clsNhanvien>>(value!)!;
                }
                catch (Exception)
                {
                    listThanhVien = new();
                }

            }
        }

        public string? GHICHU { get; set; }


        public List<clsNhanvien> listThanhVien { get; set; } = new();

        public bool isThanhVien(string? manhanvien)
        {
            return !string.IsNullOrEmpty(manhanvien) && $",{THANHVIEN},".Contains($",{manhanvien},");
        }
    }
}
