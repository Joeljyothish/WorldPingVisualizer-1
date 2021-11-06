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
            var visualizerConfigPath = Paths.VisualizerConfigPath;
            VisualizerConfigFile = new ConfigFile<VisualizerSettings>();

            if (!File.Exists(visualizerConfigPath))
            {
                VisualizerConfigFile.Settings = new VisualizerSettings();
                using (FileStream fs = new FileStream(
                    visualizerConfigPath,
                    FileMode.CreateNew,
                    FileAccess.Write,
                    FileShare.None))
                {
                    VisualizerConfigFile.Write(fs);
                }
            }
            else
            {
                using (FileStream fs = new FileStream(
                    visualizerConfigPath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read))
                {
                    VisualizerConfigFile.Read(fs, out bool incompleteSettings);
                }
            }

            Loaded = true;
        }
    }
}
