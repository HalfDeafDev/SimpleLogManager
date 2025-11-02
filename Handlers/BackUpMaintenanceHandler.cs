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

using SimpleLogManager.Conditions;
using SimpleLogManager.ConfigOptions;
using SimpleLogManager.Configs;

namespace SimpleLogManager.Handlers
{
    public class BackUpMaintenanceHandler
    {
        Dictionary<MaintenanceCondition, Action<SLMConfig>> strategies = new();

        public bool AddHandler<TOptionType>(MaintenanceCondition condition, Action<SLMConfig, TOptionType> action)
            where TOptionType : IMaintenanceOptions
        {
            Action<SLMConfig> wrapper = (config) =>
            {
                if (config.MaintenanceOptions is TOptionType opts)
                {
                    action(config, opts);
                    return;
                }

                Console.WriteLine($"{config.MaintenanceOptions} does not match the expected {nameof(TOptionType)}");
            };

            return strategies.TryAdd(condition, wrapper);
        }

        public void Handle(SLMConfig config)
        {
            strategies.TryGetValue(config.MaintenanceCondition, out var action);

            if (action is not null)
            {
                Console.WriteLine($"Back Up Handler found for {config.MaintenanceCondition}");
                action(config);
            }
        }
    }
}
