using Newtonsoft.Json;
using System.ComponentModel;
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
            [DefaultValue(true)]
            public bool Enabled { get; set; } = true;

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

        public class CombatTextSettings
        {
            [JsonProperty(
                "enabled"
            )]
            public bool Enabled { get; set; }

            [JsonProperty(
                "combatTextIntervalMilliseconds"
            )]
            [DefaultValue(1000)]
            public int CombatTextIntervalMilliseconds { get; set; } = 1000;

            [JsonProperty(
                "combatTextContents"
            )]
            [DefaultValue("PONG!")]
            public string CombatTextContents { get; set; } = "PONG!";

            [JsonProperty(
                "combatTextColor"
            )]
            [DefaultValue(0xFFF00)]
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