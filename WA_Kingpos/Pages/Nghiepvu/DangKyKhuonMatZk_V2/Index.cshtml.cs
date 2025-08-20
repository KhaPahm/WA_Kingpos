using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.DangKyKhuonMatZk_V2
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public const string Perm_DangKyKhuonMatZk = "202002131";

        public List<cls_KS_KhachHang> listitem = new();
        public bool bThem { get; set; }
        public bool bSua { get; set; }
        public bool bXoa { get; set; }

        [BindProperty]
        public bool bOnlyNoFace { get; set; } = true;

        public IActionResult OnGet(bool? id1)
        {
            try
            {
                //check quyen
                var quyen = CheckQuyen();
                if (quyen != null)
                {
                    return quyen;
                }
                //xu ly trang
                LoadData(id1);
                //listitem.Clear();
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "DangKyKhuonMatZk", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }

        public IActionResult OnPostDel(int id)
        {
            // delete...
            return new JsonResult(new { ok = true, id });
        }

        private IActionResult? CheckQuyen()
        {
            if (!cls_UserManagement.AllowView("2025081601", HttpContext.Session.GetString("Permission")))
            {
                return RedirectToPage("/AccessDenied");
            }
            else
            {
                bThem = cls_UserManagement.AllowAdd("2025081601", HttpContext.Session.GetString("Permission"));
                bSua = cls_UserManagement.AllowEdit("2025081601", HttpContext.Session.GetString("Permission"));
                bXoa = cls_UserManagement.AllowDelete("2025081601", HttpContext.Session.GetString("Permission"));
            }
            return null;
        }

        private void LoadData(bool? id1)
        {
            //if (id1 != null)
            //{
            //    bOnlyNoFace = Convert.ToBoolean(id1);
            //}

            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            //  AND IS_ADD_FACE = 0 IS_ADD = 1 
            string sSQL = $"EXEC SP_GET_DS_KS_KHACHHANG";
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            listitem = dt.ToList<cls_KS_KhachHang>();
        }
    }
}
