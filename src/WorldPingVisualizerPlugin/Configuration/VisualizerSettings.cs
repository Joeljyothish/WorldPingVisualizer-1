using Newtonsoft.Json;
using System.ComponentModel;
using Terraria.GameContent.Drawing;

namespace WorldPingVisualizerPlugin.Configuration
{
    public class VisualizerSettings
    {
        [JsonProperty(
            "particlesIntervalMilliseconds"
        )]
        [DefaultValue(1000)]
        public int ParticlesIntervalMilliseconds { get; set; } = 1000;

        [JsonProperty(
            "particleType"
        )]
        [DefaultValue(ParticleOrchestraType.StellarTune)]
        public ParticleOrchestraType ParticleType { get; set; } = ParticleOrchestraType.StellarTune;
    }
}