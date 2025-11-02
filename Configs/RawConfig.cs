namespace SimpleLogManager.Configs
{
    public record RawConfig(
        string LogFilePath,
        string BackUpDirectory,
        string BackUpCondition,
        string? MaintenanceCondition = null,
        bool? SupressMaintenance = null,
        int? MaxFileSize = null,
        string? MaxFileSizeUnit = null,
        int? MaxFolderSize = null,
        string? MaxFolderSizeUnit = null,
        int? NumOfLogs = null,
        int? Interval = null,
        string? IntervalType = null,
        string? ByteSizeType = null
    );
}
