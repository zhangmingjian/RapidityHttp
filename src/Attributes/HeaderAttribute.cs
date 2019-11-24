using System;

namespace Rapidity.Http.Attributes
{
    /// <summary>
    /// RequestHeader设置可用于方法，参数，类属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class
                    | AttributeTargets.Parameter | AttributeTargets.Property
                    | AttributeTargets.Method, AllowMultiple = true)]
    public class HeaderAttribute : NamedAttribute, ICanNullable
    {
        public string Value { get; protected set; }

        public bool CanNull { get; set; }

        public HeaderAttribute() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nameValue">example: token:12232323,token:{token},accessToken</param>
        public HeaderAttribute(string nameValue)
        {
            if (string.IsNullOrEmpty(nameValue))
                throw new ArgumentNullException(nameof(nameValue));
            var index = nameValue.IndexOf(':');
            if (index == 0)
                throw new Exception($"nameValue格式错误，name不能为空");
            if (index == -1)
                Name = nameValue;
            else
            {
                Name = nameValue.Substring(0, index);
                if (nameValue.Length > index + 1)
                    Value = nameValue.Substring(index + 1);
            }
        }


    }
}