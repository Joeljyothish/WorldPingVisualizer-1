using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.NetModules;
using Terraria.Localization;
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
        #region Plugin Information

        /// <inheritdoc />
        public override string Name => typeof(WorldPingVisualizer).Assembly.GetCustomAttribute<AssemblyTitleAttribute>().Title;

        /// <inheritdoc />
        public override string Description => typeof(WorldPingVisualizer).Assembly.GetCustomAttribute<AssemblyDescriptionAttribute>().Description;

        /// <inheritdoc />
        public override Version Version => typeof(WorldPingVisualizer).Assembly.GetName().Version;

        /// <inheritdoc />
        public override string Author => typeof(WorldPingVisualizer).Assembly.GetCustomAttribute<AssemblyCompanyAttribute>().Company;

        #endregion

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

        #region Last- Times

        /// <summary>
        /// Gets a <see cref="DateTime"/> indicating the last time ping expiration was checked.
        /// </summary>
        public DateTime LastExpiredCheckTime { get; private set; }

        /// <summary>
        /// Gets a <see cref="DateTime"/> indicating the last time particles were broadcasted at pings.
        /// </summary>
        public DateTime LastParticlesTime { get; private set; }

        /// <summary>
        /// Gets a <see cref="DateTime"/> indicating the last time CombatText was broadcasted at pings.
        /// </summary>
        public DateTime LastCombatTextTime { get; private set; }

        #endregion

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

        #region Hooks

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

            if (Pings.Count > 0)
            {
                var timePassedExpiration = (now - LastExpiredCheckTime).TotalSeconds;
                if (timePassedExpiration > 1)
                {
                    // Reverse for loop to delete times
                    // Using a normal for loop to clear items shifts the indexes
                    // And will lead to Index Out of Range Exception
                    for (int i = Pings.Count; i-- > 0;)
                    {
                        var ping = Pings[i];
                        var pingLifetime = (now - ping.Time).TotalSeconds;
                        if (pingLifetime > PingMapLayer.PING_DURATION_IN_SECONDS)
                        {
                            Pings.RemoveAt(i);
                        }
                    }
                }
            }

            var visualizerSettings = VisualizerSettings;

            var particlesInterval = visualizerSettings.ParticlesIntervalMilliseconds;
            var timePassedParticles = (now - LastParticlesTime).TotalMilliseconds;
            if (timePassedParticles > particlesInterval)
            {
                foreach (var ping in Pings)
                {
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

            var combatTextInterval = visualizerSettings.CombatTextIntervalMilliseconds;
            var timePassedCombatText = (now - LastCombatTextTime).TotalMilliseconds;
            if (timePassedCombatText > combatTextInterval)
            {
                foreach (var ping in Pings)
                {
                    var combatTextContents = visualizerSettings.CombatTextContents;
                    var networkText = NetworkText.FromLiteral(combatTextContents);

                    var argbColor = visualizerSettings.CombatTextColor;
                    var abgrColor =
                        (argbColor & 0xFF00FF00)
                      | ((argbColor & 0x00FF0000) >> 16)
                      | ((argbColor & 0x000000FF) << 16);

                    NetMessage.SendData(
                        msgType: (int)PacketTypes.CreateCombatTextExtended,
                        text: networkText,
                        number: (int)abgrColor,
                        number2: ping.Position.X,
                        number3: ping.Position.Y);
                }

                LastCombatTextTime = now;
            }
        }

        #endregion

        #region TShock Hooks

        private void ConfigReload(ReloadEventArgs e)
        {
            ConfigManager.Reload();
        }

        #endregion
    }
}
