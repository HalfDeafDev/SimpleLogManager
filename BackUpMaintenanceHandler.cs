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
    internal partial class SimpleLogManagerCLI
    {
        private class BackUpMaintenanceHandler
        {
            public static void Handle(MaintenanceCondition maintenanceCondition, ConfigValues values)
            {
                switch (maintenanceCondition)
                {
                    case MaintenanceCondition.LogCount:
                        break;
                    case MaintenanceCondition.FolderSize:
                        break;
                    case MaintenanceCondition.CreationDate:
                        break;
                    default:
                        Console.WriteLine("[91mWarning: No Back-Up Maintenance Set");
                        break;
                }
            }
        }
    }
}
