using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Data.SqlClient;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.Hanghoa
{
    [Authorize]
    public class CreateModel : PageModel
    {
        [BindProperty]
        public clsHanghoa item { get; set; } = new();
        public List<clsNhomhang> Listnhomhang { set; get; } = new List<clsNhomhang>();
        public List<clsDonvitinh> Listdvt { set; get; } = new List<clsDonvitinh>();
        public List<clsHanghoa> ListHH { set; get; } = new List<clsHanghoa>();
        public List<clsBep> ListBEP { set; get; } = new List<clsBep>();
        public string sID_HH { get; set; }
        public string sMAVACH_HH { get; set; }
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
                Loaddata();

                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "Hanghoa Add", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        public IActionResult OnPost(clsHanghoa item)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Loaddata();
                    return Page();
                }
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string sSQL = "";
                SqlConnection CN = new SqlConnection(sConnectionString_live);
                SqlCommand SqlComMain = null;
                Byte[] imageData = null;

                sSQL += "Insert into HANGHOA (TEN_HANGHOA,HINHANH,ID_HANGHOA,THUE,GIANHAP,GIABAN1,GIABAN2,GIABAN3,GIABAN4,GHICHU,HANSUDUNG,MAVACH,SUDUNG,SUAGIA,TONKHO,TONTOITHIEU,MA_THANHPHAN,SOLUONG,MA_NHOMHANG,MA_DONVITINH,MA_BEP,STT,IS_INBEP,SUADINHLUONG,GIATHEOTRONGLUONG,PLU,MONTHEM,INTEM)" + "\n";
                sSQL += "Values (@TEN_HANGHOA,@HINHANH,@ID_HANGHOA,@THUE,@GIANHAP,@GIABAN1,@GIABAN2,@GIABAN3,@GIABAN4,@GHICHU,@HANSUDUNG,@MAVACH,@SUDUNG,@SUAGIA,@TONKHO,@TONTOITHIEU,@MA_THANHPHAN,@SOLUONG,@MA_NHOMHANG,@MA_DONVITINH,@MA_BEP,@STT,@IS_INBEP,@SUADINHLUONG,@GIATHEOTRONGLUONG,@PLU,@MONTHEM,@INTEM)" + "\n";
                SqlCommand SqlCom = new SqlCommand(sSQL, CN);

                SqlCom.Parameters.Add(new SqlParameter("@TEN_HANGHOA", (object)item.TEN));
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
                SqlCom.Parameters.Add(new SqlParameter("@ID_HANGHOA", (object)item.ID_HANGHOA));
                SqlCom.Parameters.Add(new SqlParameter("@THUE", (object)item.THUE.Replace(",", "")));
                SqlCom.Parameters.Add(new SqlParameter("@GIANHAP", (object)item.GIANHAP.Replace(",", "")));
                SqlCom.Parameters.Add(new SqlParameter("@GIABAN1", (object)item.GIABAN1.Replace(",", "")));
                SqlCom.Parameters.Add(new SqlParameter("@GIABAN2", (object)item.GIABAN2.Replace(",", "")));
                SqlCom.Parameters.Add(new SqlParameter("@GIABAN3", (object)item.GIABAN3.Replace(",", "")));
                SqlCom.Parameters.Add(new SqlParameter("@GIABAN4", (object)item.GIABAN4.Replace(",", "")));
                SqlCom.Parameters.Add(new SqlParameter("@GHICHU", (object)item.GHICHU == null ? DBNull.Value : item.GHICHU));
                SqlCom.Parameters.Add(new SqlParameter("@HANSUDUNG", (object)item.HANSUDUNG == null ? DBNull.Value : item.HANSUDUNG));
                SqlCom.Parameters.Add(new SqlParameter("@MAVACH", (object)item.MAVACH));
                SqlCom.Parameters.Add(new SqlParameter("@SUDUNG", (object)item.SUDUNG));
                SqlCom.Parameters.Add(new SqlParameter("@SUAGIA", (object)item.SUAGIA));
                SqlCom.Parameters.Add(new SqlParameter("@TONKHO", (object)item.TONKHO));
                SqlCom.Parameters.Add(new SqlParameter("@TONTOITHIEU", (object)item.TONTOITHIEU.Replace(",", "")));
                SqlCom.Parameters.Add(new SqlParameter("@MA_THANHPHAN", (object)item.MA_THANHPHAN));
                SqlCom.Parameters.Add(new SqlParameter("@SOLUONG", (object)item.SOLUONG.Replace(",", "")));
                SqlCom.Parameters.Add(new SqlParameter("@MA_NHOMHANG", (object)item.MA_NHOMHANG));
                SqlCom.Parameters.Add(new SqlParameter("@MA_DONVITINH", (object)item.MA_DONVITINH));
                SqlCom.Parameters.Add(new SqlParameter("@MA_BEP", (object)item.MA_BEP));
                SqlCom.Parameters.Add(new SqlParameter("@STT", (object)item.STT.Replace(",", "")));
                SqlCom.Parameters.Add(new SqlParameter("@IS_INBEP", (object)item.IS_INBEP));
                SqlCom.Parameters.Add(new SqlParameter("@SUADINHLUONG", (object)item.SUADINHLUONG));
                SqlCom.Parameters.Add(new SqlParameter("@GIATHEOTRONGLUONG", (object)item.GIATHEOTRONGLUONG));
                SqlCom.Parameters.Add(new SqlParameter("@PLU", (object)item.PLU == null ? DBNull.Value : item.PLU));
                SqlCom.Parameters.Add(new SqlParameter("@MONTHEM", (object)item.MONTHEM));
                SqlCom.Parameters.Add(new SqlParameter("@INTEM", (object)item.INTEM));

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
                cls_Main.WriterLog(cls_Main.sFilePath, "Hanghoa Add", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        private void Loaddata()
        {
            //nhomhang
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "Select MA_NHOMHANG,TEN_NHOMHANG From NHOMHANG where SUDUNG=1 order by TEN_NHOMHANG";
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                clsNhomhang nhomhang = new clsNhomhang();
                nhomhang.MA = dr["MA_NHOMHANG"].ToString();
                nhomhang.TEN = dr["TEN_NHOMHANG"].ToString();
                Listnhomhang.Add(nhomhang);
            }
            //Donvitinh
            sSQL = "Select MA_DONVITINH,TEN_DONVITINH From DONVITINH where SUDUNG=1 order by TEN_DONVITINH";
            dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                clsDonvitinh dvt = new clsDonvitinh();
                dvt.ma_donvitinh = dr["MA_DONVITINH"].ToString();
                dvt.ten_donvitinh = dr["TEN_DONVITINH"].ToString();
                Listdvt.Add(dvt);
            }
            //ID_HANGHOA
            sSQL = "select [dbo].[fc_NewCodeID_HANGHOA]  ()";
            dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            if (dt.Rows.Count > 0)
            {
                sID_HH = dt.Rows[0][0].ToString();
            }
            //MAVACH
            sSQL = "select [dbo].[fc_NewCodeID_MaVach]  ()";
            dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            if (dt.Rows.Count > 0)
            {
                sMAVACH_HH = dt.Rows[0][0].ToString();
            }
            //thanhphan
            sSQL = "";
            sSQL += "Select 0 As MA,N'Không sử dụng' As TEN,'' As DONVITINH" + "\n";
            sSQL += "Union" + "\n";
            sSQL += "Select A.MA_HANGHOA As MA,A.TEN_HANGHOA As TEN,B.TEN_DONVITINH As DONVITINH" + "\n";
            sSQL += "From HANGHOA A , DONVITINH B" + "\n";
            sSQL += "Where A.MA_DONVITINH=B.MA_DONVITINH ";
            sSQL += "And A.SUDUNG=1 " + "\n";
            sSQL += "Order by  MA ";
            dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                clsHanghoa hh = new clsHanghoa();
                hh.MA = dr["MA"].ToString();
                hh.TEN = dr["TEN"].ToString();
                ListHH.Add(hh);
            }
            //Bep
            sSQL = "Select MA_BEP,TEN_BEP From DM_BEP where SUDUNG=1 order by TEN_BEP";
            dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                clsBep bep = new clsBep();
                bep.ma_bep = dr["MA_BEP"].ToString();
                bep.ten_bep = dr["TEN_BEP"].ToString();
                ListBEP.Add(bep);
            }

        }

    }
}
