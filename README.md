# The Simple Log Manager

The Simple Log Manger is a pet project that comes from my need to manage logs that would be unmanaged otherwise.

## Using The Simple Log Manager

The original intent was to have a scheduled task setup to run TSLM and that is how I use it today. Inherently, you can run it manually whenever you want, but I would like to move the flow to have various options for using TSLM.

## Configuration of Application

Configuration is done through a `config.json` file that is in the same directory as the executable.

Below is an example of the current iteration of the config file, expect it to change. Soon.

```
[
    {
        "LogFilePath": "C:\Path\To\Log\File",
        "BackUpDirectory": "C:\Path\To\Backups\",
        "BackUpCondition": "FileSize",
        "MaxFileSize": "10",
        "MaxFileSizeUnit": "MB",
        "MaintenanceCondition": "LogCount",
        "NumOfLogs": "3"
    }
]
```

Note: More than one configuration can exist in the `config.json`, the only caveat is that all configs are executed sequentially whenever the program is ran.

Below are the options available to you

| Option | For | Required | Default | Values | Description |
| ------- | --- | -------- | -------- | ------- | ----------- |
| LogFilePath | - | Yes | None | Full & Relative Paths | The path of the log file |
| BackUpDirectory | Back Up | Yes | None | Full & Relative Paths | The path of the directory back-up should be stored in |
| BackUpCondition | Back Up | Yes | None | `FileSize`, `LastModDate`, `CreationDate`, `None` | The condition that should cause a back up to occur |
| MaintenanceCondition | Maintenance | No | Null | `LogCount`, `FolderSize`, `CreationDate`, `None` | The condition in which maintenance on back ups should occur |
| SupressMaintenance | Maintenance | No | Null | `True`, `False` | Whether or not to suppress maintenance task |
| MaxFileSize | Back Up | No | Null | Integer | The maximum size of the log file before being backed up |
| MaxFileSizeUnit | Back Up | No | Null | `B`, `KB`, `MB`, `GB` | The unit associated with the Max File Size value |
| MaxFolderSize | Maintenance | No | Null | Integer | The maximum size of the back up directory before purging the oldest backup |
| MaxFolderSizeUnit | Maintenance | No | Null | `B`, `KB`, `MB`, `GB` | The unit associated with the Max Folder Size value |
| NumOfLogs | Maintenance | No | Null | Integer | The maximum number of logs before purging the oldest backup |
| Interval | Back Up | No | Null | Integer | The number of days, weeks, months, etc. before backing up log file |
| IntervalType | Back Up | No | Null | `Seconds`, `Minutes`, `Hours`, `Days`, `Months`, `Years` | The unit associated with the Interval Back Up Condition |
| ByteSizeType | - | No | Binary | `Binary`, `Decimal` | Whether 1000B = 1KB (Decimal) or 1024B = 1KB (Binary) |
| StartMessage | - | No | "<<$CurrentDateTime>>" | Formatted String | The message that is added to the log file at the start of a run |
| WriteStartMessage | - | No | Boolean Values | `True`, `T`, `Yes`, `Y`, `False`, `F`, `No`, `N` | Whether or not to write the Start Message |
| EndMessage | - | No | "" | Formatted String | The message that is added to the log file at the end of the run |
| WriteEndMessage | - | No | "" | `True`, `T`, `Yes`, `Y`, `False`, `F`, `No`, `N` | Whether or not to write the End Message |


### Formatted Messages

When crafting the Start and End Messages, there are some variables you can call upon.

|Variable|Description|
|-|-|
|`$CurrentDateTime`|The date and time that the configuration was executed|
|`$CurrentDate`|Only the date of `$CurrentDateTime`|
|`$CurrentTime`|Only the time of `$CurrentDateTime`|
|`$FileName`|The file name of the log file, with the extension|
|`$FilePath`|The full file path of the log file|


## TODO

This is not comprehensive list, in some ways they are features that are out of current scope that I don't want to forget about.

- [ ] CLI Flags
	- [ ] Config File Target (`--ConfigPath`)
- [ ] Config Options
    - [ ] Schedulable Config Execution
    - [ ] "No Backup" Option making maintenance rules impact the log file itself
    - [ ] "No Maintenance" Option to allow for back ups to continue to accrue
- [ ] TSLM as a Task
    - [ ] CLI Flag to target specific config to run
