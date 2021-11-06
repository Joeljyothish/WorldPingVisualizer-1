using Terraria.GameContent.Drawing;
using Terraria.GameContent.NetModules;
using Terraria.Net;
using static Terraria.Map.PingMapLayer;

namespace WorldPingVisualizerPlugin.Extensions
{
    /// <summary>
    /// Provides extension methods to the <see cref="Ping"/> class.
    /// </summary>
    public static class PingExtensions
    {
        /// <summary>
        /// Show &lt;<paramref name="particleType"/>&gt; particles at <paramref name="ping"/>.
        /// </summary>
        /// <param name="ping">The ping.</param>
        /// <param name="particleType">The particle type to show.</param>
        public static void ShowParticles(this Ping ping, ParticleOrchestraType particleType)
        {
            // Convert to pixel position
            var position = ping.Position * 16;

            var settings = new ParticleOrchestraSettings()
            {
                // 255 is the server player's index
                // We don't track ping creators, for now
                IndexOfPlayerWhoInvokedThis = 255,

                PositionInWorld = position
            };

            // Create the packet
            var packet = NetParticlesModule.Serialize(
                particleType,
                settings);

            // Broadcast it
            NetManager.Instance.Broadcast(packet);
        }
    }
}
