using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Data.SqlClient;
using System.Xml.Linq;
using WA_Kingpos.Data;
using WA_Kingpos.Models;
using static System.Net.Mime.MediaTypeNames;

namespace WA_Kingpos.Pages.Nhomhang
{
    [Authorize]
    public class CreateModel : PageModel
    {
        [BindProperty]
        public clsNhomhang item { get; set; }
        public IActionResult OnGet()
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowAdd("6", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "Nhomhang Add", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        public IActionResult OnPost(clsNhomhang item)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string sSQL = "";
                SqlConnection CN = new SqlConnection(sConnectionString_live);
                SqlCommand SqlComMain = null;
                Byte[] imageData = null;
                sSQL += "Insert into NHOMHANG (TEN_NHOMHANG,GHICHU,SUDUNG,THUCDON,STT,HINHANH,MONTHEM) Values (@TEN_NHOMHANG,@GHICHU,@SUDUNG,@THUCDON,@STT,@HINHANH,@MONTHEM)" + "\n";
                SqlCommand SqlCom = new SqlCommand(sSQL, CN);
                SqlCom.Parameters.Add(new SqlParameter("@TEN_NHOMHANG", (object)item.TEN));
                SqlCom.Parameters.Add(new SqlParameter("@GHICHU", (object)item.GHICHU==null?DBNull.Value:item.GHICHU));
                SqlCom.Parameters.Add(new SqlParameter("@SUDUNG", (object)item.SUDUNG));
                SqlCom.Parameters.Add(new SqlParameter("@THUCDON", (object)item.THUCDON));
                SqlCom.Parameters.Add(new SqlParameter("@STT", (object)item.STT));
                SqlParameter imageParameter = new SqlParameter("@HINHANH", SqlDbType.Image);
                if (item.HINHANH_UPLOAD != null)
                {
                    using (Stream fs = item.HINHANH_UPLOAD.OpenReadStream())
                    {
                        using (BinaryReader br = new BinaryReader(fs))
                        {
                            imageData = br.ReadBytes((Int32)fs.Length);
                        }
                    }
                    imageParameter.Value = imageData;
                }
                else
                {
                    imageParameter.Value = DBNull.Value;
                }
                SqlCom.Parameters.Add(imageParameter);
                //SqlCom.Parameters.Add(new SqlParameter("@HINHANH", (object)imageData));
                SqlCom.Parameters.Add(new SqlParameter("@MONTHEM", (object)item.MONTHEM));
                SqlComMain = SqlCom;
                bool bRunSQL = true;
                try
                {
                    CN.Open();
                    SqlComMain.ExecuteNonQuery();
                    CN.Close();
                    bRunSQL = true;
                }
                catch
                {
                    bRunSQL = false;
                }
                if (bRunSQL)
                {
                    TempData["AlertMessage"] = "Lưu thành công";
                    TempData["AlertType"] = "alert-success";
                    return RedirectToPage("Create");
                }
                else
                {
                    return RedirectToPage("/ErrorPage", new { id = "Lưu không thành công : " + sSQL });
                }
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "Nhomhang Add", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
    }
}
