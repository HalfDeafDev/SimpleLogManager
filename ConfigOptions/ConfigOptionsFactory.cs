using SimpleLogManager.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleLogManager.ConfigOptions
{
    /// <summary>
    /// This is where you would translate a raw setting into something concrete based on some type of condition.
    /// </summary>
    /// <typeparam name="TCondition">The type of conditions being considered.</typeparam>
    public class ConfigOptionsFactory<TCondition>
        where TCondition : notnull
    {
        private Dictionary<TCondition, Func<RawConfig, IMaintenanceOptions?>> strategies = new();

        public IMaintenanceOptions? Create(TCondition condition, RawConfig rawConfig)
        {
            strategies.TryGetValue(condition, out var strategy);

            if (strategy is not null)
            {
                return strategy(rawConfig);
            }

            return null;
        }

        public bool TryAdd(TCondition condition, Func<RawConfig, IMaintenanceOptions?> strategy)
        {
            strategies.Add(condition, strategy);
            return true;
        }
    }
}
