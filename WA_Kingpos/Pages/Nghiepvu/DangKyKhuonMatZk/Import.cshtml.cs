using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.DangKyKhuonMatZk
{
    [Authorize]
    public class ImportModel : PageModel
    {
        public const string Perm_DangKyKhuonMatZk = "202002131";

        [BindProperty]
        public List<clsKhachPhong_NoFace> listitem { get; set; } = new();

        [BindProperty]
        public List<clsGateDevice> listGateDevice { get; set; } = new();


        public bool bThem { get; set; }
        //public bool bXoa { get; set; }

        [BindProperty]
        public bool bOnlyNoFace { get; set; } = true;

        [BindProperty]
        public string sCong { get; set; } = "";

        //[BindProperty]
        //public string sPhong { get; set; } = "";

        //[BindProperty]
        //public Dictionary<string, string?> listPhong { get; set; } = new();

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
                cls_Main.WriterLog(cls_Main.sFilePath, "ImportFaceZk", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }

        public IActionResult OnPost(string submitButton, [FromForm(Name = "sCong")] List<string> listGate)
        {
            //check quyen
            var quyen = CheckQuyen();
            if (quyen != null)
            {
                return quyen;
            }
            var paramRedirect = new { id1 = bOnlyNoFace };
            if (submitButton == "save")
            {
                //var sCong = string.Join(",", listGateDevice.Where(r => listGate.Any(selected => r.ID.ToString() == selected))
                //    // chuyển Id sang gate_no để lưu db
                //    .Select(r => r.GATE_NO).Distinct());
                sCong = string.Join(",", listGate);
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                var Username = User.Claims.FirstOrDefault(c => c.Type == "Username")?.Value;
                var selectedCustomers = listitem.Where(r => r.SUDUNG).ToList();

                //string sSQL = $"UPDATE dbo.DISPLAY_ORDER_FACE SET TRANGTHAI = 0, NGUOISUA = {ExtensionObject.toSqlPar(Username)}, NGAYSUA = GETDATE() WHERE FACE_ID = {ExtensionObject.toSqlPar(face_id)}";
                string sSQL = string.Join("; ", selectedCustomers.Select(r => "EXEC dbo.sp_UpdateFaceIdKhachHang @face_Id = NULL"
                + $",  @ma_kh = {ExtensionObject.toSqlPar(r.MA_KH)}"
                + $",  @madatphong = {ExtensionObject.toSqlPar(r.MADATPHONG)}"
                + $",  @gate_id = {ExtensionObject.toSqlPar(sCong)}"
                + $",  @nguoitao = {ExtensionObject.toSqlPar(Username)}"
                + $",  @ngaybatdau = {ExtensionObject.toSqlPar(r.NGAYNHANPHONG?.ToString("dd/MM/yyyy HH:mm"))}"
                + $",  @ngayketthuc = {ExtensionObject.toSqlPar(r.NGAYTRAPHONG?.ToString("dd/MM/yyyy HH:mm"))}"
                + $",  @only_1 = {ExtensionObject.toSqlPar(r.ONLY_1 ? 1 : 0)}"));


                bool bRunSQL = cls_Main.ExecuteSQL(sSQL, sConnectionString_live);
                if (bRunSQL)
                {

                    TempData["AlertMessage"] = "Thêm thành công";
                    TempData["AlertType"] = "alert-success";
                    return RedirectToPage("Index", paramRedirect);
                }
                else
                {
                    TempData["AlertMessage"] = $"Thêm không thành công";
                    TempData["AlertType"] = "alert-danger";
                    return RedirectToPage("import", paramRedirect);
                }
            }
            //if (submitButton == "view")
            //{
            //    return RedirectToPage("Index", paramRedirect);
            //}
            //else if (submitButton == "delete")
            //{
            //    string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            //    var Username = User.Claims.FirstOrDefault(c => c.Type == "Username")?.Value;
            //    string sSQL = $"UPDATE dbo.DISPLAY_ORDER_FACE SET TRANGTHAI = 0, NGUOISUA = {ExtensionObject.toSqlPar(Username)}, NGAYSUA = GETDATE() WHERE FACE_ID = {ExtensionObject.toSqlPar(face_id)}";
            //    bool bRunSQL = cls_Main.ExecuteSQL(sSQL, sConnectionString_live);
            //    if (bRunSQL)
            //    {

            //        TempData["AlertMessage"] = "Xóa thành công";
            //        TempData["AlertType"] = "alert-success";
            //    }
            //    else
            //    {
            //        TempData["AlertMessage"] = $"Xóa không thành công";
            //        TempData["AlertType"] = "alert-danger";
            //    }
            //    return RedirectToPage("Index", paramRedirect);
            //}



            return RedirectToPage("/AccessDenied");
        }



        private IActionResult? CheckQuyen()
        {
            if (!cls_UserManagement.AllowView(Perm_DangKyKhuonMatZk, HttpContext.Session.GetString("Permission")))
            {
                return RedirectToPage("/AccessDenied");
            }
            else
            {
                bThem = cls_UserManagement.AllowAdd(Perm_DangKyKhuonMatZk, HttpContext.Session.GetString("Permission"));
                //bSua = cls_UserManagement.AllowEdit(Perm_DangKyKhuonMatZk, HttpContext.Session.GetString("Permission"));
                //bXoa = cls_UserManagement.AllowDelete(Perm_DangKyKhuonMatZk, HttpContext.Session.GetString("Permission"));
            }
            return null;
        }

        private void LoadData(bool? id1)
        {
            if (id1 != null)
            {
                bOnlyNoFace = Convert.ToBoolean(id1);
            }

            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            //  AND IS_ADD_FACE = 0
            string sSQL = $"SELECT * FROM dbo.fn_View_KhachPhong_NoFace('') "
                    + $" ORDER BY TENPHONG, TEN  ";

            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            listitem = dt.ToList<clsKhachPhong_NoFace>();
            //if (!string.IsNullOrEmpty(sPhong))
            //{
            //    listitem = listitem.Where(r => $",{sPhong},".Contains($",{r.MAPHONG},")).ToList();
            //}
            //else
            //{
            //    sPhong = string.Join(",", listitem.Select(r => r.MAPHONG).Distinct());
            //}
            //listPhong = listitem.Select(r => new KeyValuePair<string, string?>($"{r.MAPHONG}", r.TENPHONG)).ToDictionary(p => p.Key, p => p.Value);

            sSQL = $"SELECT ID,GATE_NO,IP,NAME,NAME_FULL"
                + "\n FROM dbo.fn_ListZkDeviceByFace('0') ORDER BY NAME_FULL";

            DataTable dt2 = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            listGateDevice = dt2.ToList<clsGateDevice>();
            // mặc định chọn hết cổng
            sCong = string.Join(",", listGateDevice.Select(r => r.ID));
        }
    }
}
