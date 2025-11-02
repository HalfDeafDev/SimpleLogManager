using SimpleLogManager.Types;

namespace SimpleLogManager.ConfigOptions
{
    public record IntervalConditionOptions(
        double Interval,
        IntervalType IntervalType
    ) : IMaintenanceOptions;
}
