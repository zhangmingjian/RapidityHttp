using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Sample.Service;
using System;

namespace Sample.Http.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("started===============");
            var provider = Startup.ConfigureServices(null);
            var tokenService = provider.GetService<ITokenService>();
            var token = tokenService.GetToken("wx431ac6b88d367ede", "7fc5f5411e0127f21ff413c521736044").GetAwaiter().GetResult();
            Console.WriteLine("access_token:{0}", token.access_token);
            var userService = provider.GetService<IUserService>();
            var userList = userService.GetUserList(token.access_token);
            Console.WriteLine("userList:{0}", JsonConvert.SerializeObject(userList));

            var userInfo = userService.GetUserInfo(token.access_token, userList.NextOpenid, "中文").GetAwaiter().GetResult();
            Console.WriteLine("userInfo:{0}", JsonConvert.SerializeObject(userInfo));

            Console.ReadKey();
        }
    }
}
