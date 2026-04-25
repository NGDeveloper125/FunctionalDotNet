using FunctionalDotNet;

namespace SimpleConsole;

internal static class HardwareSpecPromptCreator
{
    private const string Preamble =
        "The following is the hardware configuration of my Windows machine, " +
        "collected via PowerShell / WMI. Please review it and answer the " +
        "questions I add at the end.";

    // Each entry is a (title, PowerShell-command) pair. Treating the
    // sections as a piece of data — declared once, in one place — is the
    // shift that lets PromptCreator do its work in a single map-and-fold
    // expression, instead of ten copy-pasted branches.
    //
    // Each command ends with `Format-List`/`Format-Table | Out-String -Width 4096`
    // so the captured stdout is fully formatted by PowerShell's host
    // formatter and not truncated to the default 80-column width that
    // applies when output is redirected.
    private static readonly (string Title, string Command)[] Sections =
    {
        ("Operating System",
            "Get-CimInstance Win32_OperatingSystem | " +
            "Select-Object Caption, Version, BuildNumber, OSArchitecture, " +
            "InstallDate, LastBootUpTime | Format-List | Out-String -Width 4096"),

        ("Computer System",
            "Get-CimInstance Win32_ComputerSystem | " +
            "Select-Object Manufacturer, Model, SystemType, " +
            "@{n='TotalRAM_GB';e={[math]::Round($_.TotalPhysicalMemory/1GB, 2)}}, " +
            "NumberOfProcessors, NumberOfLogicalProcessors | " +
            "Format-List | Out-String -Width 4096"),

        ("Processor",
            "Get-CimInstance Win32_Processor | " +
            "Select-Object Name, Manufacturer, NumberOfCores, NumberOfLogicalProcessors, " +
            "MaxClockSpeed, L2CacheSize, L3CacheSize | " +
            "Format-List | Out-String -Width 4096"),

        ("Memory Modules",
            "Get-CimInstance Win32_PhysicalMemory | " +
            "Select-Object Manufacturer, PartNumber, " +
            "@{n='Capacity_GB';e={[math]::Round($_.Capacity/1GB,2)}}, " +
            "Speed, ConfiguredClockSpeed | " +
            "Format-Table -AutoSize | Out-String -Width 4096"),

        ("Graphics Adapters",
            "Get-CimInstance Win32_VideoController | " +
            "Select-Object Name, " +
            "@{n='VRAM_GB';e={[math]::Round($_.AdapterRAM/1GB,2)}}, " +
            "DriverVersion, VideoModeDescription, CurrentRefreshRate | " +
            "Format-List | Out-String -Width 4096"),

        ("Physical Disks",
            "Get-CimInstance Win32_DiskDrive | " +
            "Select-Object Model, InterfaceType, MediaType, " +
            "@{n='Size_GB';e={[math]::Round($_.Size/1GB,2)}} | " +
            "Format-Table -AutoSize | Out-String -Width 4096"),

        ("Logical Drives",
            "Get-CimInstance Win32_LogicalDisk -Filter 'DriveType=3' | " +
            "Select-Object DeviceID, VolumeName, FileSystem, " +
            "@{n='Size_GB';e={[math]::Round($_.Size/1GB,2)}}, " +
            "@{n='Free_GB';e={[math]::Round($_.FreeSpace/1GB,2)}} | " +
            "Format-Table -AutoSize | Out-String -Width 4096"),

        ("Motherboard",
            "Get-CimInstance Win32_BaseBoard | " +
            "Select-Object Manufacturer, Product, Version | " +
            "Format-List | Out-String -Width 4096"),

        ("BIOS / Firmware",
            "Get-CimInstance Win32_BIOS | " +
            "Select-Object Manufacturer, Name, Version, ReleaseDate, SMBIOSBIOSVersion | " +
            "Format-List | Out-String -Width 4096"),

        ("Battery",
            "Get-CimInstance Win32_Battery -ErrorAction SilentlyContinue | " +
            "Select-Object Name, EstimatedChargeRemaining, BatteryStatus, " +
            "DesignCapacity, FullChargeCapacity | " +
            "Format-List | Out-String -Width 4096"),
    };

    // The function is no longer pure — it shells out to PowerShell — but
    // the Result return type still earns its keep: it lets this composable
    // with the upstream Bind-chain in Program.cs without that pipeline
    // having to know whether the value was computed by a pure switch
    // expression or by ten subprocess calls.
    public static Result<string, string> CreatePrompt() =>
        new Result<string, string>.Success(
            PromptSection.Render(Sections, Preamble, "# Hardware Specification"));
}
