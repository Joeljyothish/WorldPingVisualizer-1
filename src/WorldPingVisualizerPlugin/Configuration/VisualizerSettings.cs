using Newtonsoft.Json;
using Terraria.GameContent.Drawing;

namespace WorldPingVisualizerPlugin.Configuration
{
    public class VisualizerSettings
    {
        public class ParticleSettings
        {
            [JsonProperty(
                "enabled"
            )]
            public bool Enabled { get; set; } = true;

            [JsonProperty(
                "particlesIntervalMilliseconds"
            )]
            public int ParticlesIntervalMilliseconds { get; set; } = 1000;

            [JsonProperty(
                "particleType"
            )]
            public ParticleOrchestraType ParticleType { get; set; } = ParticleOrchestraType.StellarTune;
        }

        public class CombatTextSettings
        {
            [JsonProperty(
                "enabled"
            )]
            public bool Enabled { get; set; }

            [JsonProperty(
                "combatTextIntervalMilliseconds"
            )]
            public int CombatTextIntervalMilliseconds { get; set; } = 1000;

            [JsonProperty(
                "combatTextContents"
            )]
            public string CombatTextContents { get; set; } = "PONG!";

            [JsonProperty(
                "combatTextColor"
            )]
            public uint CombatTextColor { get; set; } = 0xFFFF00;
        }

        [JsonProperty(
            "particles"
        )]
        public ParticleSettings Particles { get; set; } = new ParticleSettings();

        [JsonProperty(
            "combatText"
        )]
        public CombatTextSettings CombatText { get; set; } = new CombatTextSettings();
    }
}