using System;

namespace Rapidity.Http
{
    /// <summary>
    /// 
    /// </summary>
    public interface IUriGenerator
    {
        Uri GetUri(RequestDescription description);
    }
}