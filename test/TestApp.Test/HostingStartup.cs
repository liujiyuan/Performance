// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO;
using System.Reflection;
using System.Runtime.Versioning;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;

namespace MvcBenchmarks.InMemory
{
    public static class HostingStartup
    {
        public static string GetProjectDirectoryOf<TStartup>()
        {
            var libraryManager = DnxPlatformServices.Default.LibraryManager;

            var applicationName = typeof(TStartup).GetTypeInfo().Assembly.GetName().Name;
            var library = libraryManager.GetLibrary(applicationName);
            return Path.GetDirectoryName(library.Path);
        }

        public static WebHostBuilder UseProjectOf<TStartup>(this WebHostBuilder builder)
        {
            var applicationName = typeof(TStartup).GetTypeInfo().Assembly.GetName().Name;
            var webRoot = GetProjectDirectoryOf<TStartup>();

            var assemblyProvider = new StaticAssemblyProvider();
            assemblyProvider.CandidateAssemblies.Add(typeof(TStartup).Assembly);
            builder.ConfigureServices(services =>
            {
                var applicationEnvironment = new TestApplicationEnvironment(
                    PlatformServices.Default.Application,
                    applicationName,
                    webRoot);
                services.AddSingleton<IApplicationEnvironment>(applicationEnvironment);

                var hostingEnvironment = new HostingEnvironment();
                hostingEnvironment.Initialize(
                    applicationName,
                    webRoot,
                    new WebHostOptions
                    {
                        Environment = "Production",
                    });
                services.AddSingleton<IHostingEnvironment>(hostingEnvironment);

                services.AddSingleton<IAssemblyProvider>(assemblyProvider);
            });

            return builder;
        }

        private class TestApplicationEnvironment : IApplicationEnvironment
        {
            private readonly IApplicationEnvironment _original;

            public TestApplicationEnvironment(IApplicationEnvironment original, string name, string path)
            {
                _original = original;

                ApplicationName = name;
                ApplicationBasePath = path;
            }

            public string ApplicationBasePath { get; }

            public string ApplicationName { get; }

            public string ApplicationVersion => _original.ApplicationVersion;

            public FrameworkName RuntimeFramework => _original.RuntimeFramework;
        }
    }
}
