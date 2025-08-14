using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WA_Kingpos.Models
{
    public class HangHoaModels
    {
        public string? STT { get; set; }
        public string? TEN_HANGHOA { get; set; }
        public string? MA_HANGHOA { get; set; }
        public string? MA_VACH { get; set; }
        public string? TEN { get; set; }
        public string? DVT { get; set; }
        public string? GIANHAP { get; set; }
        public string? SL { get; set; }
        public string? DONGIA { get; set; }
        public string? THANHTIEN { get; set; }
        public string? DUYET { get; set; }
    }
}
