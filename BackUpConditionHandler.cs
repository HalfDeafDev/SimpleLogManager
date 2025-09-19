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
    internal class BackUpConditionHandler
    {
        private SLMConfig _config;

        public BackUpConditionHandler(SLMConfig config)
        {
            _config = config;
        }

        public void HandleFileSize(int maxFileSize, ByteSize byteSize, ByteSizeType byteSizeType )
        {
        }

        public void HandleCreationDate(int interval, IntervalType intervalType)
        {

        }

        public void HandleLastModDate(int interval, IntervalType intervalType)
        {

        }

        public void Handle(BackUpCondition condition, ConfigValues values)
        {
            switch (condition)
            {
                case BackUpCondition.FileSize:
                {
                    if (values.MaxFileSize is int size && values.MaxFileSizeUnit is ByteSize unit)
                    {
                        HandleFileSize(size, unit, values.ByteSizeType);
                    }
                    break;
                }
                case BackUpCondition.CreationDate:
                {
                    if (values.Interval is int size && values.IntervalType is IntervalType unit)
                    {
                        HandleCreationDate(size, unit);
                    }
                    break;
                }
                case BackUpCondition.LastModDate:
                {
                    if (values.Interval is int size && values.IntervalType is IntervalType unit)
                    {
                        HandleLastModDate(size, unit);
                    }
                    break;
                }
                default:
                    Console.WriteLine("[91mWarning: [93mNo Back Up Condition Set");
                    break;
            }
        }
    }
}
