using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
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
    internal class SimpleLogManagerCLI
    {
        public void Run()
        {


            if (!ConfigExists())
            {
                HandleMissingConfig();
            }

            IEnumerable<SLMConfig> configs = GetSLMConfigs();

            foreach (var slmConfig in configs)
            {
                ExecuteConfig(slmConfig);
            }
        }

        private bool ConfigExists()
        {
            return false;
        }

        private void HandleMissingConfig()
        {
            
        }

        private IEnumerable<SLMConfig> GetSLMConfigs()
        {
            return [];
        }

        private void ExecuteConfig(SLMConfig config) {
            if (!File.Exists(config.LogFilePath)) return;

            using (StreamWriter sw = File.AppendText(config.LogFilePath))
            {
                sw.WriteLine($"<<{DateTime.Now.ToString()}>>");
            }

            BackUpConditionHandler.Handle(config.BackUpCondition, config.ConfigValues);

            BackUpMaintenanceHandler.Handle(config.MaintenanceCondition, config.ConfigValues);
        }
    }
}
