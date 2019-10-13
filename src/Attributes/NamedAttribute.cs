using System;

namespace Rapidity.Http.Attributes
{
    /// <summary>
    /// 属性，参数别名
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    public class NamedAttribute : Attribute
    {
        public string Name { get; set; }

        protected NamedAttribute() { }

        public NamedAttribute(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            this.Name = name;
        }
    }
}