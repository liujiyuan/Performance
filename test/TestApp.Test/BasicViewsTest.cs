// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Net;
using System.Net.Http;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace MvcBenchmarks.InMemory
{
    public class BasicViewsTest
    {
        private static readonly TestServer Server;
        private static readonly HttpClient Client;

        static BasicViewsTest()
        {
            var builder = new WebHostBuilder();
            builder.UseStartup<BasicViews.Startup>();
            builder.UseProjectOf<BasicViews.Startup>();
            Server = new TestServer(builder);
            Client = Server.CreateClient();
        }
        
        private async Task<string[]> GetAntiforgeryToken(string requestUri)
        {
            var result = new string[]
            {
                string.Empty, string.Empty
            };
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
                var response = await Client.SendAsync(request);
                foreach (var item in response.Headers.GetValues("Set-Cookie"))
                {
                    result[0] = item.Substring(0, item.IndexOf(';'));
                    break;
                }
                var content = await response.Content.ReadAsStringAsync();
                var reader = new StringReader(content);
                var line = reader.ReadLine()?.TrimStart();
                while(line != null)
                {
                    if(line.StartsWith(@"<input name=""__RequestVerificationToken"))
                    {
                        var start = line.IndexOf(@"value=""");
                        if(start == -1) continue;
                        start += @"value=""".Length;
                        var end = line.LastIndexOf(@"""");
                        result[1] = line.Substring(start, end - start);
                        break;
                    }
                    line = reader.ReadLine()?.TrimStart();
                }
            }
            catch
            {
            }
            return result;
        }
        
        private byte[] GetValidBytes(string antiforgeryToken = null)
        {
            var message = "name=Joey&age=15&birthdate=9-9-1985";
            if(!string.IsNullOrEmpty(antiforgeryToken))
            {
                message += "&__RequestVerificationToken=" + antiforgeryToken;
            }
            return new UTF8Encoding(false).GetBytes(message);
        }

        [Fact]
        public async Task BasicViews_HtmlHelpers()
        {
            var antiforgeryToken = await GetAntiforgeryToken("/Home/HtmlHelpers");
            var request = new HttpRequestMessage(HttpMethod.Post, "/Home/HtmlHelpers");
            request.Headers.Add("Cookie", new [] {antiforgeryToken[0]});
            request.Content = new ByteArrayContent(GetValidBytes(antiforgeryToken[1]));
            request.Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

            var response = await Client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task BasicViews_TagHelpers()
        {
            var antiforgeryToken = await GetAntiforgeryToken("/");
            var request = new HttpRequestMessage(HttpMethod.Post, "/");
            request.Headers.Add("Cookie", new [] {antiforgeryToken[0]});
            request.Content = new ByteArrayContent(GetValidBytes(antiforgeryToken[1]));
            request.Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

            var response = await Client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
