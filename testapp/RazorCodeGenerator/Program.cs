// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Directives;
using Microsoft.AspNetCore.Razor;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.Extensions.FileProviders;

namespace RazorCodeGenerator
{
    public class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Usage();
                return -1;
            }

            var files = new List<string>();
            var iterations = 100;
            var dump = false;

            foreach (var arg in args)
            {
                if (arg == "--dump")
                {
                    dump = true;
                    break;
                }

                int parsed;
                if (int.TryParse(arg, out parsed))
                {
                    iterations = parsed;
                    continue;
                }

                files.Add(arg);
            }

            if (files.Count == 0)
            {
                Usage();
                return -2;
            }

            for (var i = 0; i < files.Count; i++)
            {
                files[i] = Path.GetFullPath(files[i]);
            }

            var basePath = Directory.GetCurrentDirectory();

            Console.WriteLine("Press the ANY key to start.");
            Console.ReadLine();

            GenerateCodeFile(basePath, files.ToArray(), iterations, dump);

            Console.WriteLine("Press the ANY key to exit.");
            Console.ReadLine();
            return 0;
        }

        private static void Usage()
        {
            Console.WriteLine("usage: dotnet run <file1.cshtml> <file2.cshtml> <iterations = 100> <--dump?>");
        }

        private static void GenerateCodeFile(string basePath, string[] files, int iterations, bool dump)
        {
            var codeLang = new CSharpRazorCodeLanguage();

            var host = new MvcRazorHost(
                new DefaultChunkTreeCache(new PhysicalFileProvider(basePath)),
                new TagHelperDescriptorResolver(new TagHelperTypeResolver(), new TagHelperDescriptorFactory(designTime: false)));
            var engine = new RazorTemplateEngine(host);

            Console.WriteLine($"Warm Starting Code Generation: {string.Join(", ", files)}");
            var timer = Stopwatch.StartNew();

            for (var i = 0; i < iterations; i++)
            {
                for (var j = 0; j < files.Length; j++)
                {
                    var file = files[j];
                    var fileName = Path.GetFileName(file);
                    var fileNameNoExtension = Path.GetFileNameWithoutExtension(fileName);

                    using (var fileStream = File.OpenText(file))
                    {
                        var code = engine.GenerateCode(
                            input: fileStream,
                            className: fileNameNoExtension,
                            rootNamespace: "Test",
                            sourceFileName: fileName);

                        if (dump)
                        {
                            File.WriteAllText(Path.ChangeExtension(file, ".cs"), code.GeneratedCode);
                        }
                    }
                }
                Console.WriteLine("Completed iteration: " + (i + 1));
            }

            Console.WriteLine($"Completed after {timer.Elapsed}");
        }
    }
}