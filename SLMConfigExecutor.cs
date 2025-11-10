using SimpleLogManager.Configs;
using SimpleLogManager.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SimpleLogManager
{
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
            if (config.WriteStartMessage)
            {
                string formattedStartMessage = FormatMessage(config.StartMessage, config.Meta);
                Helpers.WriteToLog(config.LogFileInfo, formattedStartMessage);
            }

            BackUpHandler.Handle(config, BackUpStrategy);

            MaintenanceHandler.Handle(config);

            if (config.WriteEndMessage)
            {
                string formattedEndMessage = FormatMessage(config.EndMessage, config.Meta);
                Helpers.WriteToLog(config.LogFileInfo, formattedEndMessage);
            }
        }

        // TODO: Will eventually need to be it's own object passed to SLMConfigOptionsFactory
        private string FormatMessage(string message, SLMConfigMeta meta)
        {
            Dictionary<string, Func<string>> keywordDictionary = new Dictionary<string, Func<string>>
            {
                { "$CurrentDateTime", () => meta.CurrentDateTime.ToString() },
                { "$CurrentDate", () => meta.CurrentDateTime.Date.ToString() },
                { "$CurrentTime", () => meta.CurrentDateTime.TimeOfDay.ToString() },
                { "$FileName", () => meta.FileName },
                { "$FilePath", () => meta.FilePath }
            };

            StringBuilder sb = new(message);

            foreach (Match match in Regex.Matches(message, @"(\$\w+)"))
            {
                if (keywordDictionary.ContainsKey(match.Value))
                {
                    sb.Replace(match.Value, keywordDictionary[match.Value]());
                }
            }

            return sb.ToString();
        }
    }
}
