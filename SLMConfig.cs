using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SimpleLogManager
{
    internal enum BackUpCondition
    {
        FileSize,
        LastModDate,
        CreationDate
    }

    internal enum MaintenanceCondition
    {
        LogCount,
        FolderSize,
        CreationDate
    }

    internal enum IntervalType
    {
        Seconds,
        Minutes,
        Hours,
        Days,
        Weeks,
        Months,
        Years
    }

    internal enum ByteSize
    {
        Bit,
        Byte,
        KiloByte,
        MegaByte,
        GigaByte,
        TerraByte,
        PetaByte
    }

    //TODO: Implement
    internal enum ByteSizeType
    {
        Binary, // 1024 Bytes = 1 KB
        Decimal // 1000 Bytes = 1 KB
    }

    internal interface IMaintenanceOptions;

    internal record SizeConditionOptions(
        double Number,
        ByteSize ByteSize,
        ByteSizeType ByteSizeType = ByteSizeType.Decimal
    ) : IMaintenanceOptions;


    internal record IntervalConditionOptions(
        double Interval,
        IntervalType IntervalType
    ) : IMaintenanceOptions;

    internal record CountConditionOptions(
        int NumOfLogs
    ) : IMaintenanceOptions;
    
    internal record ConfigValues(
        int? MaxFileSize,
        ByteSize? MaxFileSizeUnit,
        int? MaxFolderSize,
        ByteSize? MaxFolderSizeUnit,
        int? NumOfLogs,
        int? Interval,
        IntervalType? IntervalType,
        ByteSizeType ByteSizeType = ByteSizeType.Decimal
    );

    internal record SLMConfig(
        FileInfo LogFileInfo,
        FileInfo BackUpDirectoryInfo,
        BackUpCondition BackUpCondition,
        MaintenanceCondition MaintenanceCondition,
        IMaintenanceOptions MaintenanceOptions
    );
}
