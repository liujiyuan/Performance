using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using CrossgenUtil.Config;
using Microsoft.DotNet.InternalAbstractions;
using Microsoft.DotNet.ProjectModel.Resolution;
using Microsoft.Extensions.CommandLineUtils;
using Newtonsoft.Json;

namespace CrossgenUtil
{
    /// <summary>
    /// This application is used to crossgen a published application
    /// </summary>
    public class Program
    {
        private static CrossgenUtilConfig Config;
        private static readonly string ProgramHome;

        static Program()
        {
            ProgramHome = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var knowRuntimesAsString = File.ReadAllText(Path.Combine(ProgramHome, "config.json"));
            Config = JsonConvert.DeserializeObject<CrossgenUtilConfig>(knowRuntimesAsString, new RuntimeInfo.Converter());
        }

        public static void Main(string[] args)
        {
            var app = new CommandLineApplication();
            app.Name = "CrossGenHelper";
            app.HelpOption("-?|-h|--help");
            //var runtimeOpt = app.Option(
            //    "--runtime",
            //    "Target runtime to crossgen (optional; default: current runtime) -- Currently this flag is not supported",
            //    CommandOptionType.SingleValue);

            var appDirOpt = app.Option(
                "--app-dir",
                "Location of where the app should be crossgen'd (optional; default: current working directory)",
                CommandOptionType.SingleValue);

            var excludesOpt = app.Option(
                "--excludes",
                "Modules to exclude during crossgen (optional; default: no exclusion)",
                CommandOptionType.MultipleValue);

            var symbolsOpt = app.Option(
                "--symbols",
                "Generate symbols for native images (currently only works on Windows)",
                CommandOptionType.NoValue);

            app.OnExecute(async () =>
            {
                var hostRuntime = DetectRuntimeIdentifier();
                var hostRuntimeMoniker = hostRuntime.Moniker;
                // var runtime = runtimeOpt.HasValue() ? runtimeOpt.Value() : hostRuntimeMoniker;
                var appDir = appDirOpt.HasValue() ? appDirOpt.Value() : Directory.GetCurrentDirectory();
                var excludes = excludesOpt.Values;
                var symbols = symbolsOpt.HasValue();

                if (symbols && RuntimeEnvironment.OperatingSystemPlatform != Platform.Windows)
                {
                    Console.WriteLine("Symbols can only be generated on Windows");
                    return -1;
                }

                var toolsDirPath = Path.Combine(ProgramHome, ".tools");

                if (!Directory.Exists(toolsDirPath))
                {
                    Directory.CreateDirectory(toolsDirPath);
                }

                var runtimePkgName = $"runtime.{hostRuntime}.Microsoft.NETCore.Runtime.CoreCLR";
                var clrjitPkgName = $"runtime.{hostRuntime}.Microsoft.NETCore.Jit";

                var coreclrPkgLocation = await PreparePackage(runtimePkgName, Config.ToolsDependencies.CoreClrVersion, toolsDirPath);
                var clrjitPkgLocation = await PreparePackage(clrjitPkgName, Config.ToolsDependencies.CoreJitVersion, toolsDirPath);

                var clrtoolsDir = Path.Combine(coreclrPkgLocation, "tools");
                var clrjitLibRoot = Path.Combine(clrjitPkgLocation, "runtimes");
                var clrjitLibDir = Path.Combine(clrjitLibRoot, FindDirectoryByRuntime(clrjitLibRoot, hostRuntime), "native");

                if (!Directory.Exists(clrjitLibDir))
                {
                    Console.WriteLine($"Cannot locate clrjit library directory {clrjitLibDir}, this likely means we cannot crossgen the target runtime {hostRuntime} from a {hostRuntime} host");
                    return -1;
                }

                foreach (var file in Directory.GetFiles(clrtoolsDir).Concat(Directory.GetFiles(clrjitLibDir)))
                {
                    File.Copy(file, Path.Combine(toolsDirPath, Path.GetFileName(file)), true);
                }

                if (symbols)
                {
                    var diaSymReaderPkgName = "Microsoft.DiaSymReader.Native";
                    var diaSymReaderPkgLocation = await PreparePackage(diaSymReaderPkgName, Config.ToolsDependencies.DiaSymReaderVersion, toolsDirPath);
                    var diaSymReaderRoot = Path.Combine(diaSymReaderPkgLocation, "runtimes");
                    var diaSymReaderDir = Path.Combine(diaSymReaderRoot, FindDirectoryByRuntime(diaSymReaderRoot, hostRuntime), "native");
                    if (!Directory.Exists(diaSymReaderDir))
                    {
                        Console.WriteLine($"Cannot locate diaSymReader library directory {diaSymReaderDir}");
                        return -1;
                    }

                    var diaSymReaderDlls = Directory.GetFiles(diaSymReaderDir);

                    if (diaSymReaderDlls.Count() != 1)
                    {
                        throw new Exception($"Only 1 file is expected in the diasymreader directory {diaSymReaderDir}");
                    }

                    File.Copy(diaSymReaderDlls.First(), Path.Combine(toolsDirPath, "diasymreader.dll"), true);
                }

                var crossgenPath = Path.Combine(toolsDirPath, "crossgen");
                var sharedFrameworkPath = Path.GetDirectoryName(Assembly.Load(new AssemblyName("mscorlib")).Location);

                var crossgenMgr = new CrossgenManager(crossgenPath, sharedFrameworkPath, appDir, excludes);
                crossgenMgr.RunCrossgen(symbols);
                
                return 0;
            });

            app.Execute(args);
        }

        static string FindDirectoryByRuntime(string location, RuntimeInfo runtime)
        {
            RuntimeInfo matched = null;
            string matchedDir = null;
            foreach (var dir in Directory.EnumerateDirectories(location))
            {
                var dirname = Path.GetFileName(dir);
                var candidate = RuntimeInfo.Parse(dirname);

                if (runtime.IsOS(candidate.OS) &&
                    runtime.IsArch(candidate.Arch) &&
                    runtime.IsVersionAboveOrEqual(candidate.Version))
                {
                    if (matched == null || candidate.IsVersionAboveOrEqual(matched.Version))
                    {
                        matched = candidate;
                        matchedDir = dirname;
                    }
                }
            }
            return matchedDir;
        }

        /// <summary>
        /// return the path of the package expected.
        /// If the package is not located in the cache directory, the app will download the package
        /// </summary>
        static async Task<string> PreparePackage(string pkgName, string version, string toolsDir)
        {
            var nugetCacheDir = PackageDependencyProvider.ResolvePackagesPath(null, null);
            var cachePkgDir = Path.Combine(nugetCacheDir, pkgName, version);
            var pgkMoniker = $"{pkgName}.{version}";
            var toolsPkgDir = Path.Combine(toolsDir, pgkMoniker);

            if (!Directory.Exists(toolsPkgDir))
            {
                if (Directory.Exists(cachePkgDir))
                {
                    return cachePkgDir;
                }
                else
                {
                    var saveFile = $"{pgkMoniker}.nupkg";
                    var saveDir = Path.Combine(toolsDir, "downloads");
                    if (!Directory.Exists(saveDir))
                    {
                        Directory.CreateDirectory(saveDir);
                    }
                    var saveLocation = Path.Combine(saveDir, saveFile);
                    using (var client = new HttpClient())
                    {
                        var downloadUri = $"https://api.nuget.org/packages/{saveFile}".ToLower();
                        var source = await client.GetStreamAsync(downloadUri);
                        using (var output = File.Create(saveLocation))
                        {
                            await source.CopyToAsync(output);
                        }
                    }
                    ZipFile.ExtractToDirectory(saveLocation, toolsPkgDir);
                }
            }

            return toolsPkgDir;
        }

        static RuntimeInfo DetectRuntimeIdentifier()
        {
            var current = RuntimeEnvironment.OperatingSystem;

            var result = Config.KnownRuntimes.FirstOrDefault(runtime =>
                runtime.IsOS(RuntimeEnvironment.OperatingSystem) &&
                runtime.IsArch(RuntimeEnvironment.RuntimeArchitecture) &&
                runtime.IsVersionAboveOrEqual(RuntimeEnvironment.OperatingSystemVersion)
            );

            if (result == null)
            {
                throw new Exception($"Cannot determine runtime packages for environment: {RuntimeEnvironment.GetRuntimeIdentifier()}");
            }

            return result;
        }
    }
}
