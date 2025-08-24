using System.Diagnostics.CodeAnalysis;

namespace WA_Kingpos.Models
{
    public class cls_KS_KhachHang
    {
        [AllowNull]
        public int MA_KH { get; set; } = 0;
        [AllowNull]
        public string TEN { get; set; }
        [AllowNull]
        public string NGAYSINH { get; set; }
        [AllowNull]
        public string GIOITINH { get; set; }
        [AllowNull]
        public string DIACHI { get; set; }
        [AllowNull]
        public string DIENTHOAI { get; set; }
        [AllowNull]
        public string CCCD { get; set; }
        [AllowNull]
        public string EMAIL { get; set; }
        [AllowNull]
        public string QUOCTICH { get; set; }
        [AllowNull]
        public DateTime NGAYTAO { get; set; }
        [AllowNull]
        public string TENNHANVIEN { get; set; }
        [AllowNull]
        public string FACE_PHOTO { get; set; }
        public DateTime TUNGAY { get; set; } = DateTime.Now.Date;
        public DateTime DENNGAY { get; set; } = DateTime.Now.Date;
        public string CONG { get; set; } = ""; //Danh sách cổng
        public string CONG_NAME { get; set; } = "";
        public bool CanEdit { get; set; } = false;
        public bool CanDelete { get; set; } = false;
        public List<int> GetCongSelected()
        {
            if(string.IsNullOrEmpty(CONG))
            {
                return new List<int>();
            }
            var lsString = CONG.Split(',').ToList();
            return lsString.Select(x => int.Parse(x)).ToList();
        }

    }
}
