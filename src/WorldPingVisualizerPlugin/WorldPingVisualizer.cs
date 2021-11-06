using System;
using System.Reflection;
using Terraria;
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

        public WorldPingVisualizer(Main game) : base(game)
        {
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            ConfigManager = new ConfigurationManager();
            ConfigManager.Load();
        }
    }
}
