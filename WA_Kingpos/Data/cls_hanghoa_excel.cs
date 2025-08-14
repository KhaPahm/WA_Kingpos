namespace WA_Kingpos.Data
{
    public class cls_hanghoa_excel
    {
        public string ten_hanghoa { get; set; } = "";
        public double? sl_goi { get; set; }
        public double? giaban { get; set; }
        public double? hesokpi { get; set; }
        public string ten_nhomhang { get; set; } = "";
        public string tenkho { get; set; } = "";
        public string ten_donvitinh { get; set; } = "";
        public string ghichu { get; set; } = "";

        public string noi_sanxuat { get; set; } = "";

        public override string ToString()
        {
            return $"{ten_hanghoa}|{noi_sanxuat}|{sl_goi}|{giaban}|{hesokpi}|{ten_nhomhang}|{tenkho}|{ten_donvitinh}|{ghichu}";
        }
    }
}



