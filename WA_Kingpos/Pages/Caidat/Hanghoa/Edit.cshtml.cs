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
    public class EditModel : PageModel
    {
        [BindProperty]
        public clsHanghoa item { get; set; } = new();
        public List<clsNhomhang> Listnhomhang { set; get; } = new List<clsNhomhang>();
        public List<clsDonvitinh> Listdvt { set; get; } = new List<clsDonvitinh>();
        public List<clsHanghoa> ListHH { set; get; } = new List<clsHanghoa>();
        public List<clsBep> ListBEP { set; get; } = new List<clsBep>();
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
                cls_Main.WriterLog(cls_Main.sFilePath, "Hanghoa Edit : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        public IActionResult OnPost(string id, clsHanghoa item)
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

                sSQL += "Update HANGHOA Set" + "\n";
                sSQL += "TEN_HANGHOA=@TEN_HANGHOA,HINHANH=@HINHANH,ID_HANGHOA=@ID_HANGHOA,THUE=@THUE,GIANHAP=@GIANHAP,";
                sSQL += "GIABAN1=@GIABAN1,GIABAN2=@GIABAN2,GIABAN3=@GIABAN3,GIABAN4=@GIABAN4,GHICHU=@GHICHU,HANSUDUNG=@HANSUDUNG,MAVACH=@MAVACH,SUDUNG=@SUDUNG,SUAGIA=@SUAGIA,";
                sSQL += "TONKHO=@TONKHO,TONTOITHIEU=@TONTOITHIEU,MA_THANHPHAN=@MA_THANHPHAN,SOLUONG=@SOLUONG,MA_NHOMHANG=@MA_NHOMHANG,";
                sSQL += "MA_DONVITINH=@MA_DONVITINH,MA_BEP=@MA_BEP,STT=@STT,IS_INBEP=@IS_INBEP,SUADINHLUONG=@SUADINHLUONG,GIATHEOTRONGLUONG=@GIATHEOTRONGLUONG,PLU=@PLU,MONTHEM=@MONTHEM,INTEM=@INTEM" + "\n";
                sSQL += "Where MA_HANGHOA=" + cls_Main.SQLString(id) + "\n";
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
                    if (bool.Parse(item.HINHANH_KHONGSUDUNG))
                    {
                        imageParameter.Value = DBNull.Value;
                    }
                    else if (item.HINHANH_STRING != null)
                    {
                        imageParameter.Value = Convert.FromBase64String(item.HINHANH_STRING);
                    }
                    else
                    {
                        imageParameter.Value = DBNull.Value;
                    }
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
                    return RedirectToPage("Index");
                }
                else
                {
                    return RedirectToPage("/ErrorPage", new { id = "Lưu không thành công : " + sSQL });
                }
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "Hanghoa Edit : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        private void LoadData(string id)
        {
            //hanghoa
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "";
            sSQL += "SELECT MA_HANGHOA,ID_HANGHOA,TEN_HANGHOA,H.STT,H.GHICHU,H.MAVACH,TEN_DONVITINH,H.MA_DONVITINH,";
            sSQL += "H.SUDUNG,H.SUAGIA,H.HINHANH,H.MA_NHOMHANG,TEN_NHOMHANG,THUE,GIANHAP,GIABAN1,GIABAN2,GIABAN3,GIABAN4,TONKHO,H.IS_INBEP,";
            sSQL += "THUCDON,TONTOITHIEU,H.SOLUONG,H.MA_BEP,B.TEN_BEP,H.MA_THANHPHAN,(SELECT ISNULL(TEN_HANGHOA,'') FROM HANGHOA WHERE MA_HANGHOA= H.MA_THANHPHAN) AS TEN_THANHPHAN,ISNULL(SUADINHLUONG,0) AS SUADINHLUONG,H.GIATHEOTRONGLUONG,H.PLU, H.HANSUDUNG, H.MONTHEM,H.INTEM" + "\n";
            sSQL += "FROM DONVITINH D,HANGHOA H,NHOMHANG N,DM_BEP B \n";
            sSQL += "WHERE H.MA_NHOMHANG=N.MA_NHOMHANG and H.MA_DONVITINH=D.MA_DONVITINH and H.MA_BEP=B.MA_BEP \n";
            sSQL += "AND MA_HANGHOA=" + cls_Main.SQLString(id) + "\n";
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                item.MA = dr["MA_HANGHOA"].ToString();
                item.ID_HANGHOA = dr["ID_HANGHOA"].ToString();
                item.TEN = dr["TEN_HANGHOA"].ToString();
                item.MA_DONVITINH = dr["MA_DONVITINH"].ToString();
                item.TEN_DONVITINH = dr["TEN_DONVITINH"].ToString();
                item.MA_NHOMHANG = dr["MA_NHOMHANG"].ToString();
                item.TEN_NHOMHANG = dr["TEN_NHOMHANG"].ToString();
                item.MAVACH = dr["MAVACH"].ToString();
                item.THUE = dr["THUE"].ToString();
                item.GIANHAP = dr["GIANHAP"].ToString();
                item.GIABAN1 = dr["GIABAN1"].ToString();
                item.GIABAN2 = dr["GIABAN2"].ToString();
                item.GIABAN3 = dr["GIABAN3"].ToString();
                item.GIABAN4 = dr["GIABAN4"].ToString();
                item.TONKHO = dr["TONKHO"].ToString();
                item.TONTOITHIEU = dr["TONTOITHIEU"].ToString();
                item.MA_THANHPHAN = dr["MA_THANHPHAN"].ToString();
                item.TEN_THANHPHAN = dr["TEN_THANHPHAN"].ToString();
                item.SOLUONG = dr["SOLUONG"].ToString();
                item.IS_INBEP = dr["IS_INBEP"].ToString();
                item.MA_BEP = dr["MA_BEP"].ToString();
                item.TEN_BEP = dr["TEN_BEP"].ToString();
                item.GHICHU = dr["GHICHU"].ToString();
                item.SUDUNG = dr["SUDUNG"].ToString();
                item.THUCDON = dr["THUCDON"].ToString();
                item.GIATHEOTRONGLUONG = dr["GIATHEOTRONGLUONG"].ToString();
                item.PLU = dr["PLU"].ToString();
                item.HANSUDUNG = dr["HANSUDUNG"].ToString();
                item.SUAGIA = dr["SUAGIA"].ToString();
                item.SUADINHLUONG = dr["SUADINHLUONG"].ToString();
                item.MONTHEM = dr["MONTHEM"].ToString();
                item.INTEM = dr["INTEM"].ToString();
                item.STT = dr["STT"].ToString();
                item.HINHANH = dr["HINHANH"] == DBNull.Value ? null : (byte[])dr["HINHANH"];
                item.HINHANH_STRING = item.HINHANH == null ? null : Convert.ToBase64String(item.HINHANH);
                item.HINHANH_KHONGSUDUNG = "false";
            }
            //nhomhang
            sSQL = "Select MA_NHOMHANG,TEN_NHOMHANG From NHOMHANG where SUDUNG=1 order by TEN_NHOMHANG";
            dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
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
