using System.IO;
using TerrariaApi.Server;

namespace WorldPingVisualizerPlugin.Configuration
{
    public static class Paths
    {
        public static readonly string ConfigPath = Path.GetFullPath("config");

        public static readonly string SavePath = Path.Combine(ConfigPath, "WorldPingVisualizerPlugin");

        public static readonly string VisualizerConfigPath = Path.Combine(SavePath, "config-visualizer.json");
    }
}
