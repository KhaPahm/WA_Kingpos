using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Intrinsics.X86;
using System.Text;
using WA_Kingpos.Data;
using WA_Kingpos.Models;
using static WA_Kingpos.Pages.Bep.CreateModel;

namespace WA_Kingpos.Pages.Nhomhang
{
    [Authorize]
    public class EditModel : PageModel
    {
        public clsNhomhang item = new clsNhomhang();
        public IActionResult OnGet(string id)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowEdit("6", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                LoadData(id);
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "Nhomhang Edit : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        public IActionResult OnPost(string id, clsNhomhang item)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    LoadData(id);
                    return Page();
                }
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string sSQL = "";
                SqlConnection CN = new SqlConnection(sConnectionString_live);
                SqlCommand SqlComMain = null;
                Byte[] imageData = null;
                sSQL += "Update NHOMHANG Set TEN_NHOMHANG=@TEN_NHOMHANG,GHICHU=@GHICHU,SUDUNG=@SUDUNG,THUCDON=@THUCDON,STT=@STT,HINHANH=@HINHANH,MONTHEM=@MONTHEM" + "\n";
                sSQL += "Where MA_NHOMHANG=" + cls_Main.SQLString(id) + "\n";
                SqlCommand SqlCom = new SqlCommand(sSQL, CN);
                SqlCom.Parameters.Add(new SqlParameter("@TEN_NHOMHANG", (object)item.TEN));
                SqlCom.Parameters.Add(new SqlParameter("@GHICHU", (object)item.GHICHU == null ? DBNull.Value : item.GHICHU));
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
                    if(bool.Parse (item .HINHANH_KHONGSUDUNG) )
                    {
                        imageParameter.Value = DBNull.Value;
                    }
                    else if(item.HINHANH_STRING !=null)
                    {
                        imageParameter.Value = Convert.FromBase64String(item.HINHANH_STRING);
                    }
                    else
                    {
                        imageParameter.Value = DBNull.Value;
                    }
                }
                SqlCom.Parameters.Add(imageParameter);
                //SqlCom.Parameters.Add(new SqlParameter("@HINHANH", (object)item.HINHANH));
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
                    return RedirectToPage("Index");
                }
                else
                {
                    return RedirectToPage("/ErrorPage", new { id = "Lưu không thành công : " + sSQL });
                }
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "Nhomhang Edit : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        private void LoadData(string id)
        {
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "";
            sSQL += "Select MA_NHOMHANG As MA,TEN_NHOMHANG As TEN,GHICHU,SUDUNG,THUCDON,STT,HINHANH,MONTHEM" + "\n";
            sSQL += "From NHOMHANG" + "\n";
            sSQL += "Where MA_NHOMHANG=" + cls_Main.SQLString(id) + "\n";
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                item.MA = dr["MA"].ToString();
                item.TEN = dr["TEN"].ToString();
                item.GHICHU = dr["GHICHU"].ToString();
                item.SUDUNG = dr["SUDUNG"].ToString();
                item.THUCDON = dr["THUCDON"].ToString();
                item.STT = dr["STT"].ToString();
                item.HINHANH = dr["HINHANH"] == DBNull.Value ? null : (byte[])dr["HINHANH"];
                item.MONTHEM = dr["MONTHEM"].ToString();
                item.HINHANH_STRING = item.HINHANH == null ? null : Convert.ToBase64String(item.HINHANH);
                item.HINHANH_KHONGSUDUNG = "false";
            }
        }
    }
}
