// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using BasicApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;

namespace BasicApi
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvcCore()
                .AddJsonFormatters(json => json.ContractResolver = new CamelCasePropertyNamesContractResolver())
                .AddDataAnnotations();

            services.AddSingleton<PetRepository>(new PetRepository());
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMvc();
        }

        public static void Main(string[] args)
        {
            var application = new WebHostBuilder()
                .UseServer("Microsoft.AspNetCore.Server.Kestrel")
                .UseUrls("http://+:5000")
                .UseDefaultConfiguration(args)
                .UseStartup<Startup>()
                .Build();

            application.Run();
        }
    }
}
