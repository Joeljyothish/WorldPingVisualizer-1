using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.NetModules;
using Terraria.Map;
using Terraria.Net;
using TerrariaApi.Server;
using TShockAPI.Hooks;
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
        private ConfigurationManager ConfigManager { get; private set; }

        /// <inheritdoc cref="ConfigurationManager.VisualizerConfigFile"/>
        public VisualizerSettings VisualizerSettings
        {
            get => ConfigManager.VisualizerConfigFile.Settings;
            set => ConfigManager.VisualizerConfigFile.Settings = value;
        }

        /// <summary>
        /// Gets a <see cref="DateTime"/> indicating the last time particles were broadcasted at pings.
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

            GeneralHooks.ReloadEvent += ConfigReload;

            ServerApi.Hooks.NetGetData.Register(this, OnGetData);
            ServerApi.Hooks.GamePostUpdate.Register(this, OnGamePostUpdate);
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.NetGetData.Deregister(this, OnGetData);
                ServerApi.Hooks.GamePostUpdate.Deregister(this, OnGamePostUpdate);
            }

            base.Dispose(disposing);
        }

        private void OnGetData(GetDataEventArgs e)
        {
            using (var stream = new MemoryStream(e.Msg.readBuffer, e.Index, e.Length))
            {
                if (e.MsgID == PacketTypes.LoadNetModule)
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        var id = reader.ReadUInt16();
                        var module = NetManager.Instance._modules[id];
                        if (module.GetType() == typeof(NetPingModule))
                        {
                            var position = reader.ReadVector2();
                            var ping = new PingMapLayer.Ping(position);
                            Pings.Add(ping);
                        }
                    }
                }
            }
        }

        private void OnGamePostUpdate(EventArgs e)
        {
            var now = DateTime.Now;

            var visualizerSettings = VisualizerSettings;

            // Only visualize at specific intervals
            var particlesInterval = visualizerSettings.ParticlesIntervalMilliseconds;
            var timePassedParticles = (now - LastParticlesTime).TotalMilliseconds;
            if (timePassedParticles > particlesInterval)
            {
                foreach (var ping in Pings)
                {
                    if (IsExpired(ping))
                    {
                        Pings.Remove(ping);
                        continue;
                    }

                    // Visualize particles at ping
                    var position = ping.Position;
                    var particleType = visualizerSettings.ParticleType;
                    var settings = new ParticleOrchestraSettings()
                    {
                        IndexOfPlayerWhoInvokedThis = 255,
                        PositionInWorld = position
                    };
                    var packet = NetParticlesModule.Serialize(
                        particleType,
                        settings);
                    NetManager.Instance.Broadcast(packet);
                }

                LastParticlesTime = now;
            }

            bool IsExpired(PingMapLayer.Ping ping)
            {
                // Removed expired pings
                var pingLifetime = (now - ping.Time).TotalSeconds;
                if (pingLifetime > PingMapLayer.PING_DURATION_IN_SECONDS)
                {
                    return true;
                }

                return false;
            }
        }

        private void ConfigReload(ReloadEventArgs e)
        {
            ConfigManager.Reload();
        }
    }
}
