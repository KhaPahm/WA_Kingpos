using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Data.SqlClient;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.ConfigReport
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public clsConfigreport item =new clsConfigreport();
        public IActionResult OnGet()
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowAdd("35", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                LoadData();
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "SYS_CONFIGREPORT", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        private void LoadData()
        {
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "SELECT * FROM SYS_CONFIGREPORT";
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            //
            item.tencongty = dt.Rows[0]["TENCONGTY"].ToString();
            item.diachi = dt.Rows[0]["DIACHI"].ToString();
            item.dienthoai = dt.Rows[0]["SODIENTHOAI"].ToString();
            item.fax = dt.Rows[0]["SOFAX"].ToString();
            item.email = dt.Rows[0]["EMAIL"].ToString();
            item.sotaikhoan = dt.Rows[0]["SOTAIKHOAN"].ToString();
            item.masothue = dt.Rows[0]["MASOTHUE"].ToString();
            item.mausohd = dt.Rows[0]["MAUSOPHIEUHD"].ToString();
            item.nguoigiao = dt.Rows[0]["NGUOIGIAO"].ToString();
            item.ketoan = dt.Rows[0]["KETOAN"].ToString();
            item.truongphong = dt.Rows[0]["TRUONGPHONG"].ToString();
            item.kyhieu = dt.Rows[0]["KYHIEU"].ToString();
            item.nguoinhan = dt.Rows[0]["NGUOINHAN"].ToString();
            item.thukho = dt.Rows[0]["THUKHO"].ToString();
            item.giamdoc = dt.Rows[0]["GIAMDOC"].ToString();
            item.logo1 = dt.Rows[0]["HINHANH"] == DBNull.Value ? null : (byte[])dt.Rows[0]["HINHANH"];
            item.logo1_STRING = item.logo1 == null ? null : Convert.ToBase64String(item.logo1);
            item.logo2 = dt.Rows[0]["HINHANH1"] == DBNull.Value ? null : (byte[])dt.Rows[0]["HINHANH1"];
            item.logo2_STRING = item.logo2 == null ? null : Convert.ToBase64String(item.logo2);
        }
        public IActionResult OnPost(clsConfigreport item)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    LoadData();
                    return Page();
                }
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string sSQL = "";
                SqlConnection CN = new SqlConnection(sConnectionString_live);
                SqlCommand SqlComMain = null;
                Byte[] imageData = null;

                sSQL += "Update SYS_CONFIGREPORT Set " + "\n";
                sSQL += "TENCONGTY=@TENCONGTY,DIACHI=@DIACHI,SODIENTHOAI=@DIENTHOAI,SOFAX=@FAX,EMAIL=@EMAIL,SOTAIKHOAN=@SOTAIKHOAN,";
                sSQL += "MASOTHUE=@MASOTHUE,MAUSOPHIEUHD=@MAUSOHD,NGUOIGIAO=@NGUOIGIAO,KETOAN=@KETOAN,TRUONGPHONG=@TRUONGPHONG,KYHIEU=@KYHIEU,";
                sSQL += "NGUOINHAN=@NGUOINHAN,THUKHO=@THUKHO,GIAMDOC=@GIAMDOC,HINHANH=@LOGO1,HINHANH1=@LOGO2";
                SqlCommand SqlCom = new SqlCommand(sSQL, CN);

                SqlCom.Parameters.Add(new SqlParameter("@TENCONGTY", (object)item.tencongty == null ? DBNull.Value : item.tencongty));
                SqlCom.Parameters.Add(new SqlParameter("@DIACHI", (object)item.diachi == null ? DBNull.Value : item.diachi));
                SqlCom.Parameters.Add(new SqlParameter("@DIENTHOAI", (object)item.dienthoai == null ? DBNull.Value : item.dienthoai));
                SqlCom.Parameters.Add(new SqlParameter("@FAX", (object)item.fax == null ? DBNull.Value : item.fax));
                SqlCom.Parameters.Add(new SqlParameter("@EMAIL", (object)item.email == null ? DBNull.Value : item.email));
                SqlCom.Parameters.Add(new SqlParameter("@SOTAIKHOAN", (object)item.sotaikhoan == null ? DBNull.Value : item.sotaikhoan));
                SqlCom.Parameters.Add(new SqlParameter("@MASOTHUE", (object)item.masothue == null ? DBNull.Value : item.masothue));
                SqlCom.Parameters.Add(new SqlParameter("@MAUSOHD", (object)item.mausohd == null ? DBNull.Value : item.mausohd));
                SqlCom.Parameters.Add(new SqlParameter("@NGUOIGIAO", (object)item.nguoigiao == null ? DBNull.Value : item.nguoigiao));
                SqlCom.Parameters.Add(new SqlParameter("@KETOAN", (object)item.ketoan == null ? DBNull.Value : item.ketoan));
                SqlCom.Parameters.Add(new SqlParameter("@TRUONGPHONG", (object)item.truongphong == null ? DBNull.Value : item.truongphong));
                SqlCom.Parameters.Add(new SqlParameter("@KYHIEU", (object)item.kyhieu == null ? DBNull.Value : item.kyhieu));
                SqlCom.Parameters.Add(new SqlParameter("@NGUOINHAN", (object)item.nguoinhan == null ? DBNull.Value : item.nguoinhan));
                SqlCom.Parameters.Add(new SqlParameter("@THUKHO", (object)item.thukho == null ? DBNull.Value : item.thukho));
                SqlCom.Parameters.Add(new SqlParameter("@GIAMDOC", (object)item.giamdoc == null ? DBNull.Value : item.giamdoc));
                //logo1
                SqlParameter imageParameter = new SqlParameter("@LOGO1", SqlDbType.Image);
                if (item.logo1_UPLOAD != null)
                {
                    using (Stream fs = item.logo1_UPLOAD.OpenReadStream())
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
                   if (item.logo1_STRING != null)
                    {
                        imageParameter.Value = Convert.FromBase64String(item.logo1_STRING);
                    }
                    else
                    {
                        imageParameter.Value = DBNull.Value;
                    }
                }
                SqlCom.Parameters.Add(imageParameter);
                //logo2
                SqlParameter imageParameter2 = new SqlParameter("@LOGO2", SqlDbType.Image);
                if (item.logo2_UPLOAD != null)
                {
                    using (Stream fs = item.logo2_UPLOAD.OpenReadStream())
                    {
                        using (BinaryReader br = new BinaryReader(fs))
                        {
                            imageData = br.ReadBytes((Int32)fs.Length);
                        }
                    }
                    imageParameter2.Value = imageData;
                }
                else
                {
                    if (item.logo2_STRING != null)
                    {
                        imageParameter2.Value = Convert.FromBase64String(item.logo2_STRING);
                    }
                    else
                    {
                        imageParameter2.Value = DBNull.Value;
                    }
                }
                SqlCom.Parameters.Add(imageParameter2);

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
                    return RedirectToPage("Index");
                }
                else
                {
                    return RedirectToPage("/ErrorPage", new { id = "Lưu không thành công : " + sSQL });
                }
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "SYS_CONFIGREPORT UPDATE ", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
    }
}

