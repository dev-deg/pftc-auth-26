using pftc_auth.Interfaces;
using Google.Cloud.Logging.V2;

namespace pftc_auth.Services;

public class CloudLoggingService : ICloudLoggingService
{
    private readonly ILogger<CloudLoggingService> _cloudLogger;
    private readonly IHostEnvironment _hostEnvironment; //Feel free to use this to enhance the logging experience
    private readonly string _projectId;
    private readonly string _logName;
    private readonly LoggingServiceV2Client _loggingClient;
    private bool _isInitialized;
    private LogLevel _currentLogLevel = LogLevel.Information; // Default log level
    
    public CloudLoggingService(IConfiguration config, ILogger<CloudLoggingService> cloudLogger, IHostEnvironment env)
    {
        _cloudLogger = cloudLogger;
        _hostEnvironment = env;
        _projectId = config["Logging:Google:ProjectId"] ?? throw new ArgumentNullException("Logging:Google:ProjectId");
        _logName = config["Logging:Google:LogName"] ?? throw new ArgumentNullException("Logging:Google:LogName");
        try
        {
            _loggingClient = LoggingServiceV2Client.Create();
            _isInitialized = true;
            _cloudLogger.LogInformation("Cloud Logging service initialized");
            Console.WriteLine("CloudLoggingService initialized with project: {0}, log name: {1}",
                _projectId, _logName);
        }
        catch (Exception e)
        {
            _isInitialized = false;
            Console.WriteLine("Failed to initialize Cloud Logging client: {0}", e.Message);
        }
    }

    private void LogToConsole(string level, string message, Dictionary<string, string>? metadata)
    {
        var metadataString = metadata != null ? string.Join(", ", metadata.Select(kv => $"{kv.Key}: {kv.Value}")) : "No metadata";
        Console.WriteLine("[{0}] {1} - Metadata: {2}", level, message, metadataString);
    }

    public async Task LogDebugAsync(string message, Dictionary<string, string>? metadata = null)
    {
        if (_isInitialized)
        {
            try
            {
                // Implement Google Cloud Logging logic here
                await Task.CompletedTask; // Placeholder for actual logging logic
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to log debug message: {0}", e.Message);
                LogToConsole("DEBUG", message, metadata);
            }
        }
        else
        {
            LogToConsole("DEBUG", message, metadata);
        }
    }

    public async Task LogInfoAsync(string message, Dictionary<string, string>? metadata = null)
    {
        if (_isInitialized)
        {
            try
            {
                // Implement Google Cloud Logging logic here
                await Task.CompletedTask; // Placeholder for actual logging logic
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to log info message: {0}", e.Message);
                LogToConsole("INFO", message, metadata);
            }
        }
        else
        {
            LogToConsole("INFO", message, metadata);
        }
    }

    public async Task LogWarningAsync(string message, Dictionary<string, string>? metadata = null)
    {
        if (_isInitialized)
        {
            try
            {
                // Implement Google Cloud Logging logic here
                await Task.CompletedTask; // Placeholder for actual logging logic
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to log warning message: {0}", e.Message);
                LogToConsole("WARNING", message, metadata);
            }
        }
        else
        {
            LogToConsole("WARNING", message, metadata);
        }
    }

    public async Task LogErrorAsync(string message, Dictionary<string, string>? metadata = null)
    {
        if (_isInitialized)
        {
            try
            {
                // Implement Google Cloud Logging logic here
                await Task.CompletedTask; // Placeholder for actual logging logic
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to log error message: {0}", e.Message);
                LogToConsole("ERROR", message, metadata);
            }
        }
        else
        {
            LogToConsole("ERROR", message, metadata);
        }
    }

    public async Task LogCriticalAsync(string message, Dictionary<string, string>? metadata = null)
    {
        if (_isInitialized)
        {
            try
            {
                // Implement Google Cloud Logging logic here
                await Task.CompletedTask; // Placeholder for actual logging logic
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to log critical message: {0}", e.Message);
                LogToConsole("CRITICAL", message, metadata);
            }
        }
        else
        {
            LogToConsole("CRITICAL", message, metadata);
        }
    }

    public void SetLogLevel(LogLevel minimumLogLevel)
    {
        _currentLogLevel = minimumLogLevel;
        _cloudLogger.LogInformation("Log level set to: {LogLevel}", minimumLogLevel);
        Console.WriteLine("Log level set to: {0}", minimumLogLevel);
    }

    public LogLevel GetCurrentLogLevel()
    {
        _cloudLogger.LogInformation("Current log level retrieved: {LogLevel}", _currentLogLevel);
        Console.WriteLine("Current log level retrieved: {0}", _currentLogLevel);
        return _currentLogLevel;
    }
}