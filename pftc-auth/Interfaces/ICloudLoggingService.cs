namespace pftc_auth.Interfaces;

// Google Cloud Logging severities (in increasing order of severity):
//   DEBUG -> INFO -> NOTICE -> WARNING -> ERROR -> CRITICAL -> ALERT -> EMERGENCY
//
// Notes:
// - The Microsoft `LogLevel` enum (Trace, Debug, Information, Warning, Error, Critical)
//   maps approximately as follows:
//     Trace/Debug  -> DEBUG
//     Information  -> INFO
//     Warning      -> WARNING
//     Error        -> ERROR
//     Critical     -> CRITICAL
// - Google Cloud has additional severities (NOTICE, ALERT, EMERGENCY) that do not have
//   direct equivalents in `LogLevel`. When forwarding logs to Google Cloud, choose the
//   closest matching `LogLevel` or add metadata if you need the finer-grained severity.

public interface ICloudLoggingService
{
    Task LogDebugAsync(string message, Dictionary<string, string>? metadata = null);
    Task LogInfoAsync(string message, Dictionary<string, string>? metadata = null);
    Task LogWarningAsync(string message, Dictionary<string, string>? metadata = null);
    Task LogErrorAsync(string message, Dictionary<string, string>? metadata = null);
    Task LogCriticalAsync(string message, Dictionary<string, string>? metadata = null);
    
    void SetLogLevel(LogLevel minimumLogLevel);
    LogLevel GetCurrentLogLevel();
    
    
}