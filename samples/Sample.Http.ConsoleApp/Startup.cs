using Microsoft.Extensions.DependencyInjection;
using Rapidity.Http;
using Rapidity.Http.Extensions;
using Sample.Service;
using System;

namespace Sample.Http.ConsoleApp
{
    public class Startup
    {
        public static IServiceProvider ConfigureServices(IServiceCollection services)
        {
            if (services == null) services = new ServiceCollection();
            services.AddRapidityHttp().AddHttpService("wechat", config =>
            {
                config.BaseAddress = "https://api.weixin.qq.com/";
                config.Timeout = 60;
                config.Option.ContentType = MimeTypes.Application.Json;

                config.Option.RetryOption = new Rapidity.Http.Configurations.RetryOption { TransientErrorRetry = false };

                config.AddModule("GetUserList", httpOption =>
                {
                    httpOption.CacheOption = new Rapidity.Http.Configurations.CacheOption
                    {
                        Enabled = true,
                        ExpireIn = TimeSpan.FromSeconds(30),
                        ExpireType = Rapidity.Http.Configurations.ExpireType.Sliding
                    };
                });
            }).ForTypes(typeof(ITokenService), typeof(IUserService));
            services.ConfigDefaultRecordStore().BuildProxy(Rapidity.Http.DynamicProxies.CodeGeneratorOption.Default);

            return services.BuildServiceProvider();
        }
    }
}