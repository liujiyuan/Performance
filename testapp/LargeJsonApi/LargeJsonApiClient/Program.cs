// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.Net.Http;
using System.Text;
using LargeJsonApi.Data;
using Microsoft.Extensions.CommandLineUtils;

namespace LargeJsonApiClient
{
    public class Program
    {
        private const string DefaultServerUri = "http://localhost:5000/";
        private const int DefaultIterationCount = 100;

        public static int Main(string[] args)
        {
            var app = new CommandLineApplication
            {
                Name = "LargeJsonApiClient",
                FullName = "Large JSON Api Client",
                Description = "Client application to test performance costs of MVC APIs",
            };
            app.HelpOption("-h|--help");
            var serverUriOption = app.Option("--server|-s", $"The URI of the server to target, defaults to {DefaultServerUri}", CommandOptionType.SingleValue);
            var runGetOption = app.Option("--get|-g", "Indicates the program to run the GET test, you must specify at least one of GET or POST", CommandOptionType.NoValue);
            var runPostOption = app.Option("--post|-p", "Indicates the program to run the POST test, you must specify at least one of GET or POST", CommandOptionType.NoValue);
            var iterationCountOption = app.Option("--iterations|-i", "The iteration count for the test, defaults to {DefaultIterationCount}, must be an int", CommandOptionType.SingleValue);

            app.OnExecute(() =>
            {
                var serverUri = DefaultServerUri;
                if (serverUriOption.HasValue())
                {
                    var serverUriString = serverUriOption.Value();
                    if (serverUriString == null)
                    {
                        app.ShowHelp();
                        return 2;
                    }
                    serverUri = serverUriString;
                }

                var runGet = runGetOption.HasValue();
                var runPost = runPostOption.HasValue();
                if (!(runGet | runPost))
                {
                    app.ShowHelp();
                    return 3;
                }

                var iterationCount = DefaultIterationCount;
                if (iterationCountOption.HasValue())
                {
                    if (!int.TryParse(iterationCountOption.Value(), out iterationCount))
                    {
                        app.ShowHelp();
                        return 4;
                    }
                }

                PrintLine($"Targetting { serverUri }");
                PrintLine("Start test...");
                try
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(serverUri);

                        for (var i = 1; i <= iterationCount; ++i)
                        {
                            PrintLine($"Iteration { i }");

                            if (runGet)
                            {
                                RunGetScenario(client);
                            }
                            if (runPost)
                            {
                                RunPostScenario(client);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.Message);
                    Console.Error.WriteLine(e.StackTrace);
                }
                PrintLine("Done.");

                return 0;
            });

            return app.Execute(args);
        }

        public static void RunGetScenario(HttpClient client)
        {
            for (var i = 0; i < DataFactory.MovieStrings.Length; i++)
            {
                PrintLine($"Get movie {i}");
                var result = client.GetAsync($"/popcorn/movie/{i}").Result;
                result.EnsureSuccessStatusCode();
            }
            for (var i = 0; i < DataFactory.SeriesStrings.Length; i++)
            {
                PrintLine($"Get series {i}");
                var result = client.GetAsync($"/popcorn/series/{i}").Result;
                result.EnsureSuccessStatusCode();
            }
            for (var i = 0; i < DataFactory.SeasonStrings.Length; i++)
            {
                PrintLine($"Get season {i}");
                var result = client.GetAsync($"/popcorn/season/{i}").Result;
                result.EnsureSuccessStatusCode();
            }
        }

        public static void RunPostScenario(HttpClient client)
        {
            for (var i = 0; i < DataFactory.MovieStrings.Length; i++)
            {
                PrintLine($"Post movie {i}");
                var result = client.PostAsync("/popcorn/movie/0", new StringContent(DataFactory.MovieStrings[i], Encoding.UTF8, "application/json")).Result;
                result.EnsureSuccessStatusCode();
            }
            for (var i = 0; i < DataFactory.SeriesStrings.Length; i++)
            {
                PrintLine($"Post series {i}");
                var result = client.PostAsync("/popcorn/series/0", new StringContent(DataFactory.SeriesStrings[i], Encoding.UTF8, "application/json")).Result;
                result.EnsureSuccessStatusCode();
            }
            for (var i = 0; i < DataFactory.SeasonStrings.Length; i++)
            {
                PrintLine($"Post season {i}");
                var result = client.PostAsync("/popcorn/season/0", new StringContent(DataFactory.SeasonStrings[i], Encoding.UTF8, "application/json")).Result;
                result.EnsureSuccessStatusCode();
            }
        }

        private static void PrintLine(string input, params object[] paramStrings)
        {
            var original = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"[{ DateTime.UtcNow.ToString(CultureInfo.InvariantCulture) }] ");
            Console.ForegroundColor = original;
            Console.WriteLine(input, paramStrings);
        }
    }
}
