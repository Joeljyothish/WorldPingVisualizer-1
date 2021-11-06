using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.NetModules;
using Terraria.Map;
using Terraria.Net;
using TerrariaApi.Server;
using WorldPingVisualizerPlugin.Configuration;

namespace WorldPingVisualizerPlugin
{
    [ApiVersion(2, 1)]
    public class WorldPingVisualizer : TerrariaPlugin
    {
        /// <inheritdoc />
        public override string Name => typeof(WorldPingVisualizer).Assembly.GetCustomAttribute<AssemblyTitleAttribute>().Title;

        /// <inheritdoc />
        public override string Description => typeof(WorldPingVisualizer).Assembly.GetCustomAttribute<AssemblyDescriptionAttribute>().Description;

        /// <inheritdoc />
        public override Version Version => typeof(WorldPingVisualizer).Assembly.GetName().Version;

        /// <inheritdoc />
        public override string Author => typeof(WorldPingVisualizer).Assembly.GetCustomAttribute<AssemblyCompanyAttribute>().Company;

        /// <summary>
        /// Gets the object that represents this plugin's configuration manager.
        /// </summary>
        public ConfigurationManager ConfigManager { get; private set; }

        /// <summary>
        /// Gets a <see cref="DateTime"/> representing the last time particles were broadcasted at pings.
        /// </summary>
        public DateTime LastParticlesTime { get; private set; }

        /// <summary>
        /// Gets a list of active pings.
        /// </summary>
        public List<PingMapLayer.Ping> Pings { get; } = new List<PingMapLayer.Ping>();

        public WorldPingVisualizer(Main game) : base(game)
        {
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            ConfigManager = new ConfigurationManager();
            ConfigManager.Load();

            ServerApi.Hooks.GamePostUpdate.Register(this, OnGamePostUpdate);
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.GamePostUpdate.Deregister(this, OnGamePostUpdate);
            }

            base.Dispose(disposing);
        }

        private void OnGamePostUpdate(EventArgs e)
        {
            var now = DateTime.Now;

            // Only visualize at specific intervals
            var particlesInterval = ConfigManager.VisualizerConfigFile.Settings.ParticlesIntervalMilliseconds;
            var timePassedParticles = (now - LastParticlesTime).TotalMilliseconds;
            if (timePassedParticles > particlesInterval)
            {
                foreach (var ping in Pings)
                {
                    // Removed expired pings
                    var pingLifetime = (now - ping.Time).TotalSeconds;
                    if (pingLifetime > PingMapLayer.PING_DURATION_IN_SECONDS)
                    {
                        Pings.Remove(ping);
                        continue;
                    }

                    // Visualize particles at ping
                    var position = ping.Position;
                    var packet = NetParticlesModule.Serialize(
                        ParticleOrchestraType.StellarTune,
                        new ParticleOrchestraSettings()
                        {
                            IndexOfPlayerWhoInvokedThis = 255,
                            PositionInWorld = position
                        });
                    NetManager.Instance.Broadcast(packet);
                }

                LastParticlesTime = now;
            }
        }
    }
}
