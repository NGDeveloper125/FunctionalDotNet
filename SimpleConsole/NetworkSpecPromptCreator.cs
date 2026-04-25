using FunctionalDotNet;

namespace SimpleConsole;

internal static class NetworkSpecPromptCreator
{
    private const string Preamble =
        "The following is the network configuration of my Windows machine, " +
        "collected via PowerShell. Please review it and answer the questions " +
        "I add at the end.";

    // See HardwareSpecPromptCreator.Sections for the rationale on the
    // (title, command) shape — same idea here.
    //
    // The "Public IP" section deliberately wraps its Invoke-WebRequest in
    // a PowerShell try/catch so that an offline run produces a friendly
    // string ("unreachable: ...") inside Success, rather than a non-zero
    // exit that would surface as an inline failure note. Either rendering
    // is fine, but treating "no internet" as a normal data point keeps the
    // prompt readable.
    private static readonly (string Title, string Command)[] Sections =
    {
        ("Host Identity",
            "Get-CimInstance Win32_ComputerSystem | " +
            "Select-Object Name, Domain, Workgroup, PartOfDomain | " +
            "Format-List | Out-String -Width 4096"),

        ("Active Network Adapters",
            "Get-NetAdapter | Where-Object { $_.Status -eq 'Up' } | " +
            "Select-Object Name, InterfaceDescription, Status, LinkSpeed, " +
            "MacAddress, MediaType, MediaConnectionState | " +
            "Format-List | Out-String -Width 4096"),

        ("IPv4 Addresses",
            "Get-NetIPAddress -AddressFamily IPv4 | " +
            "Where-Object { $_.PrefixOrigin -ne 'WellKnown' } | " +
            "Select-Object InterfaceAlias, IPAddress, PrefixLength, AddressState, PrefixOrigin | " +
            "Format-Table -AutoSize | Out-String -Width 4096"),

        ("IPv6 Addresses",
            "Get-NetIPAddress -AddressFamily IPv6 | " +
            "Where-Object { $_.PrefixOrigin -ne 'WellKnown' } | " +
            "Select-Object InterfaceAlias, IPAddress, PrefixLength, AddressState | " +
            "Format-Table -AutoSize | Out-String -Width 4096"),

        ("DNS Servers",
            "Get-DnsClientServerAddress | " +
            "Where-Object { $_.ServerAddresses } | " +
            "Select-Object InterfaceAlias, AddressFamily, " +
            "@{n='Servers';e={ $_.ServerAddresses -join ', ' }} | " +
            "Format-Table -AutoSize | Out-String -Width 4096"),

        ("Default Routes (IPv4)",
            "Get-NetRoute -DestinationPrefix '0.0.0.0/0' -ErrorAction SilentlyContinue | " +
            "Select-Object InterfaceAlias, NextHop, RouteMetric, InterfaceMetric | " +
            "Format-Table -AutoSize | Out-String -Width 4096"),

        // We deliberately do NOT use `netsh wlan show interfaces` here:
        // on Windows 11 it requires Location permission or admin
        // elevation (Wi-Fi SSID is treated as location data) and exits
        // non-zero otherwise. Get-NetConnectionProfile gives the useful
        // bits of the same picture — what network we're on, what category
        // it's classified as, and whether IPv4/IPv6 actually reach the
        // internet — without the privacy-prompt baggage.
        ("Connection Profile",
            "Get-NetConnectionProfile | " +
            "Select-Object Name, InterfaceAlias, NetworkCategory, " +
            "IPv4Connectivity, IPv6Connectivity, DomainAuthenticationKind | " +
            "Format-List | Out-String -Width 4096"),

        ("Public IP",
            "try { (Invoke-WebRequest -Uri 'https://api.ipify.org' " +
            "-UseBasicParsing -TimeoutSec 5).Content } " +
            "catch { 'unreachable: ' + $_.Exception.Message }"),
    };

    public static Result<string, string> CreatePrompt() =>
        new Result<string, string>.Success(
            PromptSection.Render(Sections, Preamble, "# Network Specification"));
}
