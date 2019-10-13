using System.Threading.Tasks;
using Rapidity.Http;
using Rapidity.Http.Attributes;

namespace Sample.Service
{
    /// <summary>
    /// 
    /// </summary>
    public interface ITokenService : IHttpService
    {
        [Get("/cgi-bin/token")]
        Task<AccessToken> GetToken([Query] string appid, [Query]string secret, [Query(Name = "grant_type")]string grantType = "client_credential");

        [Get("/cgi-bin/getcallbackip?access_token={token}")]
        Task<IpList> GetIpListAsync([Query] string token);

        [Post("/cgi-bin/message/template/send?access_token={token}")]
        Task SendTemplateMsg(string token, [Body(CanNull = false)] MessageTemplate template);
    }


    public class AccessToken
    {
        public string access_token { get; set; }

        public int expires_in { get; set; }
    }

    public class IpList
    {
        public string[] ip_list { get; set; }
    }


    public class MessageTemplate
    {
        public string touser { get; set; }
        public string template_id { get; set; }
        public string url { get; set; }
        public string topcolor { get; set; }
        public Data data { get; set; }

        public class Data
        {
            public User User { get; set; }
            public Date Date { get; set; }
            public Cardnumber CardNumber { get; set; }
            public Type Type { get; set; }
            public Money Money { get; set; }
            public Deadtime DeadTime { get; set; }
            public Left Left { get; set; }
        }

        public class User
        {
            public string value { get; set; }
            public string color { get; set; }
        }

        public class Date
        {
            public string value { get; set; }
            public string color { get; set; }
        }

        public class Cardnumber
        {
            public string value { get; set; }
            public string color { get; set; }
        }

        public class Type
        {
            public string value { get; set; }
            public string color { get; set; }
        }

        public class Money
        {
            public string value { get; set; }
            public string color { get; set; }
        }

        public class Deadtime
        {
            public string value { get; set; }
            public string color { get; set; }
        }

        public class Left
        {
            public string value { get; set; }
            public string color { get; set; }
        }
    }
}