using System;

namespace Sample.Service
{
    /// <summary>
    /// 
    /// </summary>
    public class Person 
    {
        public string Name { get; set; }

        public string Body { get; set; }

        public string Say()
        {
            return "helloword";
        }
    }
}