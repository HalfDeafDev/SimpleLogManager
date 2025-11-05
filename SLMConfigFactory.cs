using SimpleLogManager.Conditions;
using SimpleLogManager.ConfigOptions;
using SimpleLogManager.Configs;
using SimpleLogManager.Handlers;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

/*
 *  SimpleLogManager
 *  
 *  Configuration File for
 *      - Log File Path
 *      - Directory Logs should back-up to
 *      - Back-Up Conditions
 *          - File Size
 *          - Last Modification Date
 *          - Creation Date
 *      - Back-Up Maintenance
 *          - Max # of logs
 *          - Max file size
 *          - Creation Date
 *      
 *  Every time this runs, SLM will:
 *      - Append data on Start (Default: Date & Time)
 *      - Check if Back-Up Conditions are True
 *          - If So
 *              - Copy file to specified location
 *              - Clear the original file's data
 *              - Append data (Default: Date & Time)
 *              - Check if Back-Up Maintenance needs done
 *                  - If So, Do It.
 */

namespace SimpleLogManager
{
    /// <summary>
    /// Handles the loading and parsing of configuration files
    /// </summary>
    public class SLMConfigFactory
    {
        RawConfigParser RawConfigParser { get; set; }
        ConfigOptionsFactory<BackUpCondition> BackUpOptionsFactory { get; set; }
        ConfigOptionsFactory<MaintenanceCondition> MaintenanceOptionsFactory { get; set; }

        private string DefaultConfigPath()
        {
            return Path.Combine(
                Directory.GetCurrentDirectory(),
                "config.json"
            );
        }

        public SLMConfigFactory(
            ConfigOptionsFactory<BackUpCondition> backUpOptionsFactory,
            ConfigOptionsFactory<MaintenanceCondition> maintenanceOptionsFactory,
        {
            BackUpOptionsFactory = backUpOptionsFactory;
            MaintenanceOptionsFactory = maintenanceOptionsFactory;
            RawConfigParser = new(BackUpOptionsFactory, MaintenanceOptionsFactory);
        }

        public SLMConfigFactory(
            ConfigOptionsFactory<BackUpCondition> backUpOptionsFactory,
            ConfigOptionsFactory<MaintenanceCondition> maintenanceOptionsFactory,
            RawConfigParser? rawConfigParser)
        {
            BackUpOptionsFactory = backUpOptionsFactory;
            MaintenanceOptionsFactory = maintenanceOptionsFactory;
            if (rawConfigParser is not null)
            {
                RawConfigParser = rawConfigParser;
            } else
            {
                RawConfigParser = new(BackUpOptionsFactory, MaintenanceOptionsFactory);
            }
        }

        public SLMConfigFactory(
            ConfigOptionsFactory<BackUpCondition> backUpOptionsFactory,
            ConfigOptionsFactory<MaintenanceCondition> maintenanceOptionsFactory
        )
        {
            BackUpOptionsFactory = backUpOptionsFactory;
            MaintenanceOptionsFactory = maintenanceOptionsFactory;
            RawConfigParser = new(BackUpOptionsFactory, MaintenanceOptionsFactory);
        }

        private bool ConfigFileExists(string path)
        {
            bool configExists = File.Exists(path);

            if (configExists)
            {
                Console.WriteLine($"Config found at {path}");
                return true;
            }

            Console.WriteLine($"Config not found at {path}");
            return false;
        }

        // TODO: Set to prompt for sample config creation
        private void HandleMissingConfig(string configPath)
        {
            Console.WriteLine("Creating Sample Config File.");
            Helpers.GenerateSampleConfig(configPath);
            Console.WriteLine("Sample Configuration Created.");
        }
                
        public IEnumerable<SLMConfig> ParseRawConfig(string configPath)
        {
            List<SLMConfig> configs = [];

            if (!ConfigFileExists(configPath))
            {
                HandleMissingConfig(configPath);
                return configs;
            }

            try
            {
                string jsonText = File.ReadAllText(configPath);
                List<RawConfig>? rawConfigs = JsonSerializer.Deserialize<List<RawConfig>>(jsonText);

                if (rawConfigs is null) return [];

                return RawConfigParser.Parse(rawConfigs);

            } catch (JsonException ex)
            {
                Console.WriteLine(ex.Message);
            }

            return configs;
        }
    }
}
