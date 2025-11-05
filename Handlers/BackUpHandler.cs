using SimpleLogManager.Conditions;
using SimpleLogManager.ConfigOptions;
using SimpleLogManager.Configs;

namespace SimpleLogManager.Handlers
{
    /// <summary>
    /// This is where rules for how to handle particular BackUp 
    /// </summary>
    public class BackUpHandler
    {
        // Key should probably be (BackUpCondition, TOptionType) to allow for unique demands
        // for particular conditions/types combos.

        Dictionary<BackUpCondition, Func<SLMConfig, bool>> strategies = new();

        public bool AddHandler<TOptionType>(BackUpCondition condition, Func<SLMConfig, TOptionType, bool> action)
            where TOptionType : IMaintenanceOptions
        {
            Func<SLMConfig, bool> wrapper = (config) =>
            {
                // It is because of config.BackUpOptions that we cannot generalize Handlers.
                // Removing the need to reference a specific property in config here would
                // allow for generalizing handlers and reduced code duplication
                if (config.BackUpOptions is TOptionType opts)
                {
                    action(config, opts);
                    return true;
                }

                return false;
            };

            return strategies.TryAdd(condition, wrapper);
        }

        public void Handle(SLMConfig config, IBackUpStrategy backUpStrategy)
        {
            strategies.TryGetValue(config.BackUpCondition, out var action);

            if (action is not null)
            {
                Console.WriteLine($"Back Up Handler found for {config.BackUpCondition}");
                bool shouldBackUp = action(config);
            }
        }
    }
}
