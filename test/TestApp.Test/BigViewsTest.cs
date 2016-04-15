// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace MvcBenchmarks.InMemory
{
    public class BigViewsTest
    {
        private static readonly TestServer Server;
        private static readonly HttpClient Client;

        static BigViewsTest()
        {
            var builder = new WebHostBuilder()
                .UseStartup<BigViews.Startup>();
            Server = new TestServer(builder);
            Client = Server.CreateClient();
        }

        [Fact]
        public async Task BigViews_HtmlHelpers()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/Home/Index");

            var response = await Client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task BigViews_TagHelpers()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/Home/IndexWithTagHelpers");

            var response = await Client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task BigViews_TagHelpers_StaticOptions()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/Home/IndexWithStaticOptions");

            var response = await Client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
