using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RestSharp;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text.Json;
using WA_Kingpos.Data;
using WA_Kingpos.Models;
using static OfficeOpenXml.ExcelErrorValue;

namespace WA_Kingpos.Pages.HoaDon.MST
{
    public class IndexModel : PageModel
    {
        public ThongTinKhachHang thongTinKhach = new ThongTinKhachHang();
        public HoaDon hoaDon = new HoaDon();
        public List<CongTy> lsCongTy = new List<CongTy>();
        public CongTy congTy = new CongTy();
        public CongTy congTyLoad = new CongTy();

        [BindProperty] public string Id { get; set; }
        [BindProperty] public string idServer { get; set; }
        [BindProperty] public string idType { get; set; }
        [BindProperty] public string idHoaDon { get; set; }
        [BindProperty] public string idCongTy { get; set; }

        public IActionResult OnGet(string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id) && id.Split('-').Length == 3)
                {
                    string[] parts = id.Split('-');
                    idServer = parts[0];
                    idType = parts[1];
                    idHoaDon = parts[2];
                    string sConnectionString = cls_ConnectDB.GetConnect(idServer);
                    Id = id;

                    string sSQL_KH = string.Empty;
                    string sSQL_HD = string.Empty;

                    if (idType.Trim().ToLower().Equals("con"))
                    {
                        sSQL_KH = "EXEC SP_SELECT_VE_HOADON " + cls_Main.SQLString(idHoaDon) + ",'3'";
                        sSQL_HD = "EXEC SP_SELECT_HOADON_THUE " + cls_Main.SQLString(idHoaDon) + ",'3'";
                    }
                    else if (idType.Trim().ToLower().Equals("box"))
                    {
                        sSQL_KH = "EXEC SP_SELECT_VE_HOADON " + cls_Main.SQLString(idHoaDon) + ",'1'";
                        sSQL_HD = "EXEC SP_SELECT_HOADON_THUE " + cls_Main.SQLString(idHoaDon) + ",'1'";
                    }
                    else if (idType.Trim().ToLower().Equals("bida"))
                    {
                        sSQL_KH = "EXEC SP_SELECT_VE_HOADON " + cls_Main.SQLString(idHoaDon) + ",'7'";
                        sSQL_HD = "EXEC SP_SELECT_HOADON_THUE " + cls_Main.SQLString(idHoaDon) + ",'7'";
                    }

                    DataTable dtKH = cls_Main.ReturnDataTable_NoLock(sSQL_KH, sConnectionString);
                    if (dtKH != null && dtKH.Rows.Count > 0)
                    {
                        idCongTy = dtKH.Rows[0]["MAKHACHHANG"].ToString();
                        thongTinKhach = new ThongTinKhachHang
                        {
                            MaKhachHang = dtKH.Rows[0]["MAKHACHHANG"].ToString(),
                            TenCongTy = dtKH.Rows[0]["TENKHACHHANG"].ToString(),
                            DiaChi = dtKH.Rows[0]["DIACHI"].ToString(),
                            Email = dtKH.Rows[0]["EMAIL"].ToString(),
                            SoDienThoai = dtKH.Rows[0]["DIENTHOAI"].ToString(),
                            MST = dtKH.Rows[0]["MST"].ToString(),
                            GhiChu = dtKH.Rows[0]["GHICHU"].ToString(),
                            STK = dtKH.Rows[0]["SOTAIKHOAN"].ToString(),
                            NganHang = dtKH.Rows[0]["NGANHANG"].ToString(),
                        };

                        congTyLoad = new CongTy
                        {
                            MaCongTy = dtKH.Rows[0]["MAKHACHHANG"].ToString(),
                            TenCongTy = dtKH.Rows[0]["TENKHACHHANG"].ToString(),
                        };
                    }
                    else
                    {
                        thongTinKhach = null;
                    }


                    DataTable dtHD = cls_Main.ReturnDataTable_NoLock(sSQL_HD, sConnectionString);
                    if (dtHD != null && dtHD.Rows.Count > 0)
                    {
                        hoaDon = new HoaDon
                        {
                            TenCuaHang = dtHD.Rows[0]["TEN_CUAHANG"].ToString(),
                            MaHoaDon = dtHD.Rows[0]["MA_HOADON"].ToString(),
                            NhanVien = dtHD.Rows[0]["TENNHANVIEN"].ToString(),
                            Quay = dtHD.Rows[0]["TEN_QUAY"].ToString(),
                            NgayTao = dtHD.Rows[0]["NGAYTAO"].ToString(),
                            TongTien = string.Format("{0:N0}", dtHD.Rows[0]["TONGTIEN"]),
                            SoPhut = dtHD.Rows[0]["SoPhut"].ToString(),
                            Thongbao_SoPhut = dtHD.Rows[0]["thongbao_sophut"].ToString(),
                            Sosanh_SoPhut = dtHD.Rows[0]["sosanh_sophut"].ToString(),
                            LinkHD = dtHD.Rows[0]["linkHD"].ToString(),
                            IdHD = dtHD.Rows[0]["GUID"].ToString(),
                            CTHoaDons = new List<CTHoaDon>()
                        };
                        foreach (DataRow row in dtHD.Rows)
                        {
                            var ct = new CTHoaDon
                            {
                                STT = row["STT"].ToString(),
                                MaHangHoa = row["MA_HANGHOA"].ToString(),
                                TenHangHoa = row["TEN_HANGHOA"].ToString(),
                                SoLuong = string.Format("{0:N1}", row["SOLUONG"]),
                                ChietKhau = row["CHIETKHAU"].ToString(),
                                GiaBan = string.Format("{0:N0}", row["GIABAN_THAT"])
                            };
                            hoaDon.CTHoaDons.Add(ct);
                        }

                        if (!string.IsNullOrWhiteSpace(hoaDon.IdHD))
                        {
                            return Redirect(hoaDon.LinkHD + hoaDon.IdHD);
                        }
                    }
                    else
                    {
                        hoaDon = null;
                    }

                    return Page();
                }
                else
                {
                    return RedirectToPage("/ErrorPageCine", new { id = 1 });
                }
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "Get HoaDonMST : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPageCine", new { id = ex.ToString() });
            }
        }
        public async Task<JsonResult> OnGetFindMSTAsync(string mst)
        {
            try
            {
                var resultVietQR = await GetBusinessInfoAsync(mst);
                if (resultVietQR != null && resultVietQR.Code == "00")
                {
                    return new JsonResult(new
                    {
                        success = true,
                        result = new
                        {
                            tenCongTy = resultVietQR.Data.Name,
                            diaChi = resultVietQR.Data.Address,
                            email = "", // Bạn có thể bổ sung nếu có
                            ghiChu = ""//resultVietQR.Data.ShortName
                        }
                    });
                }
                else
                {
                    return new JsonResult(new { success = false });
                }
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "TraCuuMST : " + mst, ex.ToString(), "0");
                return new JsonResult(new { success = false, error = ex.Message });
            }
        }
        public async Task<BusinessApiResponse> GetBusinessInfoAsync(string id)
        {
            var httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(3)
            };

            try
            {
                var url = $"https://api.vietqr.io/v2/business/{id}";

                var response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<BusinessApiResponse>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return result;
                }

                return null;
            }
            catch (TaskCanceledException ex)
            {
                // Đây là lỗi xảy ra nếu hết thời gian timeout
                // Console.WriteLine("Timeout khi gọi API: " + ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                // Các lỗi khác như DNS, mạng, deserialize...
                //Console.WriteLine("Lỗi khi gọi API: " + ex.Message);
                return null;
            }
            finally
            {
                httpClient.Dispose();
            }
        }
        public async Task<IActionResult> OnPostSaveAsync(string mst, string maCTy, string tenCongTy, string email, string soDienThoai, string STK, string NganHang, string DiaChi, string GhiChu)
        {
            try
            {
                SaveInfo(mst, tenCongTy, email, soDienThoai, STK, NganHang, DiaChi, GhiChu);
                return RedirectToPage(new { id = Id }); // Refresh trang
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "Post HoaDonMST : ", ex.ToString(), "0");
                return RedirectToPage("/ErrorPageCine", new { id = ex.ToString() });
            }
        }
        public void SaveInfo(string mst, string tenCongTy, string email, string sdt, string STK, string nganHang, string diaChi, string ghiChu)
        {
            string sNhanvientao = cls_AppSettings.GetValue["Config:UserMST"].ToString();
            string sConnectionString = cls_ConnectDB.GetConnect(idServer);
            string sSQL = "select MAKHACHHANG,TENKHACHHANG,DIACHI,EMAIL,DIENTHOAI,MST,GHICHU,SOTAIKHOAN,NGANHANG from DM_KHACHHANG where MAVE= " + cls_Main.SQLString(idHoaDon);
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString);
            sSQL = "";
            if (idType.Trim().ToLower().Equals("con"))
            {
                sSQL = "EXEC SP_INSERT_UPDATE_KHACHHANG_HOADON " + "\n";
            }
            else if (idType.Trim().ToLower().Equals("box"))
            {
                sSQL = "EXEC SP_INSERT_UPDATE_KHACHHANG_HOADONVE " + "\n";
            }
            else if (idType.Trim().ToLower().Equals("bida"))
            {
                sSQL = "EXEC SP_INSERT_UPDATE_KHACHHANG_HOADON_KARAOKE " + "\n";
            }
            if (dt != null && dt.Rows.Count > 0)
            {
                idCongTy = dt.Rows[0]["MAKHACHHANG"].ToString();
                sSQL += cls_Main.SQLStringUnicode(tenCongTy) + ",";
                sSQL += cls_Main.SQLStringUnicode(diaChi) + ",";
                sSQL += cls_Main.SQLStringUnicode(mst) + ",";
                sSQL += cls_Main.SQLStringUnicode(sdt) + ",";
                sSQL += cls_Main.SQLStringUnicode(STK) + ",";
                sSQL += cls_Main.SQLStringUnicode("") + ",";
                sSQL += cls_Main.SQLStringUnicode(ghiChu) + ",";
                sSQL += cls_Main.SQLStringUnicode(sNhanvientao) + ",";
                sSQL += cls_Main.SQLStringUnicode(email) + ",";
                sSQL += cls_Main.SQLStringUnicode(idHoaDon) + ",";
                sSQL += cls_Main.SQLStringUnicode(idCongTy) + ",";
                sSQL += cls_Main.SQLStringUnicode("2") + "\n";
                cls_Main.ExecuteSQL(sSQL, sConnectionString);
                TempData["Message"] = "Thông tin xuất hóa đơn đã được cập nhật thành công!";
                TempData["AlertType"] = "success";
            }
            else
            {
                sSQL += cls_Main.SQLStringUnicode(tenCongTy) + ",";
                sSQL += cls_Main.SQLStringUnicode(diaChi) + ",";
                sSQL += cls_Main.SQLStringUnicode(mst) + ",";
                sSQL += cls_Main.SQLStringUnicode(sdt) + ",";
                sSQL += cls_Main.SQLStringUnicode(STK) + ",";
                sSQL += cls_Main.SQLStringUnicode("") + ",";
                sSQL += cls_Main.SQLStringUnicode(ghiChu) + ",";
                sSQL += cls_Main.SQLStringUnicode(sNhanvientao) + ",";
                sSQL += cls_Main.SQLStringUnicode(email) + ",";
                sSQL += cls_Main.SQLStringUnicode(idHoaDon) + ",";
                sSQL += cls_Main.SQLStringUnicode(idCongTy) + ",";
                sSQL += cls_Main.SQLStringUnicode("1") + "\n";
                cls_Main.ExecuteSQL(sSQL, sConnectionString);
                TempData["Message"] = "Thông tin xuất hóa đơn  đã được thêm thành công!";
                TempData["AlertType"] = "success";
            }
        }

    }


    public class ThongTinKhachHang
    {
        [Required(ErrorMessage = "Vui lòng nhập MST")]
        public string MST { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên công ty")]
        public string TenCongTy { get; set; }

        public string TenNguoiMua { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        public string DiaChi { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string SoDienThoai { get; set; }

        public string CCCD { get; set; }
        public string NguoiMuaHang { get; set; }
        public string STK { get; set; }

        public string NganHang { get; set; }
        public string GhiChu { get; set; }
        public string MaKhachHang { get; set; }
    }
    public class CongTy
    {
        public string MaCongTy { get; set; }
        public string TenCongTy { get; set; }
    }
    public class HoaDon
    {
        public string TenCuaHang { get; set; }
        public string MaHoaDon { get; set; }
        public string NhanVien { get; set; }
        public string Quay { get; set; }
        public string NgayTao { get; set; }
        public string TongTien { get; set; }
        public string SoPhut { get; set; }
        public string Thongbao_SoPhut { get; set; }
        public string Sosanh_SoPhut { get; set; }
        public string LinkHD { get; set; }
        public string IdHD { get; set; }
        public List<CTHoaDon> CTHoaDons { get; set; }
    }
    public class CTHoaDon
    {
        public string STT { get; set; }
        public string MaHangHoa { get; set; }
        public string TenHangHoa { get; set; }
        public string SoLuong { get; set; }
        public string ChietKhau { get; set; }
        public string GiaBan { get; set; }
    }
    public class BusinessData
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string InternationalName { get; set; }
        public string ShortName { get; set; }
        public string Address { get; set; }
    }
    public class BusinessApiResponse
    {
        public string Code { get; set; }
        public string Desc { get; set; }
        public BusinessData Data { get; set; }
    }

}
