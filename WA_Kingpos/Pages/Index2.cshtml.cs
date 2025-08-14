using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using WA_Kingpos.Data;

namespace WA_Kingpos.Pages
{
    [Authorize]
    public class Index2Model : PageModel
    {
        [BindProperty]
        public string MANHANVIEN_CHON { get; set; }
        public List<DM_NHANVIEN> ListNhanvien { set; get; } = new List<DM_NHANVIEN>();
        public IActionResult OnGet()
        {
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "Select * From DM_NHANVIEN";
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                DM_NHANVIEN Nhanvien = new DM_NHANVIEN();
                Nhanvien.MANHANVIEN = dr["MANHANVIEN"].ToString();
                Nhanvien.TENNHANVIEN = dr["TENNHANVIEN"].ToString();
                ListNhanvien.Add(Nhanvien);
            }
            return Page();
        }

        public IActionResult OnPost()
        {
            if (MANHANVIEN_CHON != null && MANHANVIEN_CHON != "")
            {
                //xu ly luu data....
            }
            return RedirectToPage("/Index2");
        }


        public class DM_NHANVIEN
        {
            public string MANHANVIEN { get; set; }
            public string TENNHANVIEN { get; set; }
        }



    }
}
