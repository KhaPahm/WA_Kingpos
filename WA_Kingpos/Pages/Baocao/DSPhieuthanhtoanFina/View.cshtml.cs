using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.Baocao.DSPhieuthanhtoanFina
{
    [Authorize]
    public class ViewModel : PageModel
    {
        public clsDathang item = new clsDathang();

        public List<clsDathang> listitem = new List<clsDathang>();
        public int? tongsoluong { get; set; } = 0;
        public long? tongthanhtien { get; set; } = 0;
        public IActionResult OnGet(string id)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowAdd("23120902", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                LoadData(id);

                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "DSDondathangFina view", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }

        private void LoadData(string id)
        {
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "EXEC SP_FINA_CHITIETPHIEUTT @MAPHIEU=" + cls_Main.SQLString(id);
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            item.maphieu = dt.Rows[0]["MAPDNDT"].ToString();
            item.nhanvien = dt.Rows[0]["TENNHANVIEN"].ToString(); // nguoi lap phieu
            item.ngaylap = String.Format("{0:dd/MM/yyyy}",dt.Rows[0]["NGAYLAPPHIEU"]);
            item.email = dt.Rows[0]["LYDO"].ToString(); // ly do
            item.thanhtien = int.Parse(dt.Rows[0]["TONGTIEN"].ToString()).ToString("N0");
            //
            foreach (DataRow dr in dt.Rows)
            {
                clsDathang item = new clsDathang();
                item.tensanpham = dr["HANGHOA"].ToString();
                item.khachhang = dr["TEN"].ToString();
                item.ngaygiaohang = String.Format("{0:dd/MM/yyyy}", dr["NGAYLAPPHIEU1"]);
                item.bienso = dr["TENNHANVIEN_DH"].ToString();
                item.sotien = decimal.Parse(dr["THANHTIEN"].ToString()).ToString("N0");
                item.stt = (1 + dt.Rows.IndexOf(dr)).ToString();

                listitem.Add(item);

                tongthanhtien = tongthanhtien + long.Parse(dr["TONGTIEN"].ToString());
            }
        }
    }
}
