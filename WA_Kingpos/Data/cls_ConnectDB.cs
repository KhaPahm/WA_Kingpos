using Microsoft.AspNetCore.Mvc;

namespace WA_Kingpos.Data
{
    public class cls_ConnectDB
    {
        public static string SumServer()
        {
            string sSumServer = "1";
            try
            {
                sSumServer = cls_AppSettings.GetValue["ConnectionStrings:SumServer"].ToString();
            }
            catch
            {
                sSumServer = "1";
            }
            return sSumServer;
        }
        public static string GetConnect(string id_Server)
        {
            string sConnectionString_live = "";
            try
            {
                for (int i = 0; i < 1000; i++)
                {
                    if (i.ToString() == id_Server)
                    {
                        sConnectionString_live = cls_AppSettings.GetValue["ConnectionStrings:ConnectionString" + i.ToString()].ToString();
                        break;
                    }
                }
            }
            catch
            {
                sConnectionString_live = "";
            }
            return sConnectionString_live;
        }

        public static string GetConnect_log()
        {
            string sConnectionString_live = "";
            try
            {
                sConnectionString_live = cls_AppSettings.GetValue["ConnectionStrings:ConnectionString"].ToString();
            }
            catch
            {
                sConnectionString_live = "";
            }
            return sConnectionString_live;
        }
    }
}
