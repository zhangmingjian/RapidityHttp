using Newtonsoft.Json;
using Rapidity.Http;
using Rapidity.Http.Attributes;
using Rapidity.Http.DynamicProxies;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Service
{
    public interface IUserService : IHttpService
    {
        [Get("/cgi-bin/user/get?access_token={token}")]
        [Retry(1000, 2000, 3000, TotalTimeout = 5000)]
        UserList GetUserList(string token);

        [Get("/cgi-bin/user/info")]
        [Header("call-from:testserver")]
        Task<UserInfo> GetUserInfo([Query("access_token")]string token, [Query]string openid, [Query]string lang);

    }

    public class UserList
    {
        public int Total { get; set; }
        public int Count { get; set; }
        public OpenidList Data { get; set; }
        [JsonProperty("next_openid")]
        public string NextOpenid { get; set; }

        public class OpenidList
        {
            public string[] openid { get; set; }
        }
    }

    public class UserInfo
    {
        public int Subscribe { get; set; }
        public string openid { get; set; }
        public string nickname { get; set; }
        public string headimgurl { get; set; }
    }
}
