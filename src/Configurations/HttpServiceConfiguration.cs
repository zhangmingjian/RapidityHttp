using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Rapidity.Http.Configurations
{
    /// <summary>
    /// 全局配置信息
    /// </summary>
    public class HttpServiceConfiguration : IHttpServiceConfiguration
    {
        private readonly ArrayList _innerList = new ArrayList();

        public int Count => _innerList.Count;

        /// <summary>
        /// 根据服务名称查找配置
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public HttpServiceConfigureItem Get(string serviceName)
        {
            HttpServiceConfigureItem current = null;
            var enumer = GetEnumerator();
            while (enumer.MoveNext())
            {
                current = enumer.Current;
                if (current.ServiceName == serviceName)
                    return current;
            }
            return null;
        }

        /// <summary>
        /// 通过关联的服务类型查找配置
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public HttpServiceConfigureItem Get(Type type)
        {
            HttpServiceConfigureItem current = null;
            var enumer = GetEnumerator();
            while (enumer.MoveNext())
            {
                current = enumer.Current;
                if (current.ForTypes.FirstOrDefault(x => x == type) != null)
                    return current;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="interfaceSubType">表示当前类型为接口实现类</param>
        /// <returns></returns>
        public HttpServiceConfigureItem Get(Type type, bool interfaceSubType)
        {
            HttpServiceConfigureItem current = null;
            var enumer = GetEnumerator();
            while (enumer.MoveNext())
            {
                current = enumer.Current;
                var result = current.ForTypes.FirstOrDefault(x => x == type 
                                || (interfaceSubType && x.IsInterface && x.IsAssignableFrom(type)));
                if (result != null)
                    return current;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public HttpServiceConfigureItem AddConfigure(HttpServiceConfigureItem config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            if (string.IsNullOrWhiteSpace(config.ServiceName)) throw new ArgumentNullException(nameof(config.ServiceName));
            var configed = Get(config.ServiceName);
            if (configed != null)
                throw new Exception($"请不要重复添加{config.ServiceName}服务");
            _innerList.Add(config);
            return config;
        }

        public IEnumerator<HttpServiceConfigureItem> GetEnumerator()
        {
            return new ConfigureEnumerator(_innerList.ToArray());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        class ConfigureEnumerator : IEnumerator<HttpServiceConfigureItem>
        {
            private readonly object[] configures;
            private int _index = -1;

            public ConfigureEnumerator(object[] array)
            {
                configures = array;
            }

            public bool MoveNext()
            {
                _index++;
                return _index < configures.Length;
            }

            public void Reset()
            {
                _index = 0;
            }

            public HttpServiceConfigureItem Current => (HttpServiceConfigureItem)configures[_index];

            object IEnumerator.Current => Current;

            public void Dispose()
            {
            }
        }
    }
}