using System;
using System.Collections.Generic;

namespace Rapidity.Http.Configurations
{
    /// <summary>
    /// 配置系统
    /// 一个应用程序可以包含N个httpService,一个httpService下会有N个Module, module的配置会覆盖service中的配置项
    /// </summary>
    public interface IHttpServiceConfiguration : IEnumerable<HttpServiceConfigure>
    {
        int Count { get; }

        HttpServiceConfigure Get(string serviceName);

        HttpServiceConfigure Get(Type type);

        HttpServiceConfigure AddConfigure(HttpServiceConfigure config);

    }
}