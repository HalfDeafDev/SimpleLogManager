namespace SimpleLogManager.Types
{
    public record ConfigValues(
        int? MaxFileSize,
        ByteSize? MaxFileSizeUnit,
        int? MaxFolderSize,
        ByteSize? MaxFolderSizeUnit,
        int? NumOfLogs,
        int? Interval,
        IntervalType? IntervalType,
        ByteSizeType ByteSizeType = ByteSizeType.Decimal
    );
}
