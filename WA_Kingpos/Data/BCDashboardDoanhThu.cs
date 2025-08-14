namespace WA_Kingpos.Data
{
    public class BCDashboardDoanhThu
    {
        public int stt { get; set; }
        public int maCuaHang { get; set; }
        public string? tenCuaHang { get; set;}
        public string? tenQuay { get; set;}
        public string? combo { get; set; }
        public string? tenHangHoa { get; set; }
        public int soLuong { get; set; }
        public int giaBanThat { get; set; }
        public long tongTien { get; set; }
        public DateTime ngayTao { get; set; }
    }

    public class DashboardSetHangHoa : BCDashboardDoanhThu
    {
        public List<DashboardHangHoa> hh { get; set; } = new List<DashboardHangHoa>();
    }

    public class DashboardHangHoa
    {
        public int maHangHoa { get; set;}
        public string? tenHangHoa { get; set; }
    }
}
