using SimpleLogManager.Conditions;
using SimpleLogManager.ConfigOptions;
using SimpleLogManager.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SimpleLogManager
{
    public class RawConfigParser
    {
        ConfigOptionsFactory<BackUpCondition> BackUpOptionsFactory { get; set; }
        ConfigOptionsFactory<MaintenanceCondition> MaintenanceOptionsFactory { get; set; }

        public RawConfigParser(
            ConfigOptionsFactory<BackUpCondition> backUpOptionsFactory,
            ConfigOptionsFactory<MaintenanceCondition> maintenanceOptionsFactory
        )
        {
            BackUpOptionsFactory = backUpOptionsFactory;
            MaintenanceOptionsFactory = maintenanceOptionsFactory;
        }

        private SLMConfig? Parse(RawConfig rawConfig)
        {
            FileInfo? logFileInfo = Helpers.GetLogFileInfo(rawConfig);

            if (logFileInfo is null)
            {
                return null;
            }

            DirectoryInfo? backupDirectoryInfo = Helpers.GetDirectoryInfo(rawConfig);

            if (backupDirectoryInfo is null)
            {
                return null;
            }

            BackUpCondition backupCondition = ParseBackUpCondition(rawConfig.BackUpCondition);
            IMaintenanceOptions backupOptions = ParseBackUpOptions(backupCondition, rawConfig);

            // TODO: Consider checking for No Back Up Options (NoOptions) here and doing an abandon check

            MaintenanceCondition maintenanceCondition = ParseMaintenanceCondition(rawConfig.MaintenanceCondition);
            IMaintenanceOptions maintenanceOptions = ParseMaintenanceOptions(maintenanceCondition, rawConfig);

            // TODO: Same thing for No Maintenance Options

            SLMConfig config = new(
                logFileInfo,
                backupDirectoryInfo,
                backupCondition,
                backupOptions,
                maintenanceCondition,
                maintenanceOptions
            );

            return config;
        }

        public IEnumerable<SLMConfig> Parse(string configPath)
        {
            List<SLMConfig> configs = [];

            try
            {
                string jsonText = File.ReadAllText(configPath);
                List<RawConfig>? rawConfigs = JsonSerializer.Deserialize<List<RawConfig>>(jsonText); 
                if (rawConfigs is null) return [];

                return Parse(rawConfigs);

            } catch (JsonException ex)
            {
                Console.WriteLine(ex.Message);
            }

            return configs;
        }

        public List<SLMConfig> Parse(List<RawConfig> rawConfigs)
        {
            List<SLMConfig> configs = [];

            foreach (RawConfig rawConfig in rawConfigs)
            {
                SLMConfig? slmConfig = Parse(rawConfig);
                if (slmConfig is not null)
                {
                    configs.Add(slmConfig);
                }
            }

            return configs;
        }

        private BackUpCondition ParseBackUpCondition(string strCondition)
        {
            return strCondition.ToLower() switch
            {
                "filesize" => BackUpCondition.FileSize,
                "lastmoddate" => BackUpCondition.LastModDate,
                "creationdate" => BackUpCondition.CreationDate,
                "none" or _ => BackUpCondition.None
            };
        }
        private IMaintenanceOptions ParseBackUpOptions(BackUpCondition condition, RawConfig config)
        {
            IMaintenanceOptions? options = BackUpOptionsFactory.Create(condition, config);
            if (options is null) return new NoOptions();
            return options;
        }

        private MaintenanceCondition ParseMaintenanceCondition(string? strCondition)
        {
            if (strCondition is null) return MaintenanceCondition.None;
            switch (strCondition.ToLower())
            {
                case "logcount": return MaintenanceCondition.LogCount;
                case "foldersize": return MaintenanceCondition.FolderSize;
                case "creationdate": return MaintenanceCondition.CreationDate;
                case "none": default: return MaintenanceCondition.None;
            }
        }

        private IMaintenanceOptions ParseMaintenanceOptions(MaintenanceCondition condition, RawConfig config)
        {
            IMaintenanceOptions? options = MaintenanceOptionsFactory.Create(condition, config);
            if (options is null) return new NoOptions();
            return options;
        }

    }
}
