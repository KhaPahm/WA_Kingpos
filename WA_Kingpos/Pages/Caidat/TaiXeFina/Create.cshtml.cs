using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Net.Http.Headers;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using System.Xml.Linq;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.TaiXe
{
    [Authorize]
    public class TaiXeCreateModel : PageModel
    {
        [BindProperty]
        public TaiXeModels item { get; set; } = new TaiXeModels();

        public string sID = "";
        public IActionResult OnGet()
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowAdd("23120802", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                LoadData();
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "TaiXeFina Add", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        public IActionResult OnPost(TaiXeModels item)
        {
            try
            {
                if (item.ID == "")
                {
                    ModelState.AddModelError(string.Empty, "ID Khách Hàng Không Được Trống");
                }
                if (!CheckCMND(item))
                {
                    ModelState.AddModelError("item.CMND", "CMND Này Đã Tồn Tại");
                }
                if (!ModelState.IsValid)
                {
                    LoadData();
                    return Page();
                }
                string? manhanvien = User.FindFirst("UserId")?.Value;
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string sSQL = "";
                sSQL = "";
                sSQL += "Insert into NHACUNGCAP (ID,LOAI,TEN,DIACHI,DIENTHOAI,FAX,EMAIL,WEBSITE,NGUOILIENHE,GHICHU,CMND,NGAYSINH,GIOITINH,LOAIDAILY,CAPDO,TIENDATCOC,ISDATCOC,HANTHANHTOAN,HANMUC_CONGNO,LastUpdate,MAKHGIOITHIEU,SUDUNG)" + "\n";
                sSQL += "Values ( ";
                sSQL += cls_Main.SQLString(item.ID) + ",";
                sSQL += cls_Main.SQLString("1") + ",";
                sSQL += cls_Main.SQLStringUnicode(item.TEN) + ",";
                sSQL += cls_Main.SQLStringUnicode("") + ",";
                sSQL += cls_Main.SQLString(item.DIENTHOAI) + ",";
                sSQL += cls_Main.SQLString("") + ",";
                sSQL += cls_Main.SQLString("") + ",";
                sSQL += cls_Main.SQLString(item.WEBSITE) + ",";
                sSQL += cls_Main.SQLStringUnicode("") + ",";
                sSQL += cls_Main.SQLStringUnicode("") + ",";
                sSQL += cls_Main.SQLString(item.CMND ?? "") + ",";
                sSQL += cls_Main.SQLString(String.Format("{0:yyyyMMdd}", DateTime.Now)) + ",";
                sSQL += cls_Main.SQLBit(true) + ",";
                sSQL += cls_Main.SQLString("1") + ",";
                sSQL += cls_Main.SQLString("1") + ",";

                sSQL += cls_Main.SQLString("0") + ",";
                sSQL += cls_Main.SQLBit(false) + ",";
                sSQL += cls_Main.SQLString("0") + ",";
                sSQL += cls_Main.SQLString("0") + ",";
                sSQL += "GETDATE()" + ",";
                sSQL += cls_Main.SQLString(manhanvien) + ",";
                sSQL += cls_Main.SQLBit(bool.Parse(item.SUDUNG)) + ")";

                bool bRunSQL = cls_Main.ExecuteSQL(sSQL, sConnectionString_live);
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
                cls_Main.WriterLog(cls_Main.sFilePath, "TaiXeFina Add", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        private void LoadData()
        {  
            // ID
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "select [dbo].[fc_NewCodeID_NHACUNGCAP]  ()";
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            if (dt.Rows.Count > 0)
            {
                sID = dt.Rows[0][0].ToString();
            }
            else
            {
                sID = "";
            }
        }
        private bool CheckCMND(TaiXeModels TX)
        {
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "";
            if (!string.IsNullOrEmpty(TX.CMND))
            {
                if (string.IsNullOrEmpty(TX.MA))
                {
                    sSQL += "select CMND from  NHACUNGCAP  where CMND=" + cls_Main.SQLString(TX.CMND);
                }
                else
                {
                    sSQL += "select CMND from  NHACUNGCAP  where CMND=" + cls_Main.SQLString(TX.CMND) + " and MA<>" + cls_Main.SQLString(TX.MA);

                }
                DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                if (dt.Rows.Count > 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
