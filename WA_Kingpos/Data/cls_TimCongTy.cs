using RestSharp;
using System.Net;
using Newtonsoft.Json;
namespace WA_Kingpos.Data
{
    public class cls_TimCongTy
    {
        public string MST { get; set; }

        public class Result_API
        {
            public string code { get; set; }
            public string desc { get; set; }
            public Result_Data data { get; set; }
        }

        public class Result_Data
        {
            public string id { get; set; }
            public string name { get; set; }
            public string address { get; set; }
        }

        public static Result_API getAPI(string sRequestUriString, out string msg)
        {
            Result_API kq = new Result_API();
            msg = "";
            try
            {
                var client = new RestClient(sRequestUriString);
                var request = new RestRequest();
                request.AddHeader("Accept", "application/json");
                request.AddHeader("Content-Type", "application/json");
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                RestResponse response = client.Execute(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var content = response.Content;
                    Result_API? result_API = JsonConvert.DeserializeObject<Result_API>(content);
                    kq = result_API;
                }
                //ghi log
                if (kq.code == "00")
                {
                    msg = "";
                }
                else
                {
                    msg = kq.desc;
                }
            }
            catch
            {
                msg = "Lỗi kết nối API Mã Số Thuế";
            }
            return kq;
        }
    }
}
