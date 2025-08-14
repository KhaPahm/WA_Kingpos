namespace WA_Kingpos.Data
{
    static class cls_AppSettings
    {
        public static IConfiguration GetValue { get; set; }
        static cls_AppSettings()
        {
            GetValue = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();
        }
        public static string AppId { get => GetValue["Config:AppId"].ToString(); }
        public static string RunMode { get => GetValue["Config:RunMode"].ToString(); }

        public static string Host_BioPush { get => GetValue["ExternalServer:BioPush"].ToString(); }
    }

    public class ConfRunMode
    {
        public const string vetau = "vetau";
        public const string nhahang = "nhahang";
        public const string DkKhuonMat = "DkKhuonMat";
    }

    public class AppId
    {
        public const string fina = "fina";
    }
}
