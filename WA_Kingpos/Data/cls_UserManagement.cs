using System.Data;
using System.Security;
using System.Security.Cryptography;
using System.Text.Json;
using WA_Kingpos.Models;

namespace WA_Kingpos.Data
{
    public class cls_UserManagement
    {
        public static DataTable LoadPermission(string sGroupID)
        {
            string sConnectionString_live = cls_ConnectDB.GetConnect("0");
            string sSQL = "";
            sSQL += "Select * From SYS_GROUP_RULE" + "\n";
            sSQL += "Where GroupID=" + cls_Main.SQLString(sGroupID) + "\n";
            DataTable dt = cls_Main.ReturnDataTable_NoLock(sSQL, sConnectionString_live);
            return dt;
        }
        public static bool AllowView(string sRuleID, string sPermission)
        {
            try
            {
                List <GroupRule> listgrouprule = JsonSerializer.Deserialize<List<GroupRule>>(sPermission);
                GroupRule rule = listgrouprule.Find(item => item.ruleid == sRuleID);
                if(rule ==null )
                {
                    return false;
                }
                else if (bool.Parse(rule.allowview))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        public static bool AllowAdd(string sRuleID, string sPermission)
        {
            try
            {
                List<GroupRule> listgrouprule = JsonSerializer.Deserialize<List<GroupRule>>(sPermission);
                GroupRule rule = listgrouprule.Find(item => item.ruleid == sRuleID);
                if (rule == null)
                {
                    return false;
                }
                else if (bool.Parse(rule.allowadd))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        public static bool AllowEdit(string sRuleID, string sPermission)
        {
            try
            {
                List<GroupRule> listgrouprule = JsonSerializer.Deserialize<List<GroupRule>>(sPermission);
                GroupRule rule = listgrouprule.Find(item => item.ruleid == sRuleID);
                if (rule == null)
                {
                    return false;
                }
                else if (bool.Parse(rule.allowedit))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        public static bool AllowDelete(string sRuleID, string sPermission)
        {
            try
            {
                List<GroupRule> listgrouprule = JsonSerializer.Deserialize<List<GroupRule>>(sPermission);
                GroupRule rule = listgrouprule.Find(item => item.ruleid == sRuleID);
                if (rule == null)
                {
                    return false;
                }
                else if (bool.Parse(rule.allowdelete))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
