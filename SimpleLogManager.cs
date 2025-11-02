using SimpleLogManager.Conditions;
using SimpleLogManager.ConfigOptions;
using SimpleLogManager.Configs;
using SimpleLogManager.Handlers;
using SimpleLogManager.Types;

namespace SimpleLogManager
{
    public static class SimpleLogManager
    {
        /// <summary>
        /// This is how we actually perform the Back-Up.
        /// </summary>
        /// <param name="config"></param>
        static void BackUpStrategy(SLMConfig config)
        {
            var sortedLogs = Helpers.GetSortedLogFiles(
                            config.BackUpDirectoryInfo.FullName,
                            $"{config.LogFileInfo.Extension}");

            Helpers.GetLogIndex(sortedLogs.Last(), out int lastIndex);

            string fileName = Path.GetFileNameWithoutExtension(config.LogFileInfo.FullName);

            File.Move(
                config.LogFileInfo.FullName,
                Path.Combine(
                    config.BackUpDirectoryInfo.FullName,
                    $"{fileName}.{lastIndex + 1}{config.LogFileInfo.Extension}"
                )
            );

            using (var fs = File.Create(config.LogFileInfo.FullName))
            {
                StreamWriter sw = new(fs);
                sw.WriteLine($"<<{DateTime.Now}>>");
                sw.Flush();
            }
        }

        /// <summary>
        /// Pre-Loaded Back-Up Handler
        /// </summary>
        /// <returns></returns>
        static BackUpHandler CreateBackUpHandler()
        {
            BackUpHandler backUpHandler = new();

            // TODO: Test move to backup increases index value correctly
            backUpHandler.AddHandler<SizeConditionOptions>(
                BackUpCondition.FileSize,
                (config, options) =>
                {
                    double maxSizeInBytes = ByteConversion.Convert(
                        options.Size,
                        options.ByteSize,
                        ByteSize.Byte
                    );

                    Console.WriteLine($"{config.LogFileInfo.Length} >= {maxSizeInBytes}");

                    if (config.LogFileInfo.Length >= maxSizeInBytes)
                    {
                        BackUpStrategy(config);
                    }
                    ;
                }
            );

            backUpHandler.AddHandler<IntervalConditionOptions>(
                BackUpCondition.CreationDate,
                (config, options) =>
                {
                    var triggerTime = Helpers.AddInterval(config.LogFileInfo.CreationTime, options.Interval, options.IntervalType);

                    if (DateTime.Now >= triggerTime)
                    {
                        BackUpStrategy(config);
                    }
                }
            );

            backUpHandler.AddHandler<IntervalConditionOptions>(
                BackUpCondition.LastModDate,
                (config, options) =>
                {
                    var triggerTime = Helpers.AddInterval(config.LogFileInfo.LastWriteTime, options.Interval, options.IntervalType);

                    if (DateTime.Now >= triggerTime)
                    {
                        BackUpStrategy(config);
                    }
                }
            );

            return backUpHandler;
        }

        /// <summary>
        /// Pre-Loaded Back-Up Maintenance Handler
        /// </summary>
        /// <returns></returns>
        static BackUpMaintenanceHandler CreateBackUpMaintenanceHandler()
        {
            BackUpMaintenanceHandler backUpMaintenanceHandler = new();

            backUpMaintenanceHandler.AddHandler<CountConditionOptions>(
                MaintenanceCondition.LogCount,
                (config, options) =>
                {
                    List<string> logs = Helpers.GetSortedLogFiles(
                        config.BackUpDirectoryInfo.FullName,
                        config.LogFileInfo.Extension
                    );

                    int fileCount = logs.Count;

                    Console.WriteLine($"{fileCount} > {options.NumOfLogs}");

                    if (fileCount > options.NumOfLogs)
                    {
                        File.Delete(logs[0]);
                    }
                }
            );

            backUpMaintenanceHandler.AddHandler<SizeConditionOptions>(
                MaintenanceCondition.FolderSize,
                (config, options) =>
                {
                    var files = config.BackUpDirectoryInfo.GetFiles();
                    var orderedFiles = files.OrderByDescending(fileInfo => fileInfo.CreationTime);
                    long folderSize = 0;
                    double maxFolderSize = ByteConversion.Convert(options.Size, options.ByteSize, ByteSize.Byte);
                    bool shouldDelete = false;
                    foreach (var file in orderedFiles)
                    {
                        if (!shouldDelete)
                            if (folderSize + file.Length > maxFolderSize)
                                shouldDelete = true;
                            else
                                folderSize += file.Length;

                        if (shouldDelete)
                                file.Delete();
                    }
                }
            );

            backUpMaintenanceHandler.AddHandler<IntervalConditionOptions>(
                MaintenanceCondition.CreationDate,
                (config, options) =>
                {
                    var files = config.BackUpDirectoryInfo.GetFiles();
                    var filesByDateDesc = files.OrderByDescending(fileInfo => fileInfo.CreationTime);

                    DateTime triggerDate = Helpers.RemoveInterval(DateTime.Now, options.Interval, options.IntervalType);

                    bool shouldDelete = false;
                    foreach (var file in filesByDateDesc)
                    {
                        if (!shouldDelete)
                            if (file.CreationTime <= triggerDate)
                                shouldDelete = true;

                        if (shouldDelete)
                            file.Delete();
                    }
                }
            );

            return backUpMaintenanceHandler;
        }

        /// <summary>
        /// Pre-Loaded MaintenanceOptionsFactory for Back-Up Conditions
        /// </summary>
        /// <returns></returns>
        static ConfigOptionsFactory<BackUpCondition> CreateBackUpOptionsFactory()
        {
            ConfigOptionsFactory<BackUpCondition> backUpOptions = new();

            backUpOptions.TryAdd(BackUpCondition.FileSize, (rawConfig) =>
            {
                double? nullableSize = rawConfig.MaxFileSize;
                ByteSize? nullableByteSize = ByteConversion.StringToByteSize(rawConfig.MaxFileSizeUnit);

                if (nullableSize is double size && nullableByteSize is ByteSize byteSize)
                {
                    return new SizeConditionOptions(
                        size,
                        byteSize
                    );
                }

                return MaintenanceOptions.NoOptions;
            });

            backUpOptions.TryAdd(BackUpCondition.LastModDate, (rawConfig) =>
            {
                if (
                    rawConfig.Interval is int interval &&
                    Helpers.StringToIntervalType(rawConfig.IntervalType) is IntervalType intervalType
                )
                {
                    return new IntervalConditionOptions(interval, intervalType);
                }

                return MaintenanceOptions.NoOptions;
            });

            backUpOptions.TryAdd(BackUpCondition.CreationDate, (rawConfig) =>
            {
                if (
                    rawConfig.Interval is int interval &&
                    Helpers.StringToIntervalType(rawConfig.IntervalType) is IntervalType intervalType
                )
                {
                    return new IntervalConditionOptions(interval, intervalType);
                }

                return MaintenanceOptions.NoOptions;
            });

            return backUpOptions;
        }

        /// <summary>
        /// Pre-Loaded Maintenance Options Factory
        /// </summary>
        /// <returns></returns>
        static ConfigOptionsFactory<MaintenanceCondition> CreateMaintenanceOptionsFactory()
        {
            ConfigOptionsFactory<MaintenanceCondition> maintenanceOptions = new();

            maintenanceOptions.TryAdd(MaintenanceCondition.LogCount, (rawConfig) =>
            {
                if (rawConfig.NumOfLogs is int numOfLogs)
                {
                    return new CountConditionOptions(numOfLogs);
                }
                return MaintenanceOptions.NoOptions;
            });

            maintenanceOptions.TryAdd(MaintenanceCondition.FolderSize, (rawConfig) =>
            {
                if (
                    rawConfig.MaxFolderSize is int folderSize && 
                    ByteConversion.StringToByteSize(rawConfig.MaxFolderSizeUnit) is ByteSize byteSize
                )
                {
                    return new SizeConditionOptions(folderSize, byteSize);
                }
                return MaintenanceOptions.NoOptions;
            });

            maintenanceOptions.TryAdd(MaintenanceCondition.CreationDate, (rawConfig) =>
            {
                if (
                    rawConfig.Interval is int interval &&
                    Helpers.StringToIntervalType(rawConfig.IntervalType) is IntervalType intervalType
                )
                {
                    return new IntervalConditionOptions(interval, intervalType);
                }

                return MaintenanceOptions.NoOptions;
            });

            return maintenanceOptions;
        }

        public static SimpleLogManagerCLI CreateSLM()
        {
            SimpleLogManagerCLI slm = new(
                CreateBackUpOptionsFactory(),
                CreateMaintenanceOptionsFactory(),
                CreateBackUpHandler(),
                CreateBackUpMaintenanceHandler()
            );

            return slm;
        }

        public static SimpleLogManagerCLI CreateSLM(string configPath)
        {
            SimpleLogManagerCLI slm = new(
                CreateBackUpOptionsFactory(),
                CreateMaintenanceOptionsFactory(),
                CreateBackUpHandler(),
                CreateBackUpMaintenanceHandler(),
                configPath
            );

            return slm;
        }
    }
}