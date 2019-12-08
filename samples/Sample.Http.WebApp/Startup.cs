using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rapidity.Http;
using Rapidity.Http.Extensions;
using Sample.Service;
using System;

namespace Sample.Http.WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRapidityHttp().AddHttpService("wechat", config =>
            {
                config.BaseAddress = "https://api.weixin.qq.com/";
                config.Timeout = 60;
                config.Option.ContentType = "application/json";
                config.Option.DefaultHeaders.Add("customHeader", "fromtest");
                config.AddModule("GetUserList", httpOption =>
                {
                    httpOption.CacheOption = new Rapidity.Http.Configurations.CacheOption
                    {
                        Enabled = true,
                        ExpireIn = TimeSpan.FromSeconds(10),
                        ExpireType = Rapidity.Http.Configurations.ExpireType.Sliding
                    };
                });
            }).For<ITokenService>().For<IUserService>();
            services.ConfigDefaultRecordStore().BuildProxy();

            services.AddMemoryCache();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvcWithDefaultRoute();
        }
    }
}
