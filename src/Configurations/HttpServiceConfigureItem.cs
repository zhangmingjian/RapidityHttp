using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Rapidity.Http.Configurations
{
    /// <summary>
    /// httpservice的配置
    /// 一个应用程序可以包含N个httpService,一个httpService下会有N个Module, module的配置会覆盖service中的配置项
    /// </summary>
    public class HttpServiceConfigureItem
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// uri主机
        /// </summary>
        public string BaseAddress
        {
            get => this.Option.Uri;
            set => this.Option.Uri = value;
        }

        /// <summary>
        /// 单次请求超时时间（秒）
        /// </summary>
        public int Timeout { get; set; } = 30;

        /// <summary>
        /// 表示哪些类型能使用当前服务配置
        /// </summary>
        public ICollection<Type> ForTypes { get; set; } = new HashSet<Type>();

        /// <summary>
        /// 请求相关参数配置，通用部分，module内的配置会覆盖root中的内容
        /// </summary>
        public HttpOption Option { get; set; } = new HttpOption();

        /// <summary>
        /// 服务下的模块配置
        /// </summary>
        public IDictionary<string, HttpOption> ModuleOptions { get; set; } = new Dictionary<string, HttpOption>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        public HttpOption GetHttpOption(string moduleName)
        {
            if (string.IsNullOrEmpty(moduleName) || !ModuleOptions.ContainsKey(moduleName))
                return this.Option;
            var moduleOption = ModuleOptions[moduleName];
            return moduleOption.Union(this.Option, true);
        }
    }
}