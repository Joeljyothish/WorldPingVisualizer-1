using System.IO;
using TShockAPI.Configuration;

namespace WorldPingVisualizerPlugin.Configuration
{
    public class ConfigurationManager
    {
        /// <summary>
        /// Gets a value indicating whether or not the configs has loaded.
        /// </summary>
        public bool Loaded { get; private set; }

        /// <summary>
        /// Gets an object representing visualizer settings.
        /// </summary>
        public ConfigFile<VisualizerSettings> VisualizerConfigFile { get; private set; }

        /// <summary>
        /// Initializes a new instance of <see cref="ConfigurationManager"/>.
        /// </summary>
        internal ConfigurationManager()
        {
        }

        /// <summary>
        /// Loads the configs.
        /// </summary>
        public void Load()
        {
            VisualizerConfigFile = new ConfigFile<VisualizerSettings>();

            Reload();

            Loaded = true;
        }

        /// <summary>
        /// Reloads the configs.
        /// </summary>
        public void Reload()
        {
            if (!Directory.Exists(Paths.SavePath))
            {
                Directory.CreateDirectory(Paths.SavePath);
            }

            var visualizerConfigPath = Paths.VisualizerConfigPath;
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
                bool incompleteSettings;
                using (FileStream fs = new FileStream(
                    visualizerConfigPath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read))
                {
                    VisualizerConfigFile.Read(fs, out incompleteSettings);
                }

                if (incompleteSettings)
                {
                    using (FileStream wfs = new FileStream(
                        visualizerConfigPath,
                        FileMode.Create,
                        FileAccess.Write,
                        FileShare.None))
                    {
                        VisualizerConfigFile.Write(wfs);
                    }
                }
            }
        }
    }
}
