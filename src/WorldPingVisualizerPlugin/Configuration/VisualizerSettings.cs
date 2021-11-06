using Newtonsoft.Json;
using System.ComponentModel;

namespace WorldPingVisualizerPlugin.Configuration
{
    public class VisualizerSettings
    {
        [JsonProperty(
            "particlesIntervalMilliseconds"
        )]
        [DefaultValue(1000)]
        public int ParticlesIntervalMilliseconds { get; set; } = 1000;
    }
}