using System.Collections.Generic;
using Newtonsoft.Json;

namespace CrossgenUtil.Config
{
    /// <summary>
    /// Data structure for config.json file, which contains configurations that are considered more static than
    /// input parameters
    /// 
    /// * KnownRuntimes: known clr runtimes published on nuget
    /// * ToolsDependencies: versions of tools to be used running crossgen
    /// </summary>
    public class CrossgenUtilConfig
    {

        [JsonProperty(PropertyName = "nuget-known-runtimes")]
        public List<RuntimeInfo> KnownRuntimes { get; set; }

        [JsonProperty(PropertyName = "tools-dependencies")]
        public DependenciesDetails ToolsDependencies { get; set; }
    }
}
