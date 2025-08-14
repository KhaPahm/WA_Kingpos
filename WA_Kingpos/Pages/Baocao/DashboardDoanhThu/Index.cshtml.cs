using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using WA_Kingpos.Data;
using static WA_Kingpos.Pages.Baocao.DSHanghoa.IndexModel;
using System.Collections.Generic;
using WA_Kingpos.Models;
using static WA_Kingpos.Pages.Bep.CreateModel;
using System.Linq;

namespace WA_Kingpos.Pages.Baocao.DashboardDoanhThu
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public List<BCDashboardDoanhThu> listHD { set; get; } = new List<BCDashboardDoanhThu>();
        [BindProperty]
        public List<CUAHANGFILTER> ListCuaHang { set; get; } = new List<CUAHANGFILTER>();
        [BindProperty]
        public List<DashboardSetHangHoa> setsDashboard { set; get; } = new List<DashboardSetHangHoa>();
        [BindProperty]
        public int ReloadTime { get; set; }
        public long huytra = 0;
        public long tienmat = 0;
        public long nganhang = 0;
        public long khac = 0;
        public long tongcong = 0;
        public IActionResult OnGet(string id, string id1, string id2, string id3)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowView("48", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                LoadData();
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "DSHangHoa Get", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }

        public IActionResult OnPost(string submitButton, string id, string id1, string id2, string id3)
        {
            try
            {
                if (submitButton == "view")
                {
                    //string[] dateParts = sTungay.Split(new string[] { " - " }, StringSplitOptions.None);
                    return RedirectToPage("Index");
                }
                
                LoadData();
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "DSHangHoa Post : ", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }

        public IActionResult OnPostFilter([FromForm] List<CUAHANGFILTER> ListCuaHang)
        {
            string sSQL = "";
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");

            sSQL += "EXEC DASHBOARD_DOANHTHU";

            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            int i = 0;

            List<BCDashboardDoanhThu> listHDF = new List<BCDashboardDoanhThu>();

            foreach (DataRow dr in dt.Rows)
            {
                BCDashboardDoanhThu item = new BCDashboardDoanhThu();
                item.stt = ++i;
                item.maCuaHang = Convert.ToInt32(dr["MA_CUAHANG"].ToString());
                item.tenCuaHang = dr["TEN_CUAHANG"].ToString();
                item.tenQuay = dr["TEN_QUAY"].ToString();
                item.combo = dr["COMBO_CHINH"].ToString();
                item.tenHangHoa = dr["TEN_HANGHOA"].ToString();
                item.soLuong = Convert.ToInt32(dr["SOLUONG"].ToString());
                item.giaBanThat = Convert.ToInt32(dr["GIABAN_THAT"].ToString());
                item.tongTien = Convert.ToInt64(dr["TONGTIEN"].ToString());
                item.ngayTao = Convert.ToDateTime(dr["NGAYTAO"].ToString());

                listHDF.Add(item);
            }

            // Lọc các cửa hàng đã được chọn
            var selectedStores = ListCuaHang.Where(store => store.IsSelected).ToList();

            List<BCDashboardDoanhThu> bc = new List<BCDashboardDoanhThu>();
            if (!selectedStores.Exists(t => t.Ma == null || t.Ma.Equals("")))
            {
                foreach (var store in selectedStores)
                {
                    var selectedHD = listHDF.Where(t => t.maCuaHang.ToString().Equals(store.Ma)).ToList();
                    bc.AddRange(selectedHD);
                }

                listHD = bc;
            }
            else
            {
                listHD = listHDF;
            }

            setsDashboard = ConvertRawDataToSet(listHD);

            //tongtien
            string chuoicuahang = String.Join(", ", selectedStores.Select(store => store.Ma));

            sSQL = "EXEC DASHBOARD_TONGTIEN '" + chuoicuahang + "'";

            dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                var resultHuyTra = Int64.TryParse(dr["HUYTRA"].ToString(), out huytra);
                if (resultHuyTra == false)
                    huytra = 0;

                var reultTienMat = Int64.TryParse(dr["TIENMAT"].ToString(), out tienmat);
                if (reultTienMat == false)
                    tienmat = 0;

                var resultNganHang = Int64.TryParse(dr["NGANHANG"].ToString(), out nganhang);
                if (resultNganHang == false)
                    nganhang = 0;

                var resultKhac = Int64.TryParse(dr["KHAC"].ToString(), out khac);
                if (resultKhac == false)
                    khac = 0;

                var resultTongCong = Int64.TryParse(dr["TONGCONG"].ToString(), out tongcong);
                if (resultTongCong == false)
                    tongcong = 0;
            }

            return Page();
        }

        private void LoadData()
        {
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "";

            sSQL = "EXEC SP_GET_CUAHANG";
            DataTable dtCuahang = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);

            foreach (DataRow dr in dtCuahang.Rows)
            {
                CUAHANGFILTER Cuahang = new CUAHANGFILTER();
                Cuahang.Ma = dr["MA"].ToString();
                Cuahang.Ten = dr["TEN"].ToString();
                Cuahang.IsSelected = Convert.ToBoolean(dr["IsSelected"].ToString());
                ListCuaHang.Add(Cuahang);
            }

            sSQL = "EXEC DASHBOARD_DOANHTHU";

            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            int i = 0;

            foreach (DataRow dr in dt.Rows)
            {
                BCDashboardDoanhThu item = new BCDashboardDoanhThu();
                item.stt = ++i;
                item.maCuaHang = Convert.ToInt32(dr["MA_CUAHANG"].ToString());
                item.tenCuaHang = dr["TEN_CUAHANG"].ToString();
                item.tenQuay = dr["TEN_QUAY"].ToString();
                item.combo = dr["COMBO_CHINH"].ToString();
                item.tenHangHoa = dr["TEN_HANGHOA"].ToString();
                item.soLuong = Convert.ToInt32(dr["SOLUONG"].ToString());
                item.giaBanThat = Convert.ToInt32(dr["GIABAN_THAT"].ToString());
                item.tongTien = Convert.ToInt64(dr["TONGTIEN"].ToString());
                item.ngayTao = Convert.ToDateTime(dr["NGAYTAO"].ToString());

                listHD.Add(item);
            }

            setsDashboard = ConvertRawDataToSet(listHD);

            sSQL = "EXEC DASHBOARD_TONGTIEN '0'";
            
            dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                var resultHuyTra = Int64.TryParse(dr["HUYTRA"].ToString(), out huytra);
                if (resultHuyTra == false)
                    huytra = 0;

                var reultTienMat = Int64.TryParse(dr["TIENMAT"].ToString(), out tienmat);
                if (reultTienMat == false)
                    tienmat = 0;

                var resultNganHang = Int64.TryParse(dr["NGANHANG"].ToString(), out nganhang);
                if (resultNganHang == false)
                    nganhang = 0;

                var resultKhac = Int64.TryParse(dr["KHAC"].ToString(), out khac);
                if (resultKhac == false)
                    khac = 0;

                var resultTongCong = Int64.TryParse(dr["TONGCONG"].ToString(), out tongcong);
                if (resultTongCong == false)
                    tongcong = 0;

            }
        }

        private List<DashboardSetHangHoa> ConvertRawDataToSet(List<BCDashboardDoanhThu> rawData)
        {
            List<DashboardSetHangHoa> sets = new List<DashboardSetHangHoa>();
            DashboardSetHangHoa currentSet = new DashboardSetHangHoa();

            foreach(var item in rawData)
            {
                if(item.giaBanThat > 0)
                {
                    currentSet = new DashboardSetHangHoa
                    {
                        maCuaHang = item.maCuaHang,
                        tenCuaHang = item.tenCuaHang,
                        tenQuay = item.tenQuay,
                        combo = item.combo,
                        tenHangHoa = item.tenHangHoa,
                        soLuong = item.soLuong,
                        giaBanThat = item.giaBanThat,
                        tongTien = item.tongTien,
                        ngayTao = item.ngayTao,
                        hh = new List<DashboardHangHoa>()
                    };
                    sets.Add(currentSet);
                }
                else if (currentSet != null)
                {
                    currentSet.hh.Add(new DashboardHangHoa { tenHangHoa = item.tenHangHoa });
                }
            }

            int i = 1;
            foreach (var item in sets) 
            {
                if (item.hh != null)
                {
                    foreach (var thh in item.hh)
                    {
                        item.tenHangHoa += "<br>" + " - " + thh.tenHangHoa;
                    }
                }
                item.stt = i;
                i++;
            }

            return sets;
        }
    }
}

public class CUAHANGFILTER
{
    public string Ma { get; set; }
    public string Ten { get; set; }
    public bool IsSelected { get; set; }
}