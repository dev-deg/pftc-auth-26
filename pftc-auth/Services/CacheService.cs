using pftc_auth.Interfaces;
using StackExchange.Redis;

namespace pftc_auth.Services;

public class CacheService : ICacheService, IDisposable
{
    private readonly ILogger<CacheService> _logger;
    private readonly ConnectionMultiplexer _connectionMultiplexer;
    private readonly IDatabase? _db;
    
    public CacheService(IConfiguration config, ILogger<CacheService> logger)
    {
        _logger = logger;
        try
        {
            string? connectionString = config["Authentication:Redis:ConnectionString"];
            _connectionMultiplexer = ConnectionMultiplexer.Connect(connectionString);
            _db = _connectionMultiplexer?.GetDatabase();
            _logger.LogInformation("Cache service initialized");
        }
        catch (Exception e)
        {
            _logger.LogError("Unable to connect to Redis. Error {}", e.Message);
            throw new Exception("Unable to connect to Redis", e);
        }

    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public async Task<string> GetAsync(string key)
    {
        return await _db.StringGetAsync(key);
        _logger.LogDebug("Loading from cache");
    }

    public async Task SetAsync(string key, string value, TimeSpan? expiration = null)
    {
        await _db .StringSetAsync(key, value);
        _logger.LogDebug("Cache service set");
    }

    public async Task DeleteAsync(string key)
    {
        await _db.KeyDeleteAsync(key);
        _logger.LogDebug("Cache service deleted");
    }
}