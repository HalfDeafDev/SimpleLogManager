using SimpleLogManager.Conditions;
using SimpleLogManager.ConfigOptions;
using SimpleLogManager.Configs;
using SimpleLogManager.Handlers;
using SimpleLogManager.Types;

namespace SimpleLogManager
{
    /// <summary>
    /// An implementation of SimpleLogManager that comes pre-loaded
    /// with Back-Up and Maintenance options and behaviors
    /// </summary>
    public class SimpleLogManager
    {
        static BackUpHandler PreloadedBackUpHandler()
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
                        return true;
                    }
                    ;

                    return false;
                }
            );

            backUpHandler.AddHandler<IntervalConditionOptions>(
                BackUpCondition.CreationDate,
                (config, options) =>
                {
                    var triggerTime = Helpers.AddInterval(config.LogFileInfo.CreationTime, options.Interval, options.IntervalType);

                    if (DateTime.Now >= triggerTime)
                    {
                        return true;
                    }
                    return false;
                }
            );

            backUpHandler.AddHandler<IntervalConditionOptions>(
                BackUpCondition.LastModDate,
                (config, options) =>
                {
                    var triggerTime = Helpers.AddInterval(config.LogFileInfo.LastWriteTime, options.Interval, options.IntervalType);

                    if (DateTime.Now >= triggerTime)
                    {
                        return true;
                    }
                    return false;
                }
            );

            return backUpHandler;
        }

        static MaintenanceHandler PreloadedMaintenanceHandler()
        {
            MaintenanceHandler backUpMaintenanceHandler = new();

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

        static ConfigOptionsFactory<BackUpCondition> PreloadedBackUpOptionsFactory()
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

        static ConfigOptionsFactory<MaintenanceCondition> PreloadedMaintenanceOptionsFactory()
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

        SLMConfigFactory SLMConfigFactory { get; set; }
        SLMConfigExecutor SLMConfigExecutor { get; set; }

        public SimpleLogManager()
        {
            SLMConfigFactory = CreateSLMConfigFactory();
            SLMConfigExecutor = CreateSLMConfigExecutor();
        }

        public void Execute()
        {
            Execute(null);
        }

        public void Execute(string? configPath)
        {
            List<SLMConfig> slmConfigs;
            
            if (configPath is not null)
            {
                slmConfigs = SLMConfigFactory.ParseRawConfig(configPath).ToList();
            } else
            {
                slmConfigs = SLMConfigFactory.ParseRawConfig().ToList();
            }

            foreach (var slmConfig in slmConfigs)
            {
                SLMConfigExecutor.ExecuteConfig(slmConfig);
            }
        }

        static SLMConfigExecutor CreateSLMConfigExecutor()
        {
            SLMConfigExecutor executor = new(
                PreloadedMaintenanceHandler(),
                PreloadedBackUpHandler(),
                new BackUpStrategy()
            );

            return executor;
        }

        static SLMConfigFactory CreateSLMConfigFactory()
        {
            SLMConfigFactory slm = new(
                PreloadedBackUpOptionsFactory(),
                PreloadedMaintenanceOptionsFactory()
            );

            return slm;
        }

    }
}