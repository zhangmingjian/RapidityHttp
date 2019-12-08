using Rapidity.Http.Configurations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rapidity.Http
{
    public interface ICacheOperator
    {
        ResponseWrapperResult Read(HttpRequest request);

        void Write(HttpResponse response, CacheOption option);
    }
}
