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

        public SLMConfig(FileInfo logFile, DirectoryInfo backUpDirectory, 
            BackUpCondition backUpCondition, IMaintenanceOptions backUpOptions,
            MaintenanceCondition maintenanceCondition, IMaintenanceOptions maintenanceOptions)
        {
            LogFileInfo = logFile;
            BackUpDirectoryInfo = backUpDirectory;
            BackUpCondition = backUpCondition;
            BackUpOptions = backUpOptions;
            MaintenanceCondition = maintenanceCondition;
            MaintenanceOptions = maintenanceOptions;
        }
    }
}
