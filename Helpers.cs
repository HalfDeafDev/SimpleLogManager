using SimpleLogManager.Configs;
using SimpleLogManager.Types;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SimpleLogManager
{
    public enum ChangeDateByOperation {
        Add,
        Subtract
    }

    public static class Helpers
    {
        public static DirectoryInfo? GetDirectoryInfo(RawConfig rawConfig)
        {
            try
            {
                return new DirectoryInfo(rawConfig.BackUpDirectory);
            } catch
            {
                return null;
            }
        }

        public static ByteSize? StringToByteSize(string? str)
        {
            if (str is null) return null;

            switch (str.ToUpper())
            {
                case "B":
                case "BYTE":
                    return ByteSize.Byte;
                case "KB":
                case "KILOBYTE":
                    return ByteSize.KiloByte;
                case "MB":
                case "MEGABYTE":
                    return ByteSize.MegaByte;
                case "GB":
                case "GIGABYTE":
                    return ByteSize.GigaByte;
                case "PB":
                case "PETABYTE":
                    return ByteSize.PetaByte;
            }

            return null;
        }

        public static IntervalType? StringToIntervalType(string? strIntervalType)
        {
            if (strIntervalType is null) return null;

            switch (strIntervalType.ToLower())
            {
                case "years":
                    return IntervalType.Years;
                case "months":
                    return IntervalType.Months;
                case "weeks":
                    return IntervalType.Weeks;
                case "days":
                    return IntervalType.Days;
                case "hours":
                    return IntervalType.Hours;
                case "minutes":
                    return IntervalType.Minutes;
                case "seconds":
                    return IntervalType.Seconds;
                default:
                    return null;
            }
        }

        public static List<string> GetSortedLogFiles(string directory, string extension)
        {
            Console.WriteLine($"Checking for existing log files in {directory}");
            string[] logs = Directory.GetFiles(
                            directory,
                            $"*{extension}");

            Console.WriteLine($"{logs.Length} found with {extension} extension");

            Array.Sort(logs, CompareLogIndexes);

            return logs.ToList();
        }

        public static bool GetLogIndex(string s, out int value)
        {
            var split = s.Split('.');
            bool result = int.TryParse(split[split.Length - 2], out int v);
            value = v;
            return result;
        }

        public static int CompareLogIndexes(string s1, string s2)
        {
            GetLogIndex(s1, out int xValue);
            GetLogIndex(s2, out int yValue);

            return xValue.CompareTo(yValue);
        }

        public static void GenerateSampleConfig(string configPath)
        {
            string jsonString = JsonSerializer.Serialize(
                $$"""
                [
                    {
                        "LogFilePath": "C:\\Your\Log\Path\File.log",
                        "BackUpDirectory": "C:\\Your\\BackUp\Directory",
                        "BackUpCondition": "FileSize",
                        "MaxFileSize": 10,
                        "MaxFileSizeUnit": "KB",
                        "MaintenanceCondition": "LogCount",
                        "NumOfLogs": 4
                    }
                ]
                """
            );

            File.WriteAllBytes(configPath, Encoding.UTF8.GetBytes(jsonString));

        }

        public static FileInfo? GetLogFileInfo(RawConfig rawConfig)
        {
            try
            {
                return new FileInfo(rawConfig.LogFilePath);
            } catch
            {
                return null;
            }
        }

        public static void FillFile(
            string filePath,
            double targetSize, ByteSize sizeUnit,
            double segmentSize = 1, ByteSize segmentSizeUnit = ByteSize.KiloByte,
            double maxSegmentSize = 5, ByteSize maxSegmentSizeUnit = ByteSize.KiloByte)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Path: \"{filePath}\" either doesn't exist or is inaccessible due to permissions.");
                return;
            }

            double maxSegmentSizeInBytes = ByteConversion.Convert(maxSegmentSize, maxSegmentSizeUnit, ByteSize.Byte);
            double segmentSizeInBytes = Math.Min(
                ByteConversion.Convert(segmentSize, segmentSizeUnit, ByteSize.Byte),
                maxSegmentSizeInBytes
            );
            double targetSizeInBytes = ByteConversion.Convert(targetSize, sizeUnit, ByteSize.Byte);

            using (FileStream fs = File.OpenWrite(filePath))
            {
                int iterations = 0;
                string str = new('*', (int)segmentSizeInBytes);

                while (iterations * segmentSizeInBytes < targetSizeInBytes)
                {
                    fs.Position = fs.Length;
                    fs.Write(Encoding.UTF8.GetBytes(str));
                    iterations++;
                }

                fs.Write(Encoding.UTF8.GetBytes("\n"));
            }
        }

        public static async Task WriteToLog(string filePath, string message)
        {
            if (!File.Exists(filePath)) return;

            await File.AppendAllTextAsync(filePath, $"{message}\n");
        }

        public static async void WriteToLog(FileInfo fileInfo, string message)
        {
            await WriteToLog(fileInfo.FullName, message);
        }

        public static async void WriteDateTimeToLog(string filePath)
        {
            await WriteToLog(filePath, $"<<{DateTime.Now.ToString()}>>");
        }

        public static async void WriteStartMessageToLog(SLMConfig config)
        {
            await WriteToLog(config.LogFileInfo.FullName, config.StartMessage);
        }

        public static void WriteDateTimeToLog(FileInfo fileInfo)
        {
            WriteDateTimeToLog(fileInfo.FullName);
        }
        public static async void WriteEndMessageToLog(SLMConfig config)
        {
            await WriteToLog(config.LogFileInfo.FullName, config.EndMessage);
        }
        public static DateTime ChangeDateByInterval(
            ChangeDateByOperation operation,
            DateTime startingDate,
            double interval,
            IntervalType intervalType
        )
        {
            double actualInterval = operation == ChangeDateByOperation.Add ? interval : interval * -1;
            DateTime newDate = startingDate;
            switch (intervalType)
            {
                case IntervalType.Years:
                    newDate.AddDays(actualInterval * 365);
                    break;
                case IntervalType.Months:
                    newDate.AddDays(actualInterval * 30);
                    break;
                case IntervalType.Weeks:
                    newDate.AddDays(actualInterval * 7);
                    break;
                case IntervalType.Days:
                    newDate.AddDays(actualInterval);
                    break;
                case IntervalType.Hours:
                    newDate.AddHours(actualInterval);
                    break;
                case IntervalType.Minutes:
                    newDate.AddMinutes(actualInterval);
                    break;
                case IntervalType.Seconds:
                    newDate.AddSeconds(actualInterval);
                    break;

            }
            return newDate;
        }

        public static DateTime AddInterval(DateTime startingDate, double interval, IntervalType intervalType)
        {
            return ChangeDateByInterval(ChangeDateByOperation.Add, startingDate, interval, intervalType);
        }

        public static DateTime RemoveInterval(DateTime startingDate, double interval, IntervalType intervalType)
        {
            return ChangeDateByInterval(ChangeDateByOperation.Subtract, startingDate, interval, intervalType);
        }

        
    }
}