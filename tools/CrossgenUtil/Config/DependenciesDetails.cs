using Newtonsoft.Json;

namespace CrossgenUtil.Config
{
    public class DependenciesDetails
    {
        [JsonProperty(PropertyName = "coreclr")]
        public string CoreClrVersion { get; set; }

        [JsonProperty(PropertyName = "corejit")]
        public string CoreJitVersion { get; set; }

        [JsonProperty(PropertyName = "diasymreader")]
        public string DiaSymReaderVersion { get; set; }
    }
}
