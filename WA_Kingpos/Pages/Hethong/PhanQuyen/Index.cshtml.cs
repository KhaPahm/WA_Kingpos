using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data;
using WA_Kingpos.Data;
using WA_Kingpos.Models;

namespace WA_Kingpos.Pages.PhanQuyen
{
    [Authorize]
    public class ConfigModel : PageModel
    {
        [BindProperty]
        [Required(ErrorMessage = "Phải chọn nhóm quyền")]
        public int MANHOMQUYEN { get; set; }

        [BindProperty]
        public List<clsNhomquyen> ListNhomquyen { set; get; } = new List<clsNhomquyen>();

        [BindProperty]
        public List<clsPhanquyen> ListPhanquyen { set; get; } = new List<clsPhanquyen>();
        public IActionResult OnGet(string id)
        {
            try
            {
                //check quyen
                if (!cls_UserManagement.AllowEdit("3", HttpContext.Session.GetString("Permission")))
                {
                    return RedirectToPage("/AccessDenied");
                }
                //xu ly trang
                LoadData(id);
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "sys_group_rule Config : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        public IActionResult OnPost(string submitButton,string id, List<clsPhanquyen> ListPhanquyen)
        {
            try
            {
                if (string.IsNullOrEmpty(submitButton))
                {
                    submitButton = "btn1ReLoad";
                }
                if (submitButton == "btn1ReLoad")
                {
                    return RedirectToPage("Index", new { id = MANHOMQUYEN });
                }
                if (submitButton == "btn2Save")
                {
                    string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                    string sSQL = "";
                    sSQL += "Delete From  SYS_GROUP_RULE Where  GroupID NOT IN (Select GroupID From SYS_GROUP)" + "\n";
                    foreach (clsPhanquyen pq in ListPhanquyen)
                    {
                        if (bool.Parse(pq.toanquyen))
                        {
                            sSQL += "Update sys_group_rule set AllowView=1, AllowAdd=1,AllowEdit=1,AllowDelete=1,AllowPrint=1,AllowImport=1,AllowExport=1 where RuleID=" + cls_Main.SQLString(pq.maquyen) + " AND GroupID=" + cls_Main.SQLString(id) + "\n";
                        }
                        else
                        {
                            sSQL += "Update sys_group_rule set AllowView=" + cls_Main.SQLBit(bool.Parse(pq.xem)) + ",";
                            sSQL += "AllowAdd=" + cls_Main.SQLBit(bool.Parse(pq.them)) + ",";
                            sSQL += "AllowEdit=" + cls_Main.SQLBit(bool.Parse(pq.sua)) + ",";
                            sSQL += "AllowDelete=" + cls_Main.SQLBit(bool.Parse(pq.xoa)) + ",";
                            sSQL += "AllowPrint=" + cls_Main.SQLBit(bool.Parse(pq.print)) + ",";
                            sSQL += "AllowImport=" + cls_Main.SQLBit(bool.Parse(pq.import)) + ",";
                            sSQL += "AllowExport=" + cls_Main.SQLBit(bool.Parse(pq.export)) + " ";
                            sSQL += "where RuleID=" + cls_Main.SQLString(pq.maquyen) + " AND GroupID=" + cls_Main.SQLString(id) + "\n";
                        }
                    }
                    bool bRunSQL = cls_Main.ExecuteSQL(sSQL, sConnectionString_live);
                    if (bRunSQL)
                    {
                        TempData["AlertMessage"] = "Lưu thành công";
                        TempData["AlertType"] = "alert-success";
                        return RedirectToPage("Index", new { id = MANHOMQUYEN });
                    }
                    else
                    {
                        return RedirectToPage("/ErrorPage", new { id = "Lưu không thành công : " + sSQL });
                    }
                }
                LoadData(string.Empty);
                return Page();
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "sys_group_rule Config : " + id, ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
        private void LoadData(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                id = "0";
            }
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "";
            //tu add SYS_GROUP_RULE
            sSQL = "";
            sSQL += "Select RuleID From SYS_RULE " + "\n";
            sSQL += "Where Active = " + cls_Main.SQLString("1") + "\n";
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            sSQL = "";
            sSQL += "Select GroupID From SYS_GROUP " + "\n";
            sSQL += "Where GroupID NOT IN (Select GroupID From SYS_GROUP_RULE)" + "\n";
            DataTable dt1 = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            sSQL = "";
            foreach (DataRow dr1 in dt1.Rows)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    sSQL += "Insert into SYS_GROUP_RULE (GroupID,RuleID)" + "\n";
                    sSQL += "Values ( ";
                    sSQL += cls_Main.SQLString(dr1["GroupID"].ToString()) + ",";
                    sSQL += cls_Main.SQLString(dr["RuleID"].ToString()) + ")" + "\n";
                }
            }
            cls_Main.ExecuteSQL(sSQL, sConnectionString_live);
            //nhom quyen          
            sSQL = "Select * From SYS_GROUP Where Active=1";
            dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                clsNhomquyen nhomquyen = new clsNhomquyen();
                nhomquyen.ma_nhomquyen = dr["GroupID"].ToString();
                nhomquyen.ten_nhomquyen = dr["GroupName"].ToString();
                nhomquyen.ghichu = dr["Description"].ToString();
                nhomquyen.sudung = dr["Active"].ToString();
                ListNhomquyen.Add(nhomquyen);
            }
            //phan quyen
            sSQL = "EXEC SP_W_PHANQUYEN @MAQUYEN=" + cls_Main.SQLString(id);
            dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            foreach (DataRow dr in dt.Rows)
            {
                clsPhanquyen phanquyen = new clsPhanquyen();
                phanquyen.nhomquyen = dr["GroupRule"].ToString();
                phanquyen.maquyen = dr["RuleID"].ToString();
                phanquyen.quyen = dr["RuleName"].ToString();
                phanquyen.xem = dr["AllowView"].ToString();
                phanquyen.them = dr["AllowAdd"].ToString();
                phanquyen.sua = dr["AllowEdit"].ToString();
                phanquyen.xoa = dr["AllowDelete"].ToString();
                phanquyen.print = dr["AllowPrint"].ToString();
                phanquyen.import = dr["AllowImport"].ToString();
                phanquyen.export = dr["AllowExport"].ToString();
                if (phanquyen.xem.ToLower() == "true" && phanquyen.them.ToLower() == "true" && phanquyen.sua.ToLower() == "true" && phanquyen.xoa.ToLower() == "true" && phanquyen.print.ToLower() == "true" && phanquyen.import.ToLower() == "true" && phanquyen.export.ToLower() == "true")
                {
                    phanquyen.toanquyen = "true";
                }
                else
                {
                    phanquyen.toanquyen = "false";
                }
                ListPhanquyen.Add(phanquyen);
            }
            //manhomquyen
            if (!string.IsNullOrEmpty(id))
            {
                MANHOMQUYEN = int.Parse(id);
            }
        }
    }
}
