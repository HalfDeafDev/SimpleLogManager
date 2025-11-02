using SimpleLogManager.Configs;
using SimpleLogManager.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleLogManager.ConfigOptions
{
    public class SLMConfigOptionsFactory
    {
        public delegate ConfigCreationResult<IMaintenanceOptions> SLMConfigStrategy(RawConfig rawConfig);
        private Dictionary<Type, SLMConfigStrategy> strategies = new();

        public TOptions? TryCreate<TOptions>(RawConfig config)
            where TOptions : class, IMaintenanceOptions
        {
            if(strategies.TryGetValue(typeof(TOptions), out var fn))
            {
                ConfigCreationResult<IMaintenanceOptions> result = fn(config);

                if (result.Options is not null)
                {
                    return (TOptions)result.Options;
                } else
                {
                    Console.WriteLine(result.exceptionMessage);
                }
            }

            return null;
        }

        public bool Add<TOptions>(SLMConfigStrategy strategy)
            where TOptions : IMaintenanceOptions
        {
            return strategies.TryAdd(typeof(TOptions), strategy);
        }

        public void Replace<TOptions>(SLMConfigStrategy strategy)
            where TOptions : IMaintenanceOptions
        {
            Type tOptions = typeof(TOptions);
            if (strategies.ContainsKey(tOptions))
            {
                strategies.Remove(tOptions);
            }

            Add<TOptions>(strategy);
        }
    }
}
