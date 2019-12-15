using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using Xunit;

namespace Rapidity.Http.Tests
{
    public class UrlQueryTest
    {
        [Fact]
        public void Test()
        {
            var query = new UriQueryValues();
            query.Add("name", "张三");

            query.Add("id", "1121212");
            query.Add("desc", "feeggrgr");
            query.Add("list", new string[] { "a", "A" });
            query.Add("NAME", "wangwu");

            var uri1 = query.ToString();

            var query2 = new UriQueryValues();
            query2.Add("wd", "aafefefe");
            query2.Add("token", "kjdiejjkl");
            query2.Add(query);
            var uri2 = query2.ToString();

            var uri = "http://www.host.com/api/v2?" + uri2;
            var uriQuery = UriQueryValues.FromUri(uri);
        }

        [Fact]
        public void AddTest()
        {
            var query = new UriQueryValues();
            query.Add("name", "张三");

            query.Add("id", "1121212");
            query.Add("desc", "feeggrgr");
            query.Add("list", new string[] { "a", "A" });
            query.Add("NAME", "wangwu");

            var query2 = new UriQueryValues();
            query2.Add("wd", "aafefefe");
            query2.Add("token", "kjdiejjkl");
            query2.Add(query);

            query.Add(query2);

            var uri = query.ToString();
        }

        [Fact]
        public void ConcatTest()
        {
            var uriString = "http://www.host.com/api/v2?";
            var query = new UriQueryValues();
            query.Add("name", "aafeefe");
            query.Add("query", "1212121");
            var uriUri = query.Concat(uriString);

            Assert.Equal(uriUri, new Uri("http://www.host.com/api/v2?name=aafeefe&query=1212121"));
        }

        [Fact]
        public void Concat2Test()
        {
            var uriString = "/api/v2?k=aaaaa&";
            var query = new UriQueryValues();
            query.Add("name", "aafeefe");
            query.Add("query", "1212121");
            var uriUri = query.Concat(uriString);
        }

        [Fact]
        public void EquestTest()
        {
            var query1 = new UriQueryValues();
            query1.Add("name", "zhangsan");
            query1.Add("id", "11111");
            query1.Add("key", new string[] { "aaaaa", "bbbbbbb" });

            var query2 = new UriQueryValues();
            query2.Add("NAME", "ZHANGSAN");
            query2.Add("ID", "11111");
            query2.Add("KEY", new string[] { "AAAAA", "BBBBBBB" });

            var e1 = query1.Equals(query2);
            var e2 = query2.Equals(query1);

            Assert.True(e1);
            Assert.True(e2);

            var query3 = new UriQueryValues();
            query3.Add("NAME", "zhangsan");
            query3.Add("ID", "11111");

            Assert.False(query2.Equals(query3));


            var query4 = new UriQueryValues();
            query4.Add("NAME", "zhangsan");
            query4.Add("ID", "11111");
            query4.Add("key", new string[] { "AAAAA" });
            Assert.False(query2.Equals(query4));
        }
    }
}
