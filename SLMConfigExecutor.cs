using SimpleLogManager.Configs;
using SimpleLogManager.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleLogManager
{
    public interface IBackUpStrategy
    {
        public void BackUp(SLMConfig config);
    }

    internal class BackUpStrategy : IBackUpStrategy
    {
        public void BackUp(SLMConfig config)
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
    }

    internal class SLMConfigExecutor
    {
        MaintenanceHandler MaintenanceHandler { get; set; }
        BackUpHandler BackUpHandler { get; set; }
        IBackUpStrategy BackUpStrategy { get; set; }

        public SLMConfigExecutor(
            MaintenanceHandler maintenanceHandler,
            BackUpHandler backUpHandler,
            IBackUpStrategy backUpStrategy
        )
        {
            MaintenanceHandler = maintenanceHandler;
            BackUpHandler = backUpHandler;
            BackUpStrategy = backUpStrategy;
        }

        public void ExecuteConfig(SLMConfig config)
        {
            Helpers.WriteDateTimeToLog(config.LogFileInfo.FullName);

            BackUpHandler.Handle(config, IBackUpStrategy strategy);

            MaintenanceHandler.Handle(config);
        }

    }
}
