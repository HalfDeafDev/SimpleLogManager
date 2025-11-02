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
    /// An implementaiton of the SimpleLogManager
    /// </summary>
    public class SimpleLogManagerCLI
    {
        RawConfigParser RawConfigParser { get; set; }
        ConfigOptionsFactory<BackUpCondition> BackUpOptionsFactory { get; set; }
        ConfigOptionsFactory<MaintenanceCondition> MaintenanceOptionsFactory { get; set; }
        BackUpHandler BackUpHandler { get; set; }
        BackUpMaintenanceHandler BackUpMaintenanceHandler { get; set; }
        string ConfigPath { get; }
        public SimpleLogManagerCLI(
            ConfigOptionsFactory<BackUpCondition> backUpOptionsFactory,
            ConfigOptionsFactory<MaintenanceCondition> maintenanceOptionsFactory,
            BackUpHandler backUpHandler, BackUpMaintenanceHandler backUpMaintenanceHandler)
        {
            BackUpHandler = backUpHandler;
            BackUpMaintenanceHandler = backUpMaintenanceHandler;
            BackUpOptionsFactory = backUpOptionsFactory;
            MaintenanceOptionsFactory = maintenanceOptionsFactory;
            ConfigPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "config.json"
            );
            RawConfigParser = new(BackUpOptionsFactory, MaintenanceOptionsFactory);
        }

        public SimpleLogManagerCLI(
            ConfigOptionsFactory<BackUpCondition> backUpOptionsFactory,
            ConfigOptionsFactory<MaintenanceCondition> maintenanceOptionsFactory,
            BackUpHandler backUpHandler, BackUpMaintenanceHandler backUpMaintenanceHandler,
            RawConfigParser? rawConfigPraser)
        {
            BackUpHandler = backUpHandler;
            BackUpMaintenanceHandler = backUpMaintenanceHandler;
            BackUpOptionsFactory = backUpOptionsFactory;
            MaintenanceOptionsFactory = maintenanceOptionsFactory;
            ConfigPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "config.json"
            );
            if (rawConfigPraser is not null)
            {
                RawConfigParser = rawConfigPraser;
            } else
            {
                RawConfigParser = new(BackUpOptionsFactory, MaintenanceOptionsFactory);
            }
        }

        public SimpleLogManagerCLI(
            ConfigOptionsFactory<BackUpCondition> backUpOptionsFactory,
            ConfigOptionsFactory<MaintenanceCondition> maintenanceOptionsFactory,
            BackUpHandler backUpHandler, BackUpMaintenanceHandler backUpMaintenanceHandler,
            string configPath)
        {
            BackUpHandler = backUpHandler;
            BackUpMaintenanceHandler = backUpMaintenanceHandler;
            BackUpOptionsFactory = backUpOptionsFactory;
            MaintenanceOptionsFactory = maintenanceOptionsFactory;
            ConfigPath = configPath;
            RawConfigParser = new(BackUpOptionsFactory, MaintenanceOptionsFactory);
        }

        public void Run()
        {
            if (!ConfigExists(ConfigPath))
            {
                HandleMissingConfig(ConfigPath);
                return;
            }

            IEnumerable<SLMConfig> configs = GetSLMConfigs();

            foreach (var slmConfig in configs)
            {
                ExecuteConfig(slmConfig);
            }
        }

        private bool ConfigExists(string path)
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

                
        private IEnumerable<SLMConfig> GetSLMConfigs()
        {
            List<SLMConfig> configs = [];

            try
            {
                string jsonText = File.ReadAllText(ConfigPath);
                List<RawConfig>? rawConfigs = JsonSerializer.Deserialize<List<RawConfig>>(jsonText);

                if (rawConfigs is null) return [];

                return RawConfigParser.CreateSLMConfig(rawConfigs);

            } catch (JsonException ex)
            {
                Console.WriteLine(ex.Message);
            }

            return configs;
        }

        private void ExecuteConfig(SLMConfig config) {

            Helpers.WriteDateTimeToLog(config.LogFileInfo.FullName);

            BackUpHandler.Handle(config);

            BackUpMaintenanceHandler.Handle(config);
        }
    }
}
