using System;
using Newtonsoft.Json;

namespace CrossgenUtil
{
    /// <summary>
    /// Parsing/processing runtime moniker
    /// </summary>
    public class RuntimeInfo
    {
        public string OS { get; }
        public string Version { get; }
        public string Arch { get; }
        public string Moniker
        {
            get
            {
                if (OS == "win")
                {
                    return $"{OS}{Version}-{Arch}";
                }
                else
                {
                    return $"{OS}.{Version}-{Arch}";
                }
            }
        }

        public static RuntimeInfo Parse(string moniker)
        {
            int archNameCutoff = moniker.LastIndexOf("-");
            bool hasArchName = archNameCutoff > 0;
            if (!hasArchName)
            {
                archNameCutoff = moniker.Length;
            }
            int osNameCutoff = moniker.IndexOf(".");
            string osName;
            string osVersion;
            if (osNameCutoff < 0)
            {
                if (moniker.StartsWith("win"))
                {
                    osName = "win";
                    osVersion = moniker.Substring(3, archNameCutoff - 3);
                }
                else
                {
                    // Not versioned?
                    osName = moniker.Substring(0, archNameCutoff);
                    osVersion = string.Empty;
                }
            }
            else
            {
                osName = moniker.Substring(0, osNameCutoff);
                osVersion = moniker.Substring(osNameCutoff + 1, archNameCutoff - osNameCutoff - 1);
            }

            var arch = hasArchName ? moniker.Substring(archNameCutoff + 1) : string.Empty;
            return new RuntimeInfo(osName, osVersion, arch);
        }

        private RuntimeInfo(string os, string version, string arch)
        {
            OS = os;
            Version = version;
            Arch = arch;
        }

        public bool IsArch(string arch)
        {
            return string.IsNullOrEmpty(Arch) || Arch == arch;
        }

        public bool IsOS(string osName)
        {
            return osName.ToLower().StartsWith(OS.ToLower());
        }

        public bool IsVersionAboveOrEqual(string version)
        {
            // non-versioned moniker
            if (string.IsNullOrEmpty(version))
            {
                return true;
            }

            var inputTokens = version.Split('.');
            var localTokens = Version.Split('.');

            for (int i = 0, numTokens = Math.Min(localTokens.Length, inputTokens.Length);
                i < localTokens.Length;
                i++)
            {
                var input = int.Parse(inputTokens[i]);
                var local = int.Parse(localTokens[i]);
                if (input < local)
                {
                    return false;
                }

                if (input > local)
                {
                    return true;
                }
            }

            return true;
        }

        public override string ToString()
        {
            return Moniker;
        }

        public class Converter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(RuntimeInfo);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                return Parse((string)reader.Value);
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotSupportedException();
            }
        }
    }
}
