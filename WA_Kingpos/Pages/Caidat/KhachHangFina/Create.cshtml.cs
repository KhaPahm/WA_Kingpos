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

namespace WA_Kingpos.Pages.KHACHANG
{
    [Authorize]
    public class KhachHangCreateModel : PageModel
    {
        [BindProperty]
        public KhachHangModels item { get; set; } = new KhachHangModels();
        public string sIDKHTT = "";
        public string sTen = "";
        public string sDiaChi = "";
        public string sMST = "";
        public IActionResult OnGet(string sten,string sdiachi,string smst)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowAdd("23120801", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                LoadData(sten,sdiachi,smst);
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "DM_KhachHang Add", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        public IActionResult OnPostLuu(KhachHangModels item)
        {
            try
            {
                if (item.ID=="")
                {
                    ModelState.AddModelError(string.Empty, "ID Khách Hàng Không Được Trống");
                }
                if (!CheckMST (item))
                {
                    ModelState.AddModelError("item.FAX", "Mã Số Thuế Này Đã Tồn Tại");
                }
                if (!ModelState.IsValid)
                {
                    LoadData();
                    return Page();
                }
                string? manhanvien = User.FindFirst("UserId")?.Value;
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string sSQL = "";
                sSQL += "Insert into NHACUNGCAP (ID,LOAI,TEN,DIACHI,DIENTHOAI,FAX,EMAIL,MAKHGIOITHIEU,SUDUNG)" + "\n";
                sSQL += "Values ( ";
                sSQL += cls_Main.SQLString(item.ID) + ",";
                sSQL += cls_Main.SQLString("0") + ",";
                sSQL += cls_Main.SQLStringUnicode(item.TEN) + ",";
                sSQL += cls_Main.SQLStringUnicode(item.DIACHI) + ",";
                sSQL += cls_Main.SQLString(item.DIENTHOAI) + ",";
                sSQL += cls_Main.SQLString(item.FAX ?? "") + ",";
                sSQL += cls_Main.SQLString(item.EMAIL ?? "") + ",";
                sSQL += cls_Main.SQLString(manhanvien ?? "") + ",";
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
                cls_Main.WriterLog(cls_Main.sFilePath, "KhachHangFina Add", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        public IActionResult OnPostSearch(KhachHangModels item)
        {
            try
            {
                if (!string.IsNullOrEmpty(item.FAX))
                {
                    string msg = "";
                    string sql = cls_ConfigCashier.sWebServiceURL + item.FAX;
                    cls_TimCongTy.Result_API result = cls_TimCongTy.getAPI(sql, out msg);
                    if (msg.Length > 0)
                    {
                        item.DIACHI = "";
                        item.TEN = "";
                        item.FAX = "";
                    }
                    else
                    {
                        item.DIACHI = result.data.address;
                        item.TEN = result.data.name;
                    }
                }
                return RedirectToPage("Create", new { sten = item.TEN, sdiachi = item.DIACHI, smst = item.FAX });
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "KhachHangFina Search", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        private void LoadData(string sten="", string sdiachi = "", string smst = "")
        {
            // ID
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "select [dbo].[fc_NewCodeID_NHACUNGCAP]  ()";
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            if (dt.Rows.Count > 0)
            {
                sIDKHTT = dt.Rows[0][0].ToString();
            }
            else
            {
                sIDKHTT = "";
            }
            //TEN
            if (!string.IsNullOrEmpty(sten))
            {
                sTen = sten;
            }
            //DIACHI
            if (!string.IsNullOrEmpty(sten))
            {
                sDiaChi = sdiachi;
            }
            //MST
            if (!string.IsNullOrEmpty(smst))
            {
                sMST = smst;
            }
        }
        private bool CheckMST(KhachHangModels KH)
        {
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "";
            if (!string.IsNullOrEmpty(KH.FAX))
            {
                if (string.IsNullOrEmpty(KH.MA))
                {
                    sSQL += "select FAX from  NHACUNGCAP  where FAX=" + cls_Main.SQLString(KH.FAX);
                }
                else
                {
                    sSQL += "select FAX from  NHACUNGCAP  where FAX=" + cls_Main.SQLString(KH.FAX) + " and MA<>" + cls_Main.SQLString(KH.MA);

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
