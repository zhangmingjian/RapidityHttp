using System;

namespace Rapidity.Http.Attributes
{
    /// <summary>
    /// 表示这个是一个UriQuery参数
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    public class QueryAttribute : NamedAttribute, ICanNullable
    {

        public QueryAttribute() { }

        public QueryAttribute(string name)
        {
            this.Name = name;
        }

        public bool CanNull { get; set; }
    }
}