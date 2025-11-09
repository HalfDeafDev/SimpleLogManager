/*
 {
    "MaxBackUpFiles": 2,
    "BackUpDirectory": "C:\Path\To\Dir",
    "TargetFile": "C:\Path\To\File.txt",
    "MaintenancePlan": {
        "Condition": "FileSize",
        "MaxFileSize": 20,
        "MaxFileSizeUnit": "MB"
    },
    "BackUpPlan": {
        "Condition": "CreationDate",
        "Interval": 3,
        "IntervalUnit": "Day"
    }
}
*/

namespace SimpleLogManager.Configs
{
    public record MaintenancePlan(
        
    );

    public record BackUpPlan(
        
    );

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
        string? ByteSizeType = null,
        string? StartMessage = null
    );
}
