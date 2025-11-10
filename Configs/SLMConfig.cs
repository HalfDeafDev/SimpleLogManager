using SimpleLogManager.Conditions;
using SimpleLogManager.ConfigOptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SimpleLogManager.Configs
{
    public record SLMConfigValues(
        FileInfo LogFile,
        DirectoryInfo BackUpDirectory,
        BackUpCondition BackUpCondition,
        IMaintenanceOptions BackUpOptions,
        MaintenanceCondition MaintenanceCondition,
        IMaintenanceOptions MaintenanceOptions,
        bool WriteStartMessage,
        string StartMessage,
        bool WriteEndMessage,
        string EndMessage
    );

    /// <summary>
    /// The concrete Configuration object with defined Conditions, Option Groups, and Paths
    /// </summary>
    public class SLMConfig
    {
        public FileInfo LogFileInfo { get; set; }
        public DirectoryInfo BackUpDirectoryInfo { get; set; }
        public BackUpCondition BackUpCondition { get; set; }
        public MaintenanceCondition MaintenanceCondition { get; set; }
        public IMaintenanceOptions BackUpOptions { get; set; }
        public IMaintenanceOptions MaintenanceOptions { get; set; }

        public bool WriteStartMessage { get; set; }
        public string StartMessage { get; set; }
        public bool WriteEndMessage { get; set; }
        public string EndMessage { get; set; }

        public SLMConfig(SLMConfigValues configValues)
        {
            LogFileInfo = configValues.LogFile;
            BackUpDirectoryInfo = configValues.BackUpDirectory;
            BackUpCondition = configValues.BackUpCondition;
            BackUpOptions = configValues.BackUpOptions;
            MaintenanceCondition = configValues.MaintenanceCondition;
            MaintenanceOptions = configValues.MaintenanceOptions;
            WriteStartMessage = configValues.WriteStartMessage;
            StartMessage = configValues.StartMessage;
            WriteEndMessage = configValues.WriteEndMessage;
            EndMessage = configValues.EndMessage;
        }
    }
}
