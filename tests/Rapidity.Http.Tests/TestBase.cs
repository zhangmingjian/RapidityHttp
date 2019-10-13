using System;
using Microsoft.Extensions.DependencyInjection;

namespace Rapidity.Http.Tests
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class TestBase
    {
        protected IServiceProvider ServiceProvider => ConfigService(null);

        protected IServiceProvider ConfigService(IServiceCollection services)
        {
            if (services == null) services = new ServiceCollection();
            return Startup.ConfigureServices(services);
        }
    }
}