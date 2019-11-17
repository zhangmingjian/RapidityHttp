﻿using Microsoft.Extensions.DependencyInjection;
using Rapidity.Http.Extensions;
using System;

namespace Rapidity.Http.Tests
{
    public class Startup
    {
        public static IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddRapidityHttp().ConfigDefaultRecordStore().AddHttpService("wechat", config =>
            {
                config.BaseAddress = "https://api.weixin.qq.com/";
            });

            services.BuildProxy();

            return services.BuildServiceProvider();
        }
    }
}