# RapidityHttp
RapidityHttp是对HttpClient的包装，内含重试，熔断降级，请求缓存，请求日志，用于简化/快速对接第三方RestApi。(RapidityHttp is a wrapper for HttpClient, including retry, blowdown, cache, request log, purpose is convenient, fast docking RestApi)

配置服务及注入：

    public void ConfigureServices(IServiceCollection services)
        {
            services.UseRapidityHttp().ConfigRecordStore<TextInvokeRecordStore>().AddService("wechat", config =>
            {
                config.BaseAddress = "https://api.weixin.qq.com/";
                config.Timeout = 60;
                config.Item.ContentType = "application/json";
                config.Item.DefaultHeaders.Add("customHeader", "fromtest");
            }).For<ITokenService>();
            services.BuildProxy(); //生成服务代理
            
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }
  
  服务定义时只需定义接口实现IHttpService，或者使用HttpServiceAttribute标签, 组件自动生成接口代理实现类
  
    1.继承自IHttpService接口
    public interface ITokenService : IHttpService
    {
        [Get("/cgi-bin/token")]
        Task<AccessToken> GetToken([Query] string appid, [Query]string secret, [Query(Name = "grant_type")]string grantType = "client_credential");

        [Get("/cgi-bin/getcallbackip?access_token={token}")]
        Task<ResponseWrapper<IpList>> GetIpListAsync([Query] string token);

        [Post("/cgi-bin/message/template/send?access_token={token}")]
        [Header("header:{header}")]
        void SendTemplateMsg(string token, string header, [Body(CanNull = false)] MessageTemplate template);
    }
    
    2. 使用HttpServiceAttribute标签
    
     [HttpService]
     public interface ITokenService
     {
        [Get("/cgi-bin/token")]
        Task<AccessToken> GetToken([Query] string appid, [Query]string secret, [Query(Name = "grant_type")]string grantType = "client_credential");

        [Get("/cgi-bin/getcallbackip?access_token={token}")]
        Task<ResponseWrapper<IpList>> GetIpListAsync([Query] string token);

        [Post("/cgi-bin/message/template/send?access_token={token}")]
        [Header("header:{header}")]
        void SendTemplateMsg(string token, string header, [Body(CanNull = false)] MessageTemplate template);
    }
    
 使用服务：
 
    [ApiController]
    public class TokenController : Controller
    {
        private readonly ITokenService _tokenService;

        public TokenController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        // GET api/values
        [HttpGet]
        public async Task<AccessToken> Get()
        {
            var token = await _tokenService.GetToken("**********", "**********");
            return token;
        }
    }
