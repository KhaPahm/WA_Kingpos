using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Security.Claims;
using System.Text.Json;
using System.Text;
using WA_Kingpos.Data;
using WA_Kingpos.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Reflection;
using System.Security.Principal;

namespace WA_Kingpos.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public User item { get; set; }
        public void OnGet()
        {
        }
        public IActionResult OnPost(User item)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }
                string sConnectionString_live = cls_ConnectDB.GetConnect("0");
                string sSQL = "EXEC GetUser " + cls_Main.SQLString(item.Username) + " , " + cls_Main.SQLString(item.Password);
                DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
                if (dt.Rows.Count == 0)
                {
                    ModelState.AddModelError(string.Empty, "Sai Tài Khoản hoặc Mật Khẩu");
                    return Page();
                }
                else
                {
                    //HttpContext.Session.SetString("username", item.Username);
                    List<GroupRule> listgrouprule = new List<GroupRule>();
                    DataTable dtPer = cls_UserManagement.LoadPermission(dt.Rows[0]["GroupID"].ToString());
                    foreach (DataRow dr in dtPer.Rows)
                    {
                        GroupRule rule = new GroupRule();
                        rule.groupid = dr["GroupID"].ToString();
                        rule.ruleid = dr["RuleID"].ToString();
                        rule.allowview = dr["AllowView"].ToString();
                        rule.allowadd = dr["AllowAdd"].ToString();
                        rule.allowedit = dr["AllowEdit"].ToString();
                        rule.allowdelete = dr["AllowDelete"].ToString();
                        rule.allowprint = dr["AllowPrint"].ToString();
                        rule.allowimport = dr["AllowImport"].ToString();
                        rule.allowexport = dr["AllowExport"].ToString();

                        listgrouprule.Add(rule);
                    }
                    string jsonPer = JsonSerializer.Serialize(listgrouprule);
                    HttpContext.Session.SetString("Permission", jsonPer);

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, item.Username),
                        new Claim("Username", item.Username),
                        //new Claim("Password", item.Password),
                        new Claim("UserId", dt.Rows[0]["MaNV"].ToString ()),
                        new Claim("GroupID", dt.Rows[0]["GroupID"].ToString ()),
                        new Claim(ClaimTypes.Role, dt.Rows[0]["Role"].ToString ()),
                    };
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(claimsIdentity);
                    var ExpireTime = cls_AppSettings.GetValue["Cookie:ExpireTime"];
                    var authProperties = new AuthenticationProperties
                    {
                        //AllowRefresh = <bool>,
                        // Refreshing the authentication session should be allowed.

                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(int.Parse(ExpireTime)),
                        // The time at which the authentication ticket expires. A 
                        // value set here overrides the ExpireTimeSpan option of 
                        // CookieAuthenticationOptions set with AddCookie.

                        //IsPersistent = true,
                        // Whether the authentication session is persisted across 
                        // multiple requests. When used with cookies, controls
                        // whether the cookie's lifetime is absolute (matching the
                        // lifetime of the authentication ticket) or session-based.

                        //IssuedUtc = <DateTimeOffset>,
                        // The time at which the authentication ticket was issued.

                        //RedirectUri = <string>
                        // The full path or absolute URI to be used as an http 
                        // redirect response value.
                    };
                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);
                    return RedirectToPage("Index");
                }
            }
            catch (Exception ex)
            {
                cls_Main.WriterLog(cls_Main.sFilePath, "Login", ex.ToString(), "0");
                return RedirectToPage("/ErrorPage", new { id = ex.ToString() });
            }
        }
    }
}
