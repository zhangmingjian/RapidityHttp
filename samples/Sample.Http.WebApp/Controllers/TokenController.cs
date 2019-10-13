using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Sample.Service;

namespace Sample.Http.WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : Controller
    {
        private readonly ITokenService _tokenService;
        private readonly IMemoryCache _cache;

        public TokenController(ITokenService tokenService, IMemoryCache cache)
        {
            _tokenService = tokenService;
            _cache = cache;
        }

        // GET api/values
        [HttpGet]
        public async Task<AccessToken> Get()
        {
            var appid = "wx431ac6b88d367ede";
            var secret = "7fc5f5411e0127f21ff413c521736044";
            var token = await _tokenService.GetToken(appid, secret);
            _cache.Set("token", token);
            return token;
        }

        [HttpGet("/GetIpList")]
        public async Task<IpList> GetIpList()
        {
            _cache.TryGetValue("token", out AccessToken token);
            //var token = "26_v5Otju59BnoGK3Xt2t96bdhYyzf2sSmMaZHy0cWcQHnq4r8bbtovSmdTYcLBB6eqzavBH9MuLUp4v5EXb9uEecbikimIgfG39DStw5xMs_hNCH9PpmFiCpi0LpaQbaSrvZfEmYnnUaCj_NGQAKZeAEAUET";
            return await _tokenService.GetIpListAsync(token?.access_token);
        }

        // POST api/values
        [HttpPost("/SendTemplate")]
        public async Task<ActionResult> SendTemplate()
        {
            _cache.TryGetValue("token", out AccessToken token);
            var input = new MessageTemplate
            {
                touser = "oidQ2tw0GHI6A5uBo-oU1m2ujZ4E",
                template_id = "H-car-_H02KWg1dvThAslgbPq2VjEyFtvnNSX72Tqzw",
                url = "https://developers.weixin.qq.com/doc",
                data = new MessageTemplate.Data()
            };
            await _tokenService.SendTemplateMsg(token.access_token, input);
            return Json("ok");
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {

        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
