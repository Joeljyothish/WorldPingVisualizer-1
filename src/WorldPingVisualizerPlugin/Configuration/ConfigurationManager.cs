using LrndefLib;
using Newtonsoft.Json;
using System.IO;

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
        public VersionedConfigFile<VisualizerSettings> VisualizerConfigFile { get; private set; }

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
            VisualizerConfigFile = new VersionedConfigFile<VisualizerSettings>(
                VisualizerSettings.CurrentVersion
            );

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
                VisualizerConfigFile.Metadata = new SettingsMetadata(
                    SettingsMetadata.CurrentMetadataVersion,
                    VisualizerSettings.CurrentVersion);
                VisualizerConfigFile.Settings = new VisualizerSettings();
                using (FileStream fs = new FileStream(
                    visualizerConfigPath,
                    FileMode.CreateNew,
                    FileAccess.Write,
                    FileShare.None))
                {
                    VisualizerConfigFile.Write(fs, TransformWriter);
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
                        VisualizerConfigFile.Write(wfs, TransformWriter);
                    }
                }
            }
        }

        private void TransformWriter(JsonTextWriter writer)
        {
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 4;
        }
    }
}
