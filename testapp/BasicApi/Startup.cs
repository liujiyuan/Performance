// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using BasicApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;

namespace BasicApi
{
    public class Startup
    {
        public Startup(IHostingEnvironment hosting)
        {
            Configuration =
                new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .SetBasePath(PlatformServices.Default.Application.ApplicationBasePath)
                .Build();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var rsa = new RSACryptoServiceProvider();
            var key = new RsaSecurityKey(rsa.ExportParameters(true));

            services.AddSingleton(new SigningCredentials(
                key,
                SecurityAlgorithms.RsaSha256Signature));

            services.Configure<JwtBearerOptions>(options =>
            {
                options.TokenValidationParameters.IssuerSigningKey = key;
                options.TokenValidationParameters.ValidAudience = "Myself";
                options.TokenValidationParameters.ValidIssuer = "BasicApi";
            });

            services.AddEntityFrameworkSqlServer().AddDbContext<BasicApiContext>(options =>
            {
                var connectionString = Configuration["Data:DefaultConnection:ConnectionString"];
                options.UseSqlServer(connectionString);
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    "pet-store-reader", 
                    builder => builder
                        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme‌​)
                        .RequireAuthenticatedUser()
                        .RequireClaim("scope", "pet-store-reader"));

                options.AddPolicy(
                    "pet-store-writer",
                    builder => builder
                        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme‌​)
                        .RequireAuthenticatedUser()
                        .RequireClaim("scope","pet-store-writer"));
            });

            services
                .AddMvcCore()
                .AddAuthorization()
                .AddJsonFormatters(json => json.ContractResolver = new CamelCasePropertyNamesContractResolver())
                .AddDataAnnotations();
            
            services.AddSingleton<PetRepository>(new PetRepository());
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(LogLevel.Trace);

            CreateDatabase(app.ApplicationServices);

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

            app.UseIdentity();

            app.UseJwtBearerAuthentication();

            app.UseMvc();
        }

        private void CreateDatabase(IServiceProvider services)
        {
            using (var serviceScope = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var dbContext = services.GetRequiredService<BasicApiContext>();
                dbContext.Database.EnsureDeleted();
                dbContext.Database.EnsureCreated();

                var categories = new List<Category>()
                {
                    new Category() { Name = "Dogs", },
                    new Category() { Name = "Cats", },
                    new Category() { Name = "Rabbits", },
                    new Category() { Name = "Lions", },
                };

                var tags = new List<Tag>()
                {
                    new Tag() { Name = "tag1", },
                    new Tag() { Name = "tag2", },
                };

                var pets = new List<Pet>()
                {
                    new Pet() { Category = categories[0], Name = "Cat 1", Status = "available", Tags = tags, },
                    new Pet() { Category = categories[0], Name = "Cat 2", Status = "available", Tags = tags, },
                    new Pet() { Category = categories[0], Name = "Cat 3", Status = "available", Tags = tags, },
                    new Pet() { Category = categories[1], Name = "Dog 1", Status = "available", Tags = tags, },
                    new Pet() { Category = categories[1], Name = "Dog 2", Status = "available", Tags = tags, },
                    new Pet() { Category = categories[1], Name = "Dog 3", Status = "available", Tags = tags, },
                    new Pet() { Category = categories[2], Name = "Rabbit 1", Status = "available", Tags = tags, },
                    new Pet() { Category = categories[2], Name = "Rabbit 1", Status = "available", Tags = tags, },
                    new Pet() { Category = categories[2], Name = "Rabbit 1", Status = "available", Tags = tags, },
                    new Pet() { Category = categories[3], Name = "Lion 1", Status = "available", Tags = tags, },
                    new Pet() { Category = categories[3], Name = "Lion 1", Status = "available", Tags = tags, },
                    new Pet() { Category = categories[3], Name = "Lion 1", Status = "available", Tags = tags, },
                };

                var images = new List<Image>();

                foreach (var pet in pets)
                {
                    pet.Images = new List<Image>();
                    pet.Images.Add(new Image() { Url = $"http://example.com/pets/{pet.Name}_1.png" });
                    pet.Images.Add(new Image() { Url = $"http://example.com/pets/{pet.Name}_2.png" });

                    images.AddRange(pet.Images);
                }

                foreach (var pet in pets)
                {
                    dbContext.Pets.Add(pet);
                }

                dbContext.SaveChanges();
            }
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
