using System;
using Newtonsoft.Json;

namespace Rapidity.Http.Serializes
{
    public class NewtonsoftJsonSerializer : ContentSerializer
    {
        private readonly NewtonsoftJsonOption _option;

        /// <summary>
        /// 使用驼峰命名格式
        /// </summary>
        public NewtonsoftJsonSerializer() : this(NewtonsoftJsonOption.CamelCaseStyle())
        {
        }

        public NewtonsoftJsonSerializer(NewtonsoftJsonOption option)
        {
            _option = option;
        }

        public override string Serialize(object obj)
        {
            return _option?.Settings != null
                ? JsonConvert.SerializeObject(obj, _option.Settings)
                : JsonConvert.SerializeObject(obj);
        }

        public override object Deserialize(string content, Type type)
        {
            return _option?.Settings != null
                ? JsonConvert.DeserializeObject(content, type, _option.Settings)
                : JsonConvert.DeserializeObject(content, type);
        }
    }

    public class NewtonsoftJsonOption
    {
        /// <summary>
        /// JsonSerializerSettings
        /// </summary>
        public JsonSerializerSettings Settings { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static NewtonsoftJsonOption CamelCaseStyle()
        {
            var option = new NewtonsoftJsonOption
            {
                Settings = new JsonSerializerSettings
                {
                    //驼峰命名属性（生成小写字母开头的属性）
                    ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
                }
            };
            return option;
        }
    }
}