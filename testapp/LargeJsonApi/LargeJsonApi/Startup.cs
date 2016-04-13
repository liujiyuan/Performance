// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json.Serialization;
using System;

namespace LargeJsonApi
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvcCore()
                .AddJsonFormatters(json => json.ContractResolver = new CamelCasePropertyNamesContractResolver())
                .AddDataAnnotations();
        }

        public void Configure(IApplicationBuilder app, IApplicationEnvironment applicationEnvironment)
        {
            app.Use(next => async context =>
            {
                try
                {
                    await next(context);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            });

            app.UseMvc();
        }

        public static void Main(string[] args)
        {
            var application = new WebHostBuilder()
                .UseKestrel()
                .UseDefaultHostingConfiguration(args)
                .UseStartup<Startup>()
                .Build();

            application.Run();
        }
    }
}
