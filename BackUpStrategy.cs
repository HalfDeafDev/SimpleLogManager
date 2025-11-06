using SimpleLogManager.Configs;

namespace SimpleLogManager
{
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
}
