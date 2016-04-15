// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Net;
using System.Net.Http;
using System.Text;
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

        private static readonly byte[] ValidBytes = new UTF8Encoding(false).GetBytes("name=Joey&age=15&birthdate=9-9-1985");

        static BasicViewsTest()
        {
            var builder = new WebHostBuilder()
                .UseStartup<BasicViews.Startup>();
            Server = new TestServer(builder);
            Client = Server.CreateClient();
        }

        [Fact]
        public async Task BasicViews_HtmlHelpers()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/");
            request.Content = new ByteArrayContent(ValidBytes);

            var response = await Client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task BasicViews_HtmlHelpers_NoAntiforgery()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/Home/SuppressAntiforgery");
            request.Content = new ByteArrayContent(ValidBytes);

            var response = await Client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task BasicViews_TagHelpers()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/TagHelpers");
            request.Content = new ByteArrayContent(ValidBytes);

            var response = await Client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
