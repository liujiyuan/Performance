// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace MvcBenchmarks.InMemory
{
    public class BigModelBindingTest
    {
        private static readonly TestServer Server;
        private static readonly HttpClient Client;
        private static readonly HttpContent Content;

        private static readonly byte[] ValidBytes = new UTF8Encoding(false).GetBytes(@"
{
  ""category"" : {
    ""id"" : 2,
    ""name"" : ""Cats""
  },
  ""name"" : ""fluffy"",
  ""status"" : ""available""
}");

        static BigModelBindingTest()
        {
            var builder = new WebHostBuilder()
                .UseStartup<BigModelBinding.Startup>();
            Server = new TestServer(builder);
            Client = Server.CreateClient();

            var inputFile = Path.Combine(HostingStartup.GetProjectDirectoryOf<BigModelBinding.Startup>(), "postdata.txt");
            var input = File.ReadAllText(inputFile);
            Content = new StringContent(input, Encoding.UTF8, "application/x-www-form-urlencoded");
        }

        [Fact]
        public async Task BigModelBinding()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/");
            request.Content = Content;

            var response = await Client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
