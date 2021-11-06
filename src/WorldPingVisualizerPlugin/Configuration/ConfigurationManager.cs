using System.IO;
using TShockAPI.Configuration;

namespace WorldPingVisualizerPlugin.Configuration
{
    public class ConfigurationManager
    {
        public bool Loaded { get; private set; }

        public ConfigFile<VisualizerSettings> VisualizerConfigFile { get; private set; }

        internal ConfigurationManager()
        {
        }

        public void Load()
        {
            VisualizerConfigFile = new ConfigFile<VisualizerSettings>();

            using (FileStream fs = new FileStream(
                Paths.VisualizerConfigPath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read))
            {
                VisualizerConfigFile.Read(fs, out bool incompleteSettings);
            }

            Loaded = true;
        }
    }
}
