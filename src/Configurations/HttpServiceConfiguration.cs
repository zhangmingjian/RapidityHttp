using System;
using System.Collections;
using System.Collections.Generic;

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
        public HttpServiceConfigure Get(string serviceName)
        {
            HttpServiceConfigure current = null;
            // ReSharper disable once GenericEnumeratorNotDisposed
            var enumer = GetEnumerator();
            while (enumer.MoveNext())
            {
                current = enumer.Current;
                if (current != null && current.ServiceName == serviceName)
                    return current;
            }
            return null;
        }

        /// <summary>
        /// 通过关联的服务类型查找配置
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public HttpServiceConfigure Get(Type type)
        {
            HttpServiceConfigure current = null;
            // ReSharper disable once GenericEnumeratorNotDisposed
            var enumer = GetEnumerator();
            while (enumer.MoveNext())
            {
                current = enumer.Current;
                if (current != null && current.ForTypes.Contains(type))
                    return current;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public HttpServiceConfigure AddConfigure(HttpServiceConfigure config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            if (string.IsNullOrWhiteSpace(config.ServiceName)) throw new ArgumentNullException(nameof(config.ServiceName));
            var configed = Get(config.ServiceName);
            if (configed != null)
                throw new Exception($"请不要重复添加{config.ServiceName}服务");
            _innerList.Add(config);
            return config;
        }

        public IEnumerator<HttpServiceConfigure> GetEnumerator()
        {
            return new ConfigureEnumerator(_innerList.ToArray());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        class ConfigureEnumerator : IEnumerator<HttpServiceConfigure>
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

            public HttpServiceConfigure Current => (HttpServiceConfigure)configures[_index];

            object IEnumerator.Current => Current;

            public void Dispose()
            {
            }
        }
    }
}