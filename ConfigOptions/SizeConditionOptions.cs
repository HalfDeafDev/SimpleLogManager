using SimpleLogManager.Types;

namespace SimpleLogManager.ConfigOptions
{
    public record SizeConditionOptions(
        double Size,
        ByteSize ByteSize,
        ByteSizeType ByteSizeType = ByteSizeType.Decimal
    ) : IMaintenanceOptions;
}
