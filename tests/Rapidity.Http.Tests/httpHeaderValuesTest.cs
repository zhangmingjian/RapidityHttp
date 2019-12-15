using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Rapidity.Http.Tests
{
    public class httpHeaderValuesTest
    {
        [Fact]
        public void ToStringTest()
        {
            var header = new HttpHeaderValues();
            header.Add("Accept", "image/webp,image/apng,image/*,*/*");
            header.Add("Accept", "q=0.8");
            header.Add("Accept-Encoding", "gzip, deflate, br");
            header.Add("Accept-Language", "zh-CN,zh;q=0.9,en");
            header.Add("Accept-Language", "q=0.8");

            header.Add("Content-Type", "text/plain");
            header.Add("Content-Type", "charset=UTF-8");

            header.Add("Cookie", "BAIDUID=2A3EF7F2C89B57F81E4E2D4116BA4FAB:FG=1");
            header.Add("Cookie", "BIDUPSID=911317338F66E8EDF90E147D2EB08518");
            header.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/78.0.3904.87 Safari/537.36");

            var content = header.ToString();
        }

        [Fact]
        public void AddTest()
        {
            var header = new HttpHeaderValues();
            header.Add("Accept", "image/webp,image/apng,image/*,*/*");
            header.Add("Accept", "q=0.8");
            header.Add("Accept-Encoding", "gzip, deflate, br");
            header.Add("Accept-Language", "zh-CN,zh;q=0.9,en");
            header.Add("Accept-Language", "q=0.8");

            header.Add("Content-Type", "text/plain");
            header.Add("Content-Type", "charset=UTF-8");

            var header2 = new HttpHeaderValues("token", Guid.NewGuid().ToString("n"));

            header.Add(header2);

            var headerContent = header.ToString();
        }
    }
}
