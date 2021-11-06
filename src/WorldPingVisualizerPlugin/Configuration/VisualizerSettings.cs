using Newtonsoft.Json;
using Terraria.GameContent.Drawing;

namespace WorldPingVisualizerPlugin.Configuration
{
    /// <summary>
    /// Represents the settings used to visualize pings.
    /// </summary>
    public class VisualizerSettings
    {
        /// <summary>
        /// Represents the settings used to visualize pings via particles.
        /// </summary>
        public class ParticleSettings
        {
            /// <summary>
            /// Gets or sets if particles are enabled.
            /// </summary>
            [JsonProperty(
                "enabled"
            )]
            public bool Enabled { get; set; } = true;

            /// <summary>
            /// Gets or sets the interval to show particles at pings.
            /// </summary>
            [JsonProperty(
                "particlesIntervalMilliseconds"
            )]
            public int ParticlesIntervalMilliseconds { get; set; } = 1000;

            /// <summary>
            /// Gets or sets the particle type to be used.
            /// </summary>
            [JsonProperty(
                "particleType"
            )]
            public ParticleOrchestraType ParticleType { get; set; } = ParticleOrchestraType.StellarTune;
        }

        /// <summary>
        /// Represents the settings used to visualize pings via combat text.
        /// </summary>
        public class CombatTextSettings
        {
            /// <summary>
            /// Gets or sets if combat text is enabled.
            /// </summary>
            [JsonProperty(
                "enabled"
            )]
            public bool Enabled { get; set; }

            /// <summary>
            /// Gets or sets the interval to show combat text at pings.
            /// </summary>
            [JsonProperty(
                "combatTextIntervalMilliseconds"
            )]
            public int CombatTextIntervalMilliseconds { get; set; } = 1000;

            /// <summary>
            /// Gets or sets the contents of the combat text.
            /// </summary>
            [JsonProperty(
                "combatTextContents"
            )]
            public string CombatTextContents { get; set; } = "PONG!";

            /// <summary>
            /// Gets or sets the color of the combat text.
            /// </summary>
            [JsonProperty(
                "combatTextColor"
            )]
            public uint CombatTextColor { get; set; } = 0xFFFF00;
        }

        /// <summary>
        /// Gets or sets the particle settings.
        /// </summary>
        /// <see cref="ParticleSettings"/>
        [JsonProperty(
            "particles"
        )]
        public ParticleSettings Particles { get; set; } = new ParticleSettings();

        /// <summary>
        /// Gets or sets the combat text settings.
        /// </summary>
        /// <seealso cref="CombatTextSettings"/>
        [JsonProperty(
            "combatText"
        )]
        public CombatTextSettings CombatText { get; set; } = new CombatTextSettings();
    }
}
