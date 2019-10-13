using System;

namespace Rapidity.Http.Attributes
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class BodyAttribute : Attribute, ICanNullable
    {
        public bool CanNull { get; set; } = true;
    }
}