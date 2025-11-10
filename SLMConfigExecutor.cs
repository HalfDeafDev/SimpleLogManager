using SimpleLogManager.Configs;
using SimpleLogManager.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                Helpers.WriteStartMessageToLog(config);

            BackUpHandler.Handle(config, BackUpStrategy);

            MaintenanceHandler.Handle(config);

            if (config.WriteEndMessage)
                Helpers.WriteEndMessageToLog(config);
        }

    }
}
