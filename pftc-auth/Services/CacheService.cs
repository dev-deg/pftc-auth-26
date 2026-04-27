using System.Text.Json;
using pftc_auth.Interfaces;
using StackExchange.Redis;

namespace pftc_auth.Services;

public class CacheService : ICacheService, IDisposable
{
    private readonly ILogger<CacheService> _logger;
    private readonly ConnectionMultiplexer? _connectionMultiplexer;
    private readonly IDatabase? _db;

    private bool _disposed;

    public CacheService(IConfiguration config, ILogger<CacheService> logger)
    {
        _logger = logger;
        try
        {
            string? connectionString = config["Authentication:Redis:ConnectionString"];
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("Redis connection string is not configured.");

            _connectionMultiplexer = ConnectionMultiplexer.Connect(connectionString);
            _db = _connectionMultiplexer.GetDatabase();
            _logger.LogInformation("Cache service initialised");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to connect to Redis: {Message}", e.Message);
            throw new InvalidOperationException("Unable to connect to Redis", e);
        }
    }

    // -------------------------------------------------------------------------
    // String overloads
    // -------------------------------------------------------------------------

    public async Task<string?> GetAsync(string key)
    {
        EnsureConnected();
        try
        {
            RedisValue value = await _db!.StringGetAsync(key);
            _logger.LogDebug("Cache GET {Key}: {Hit}", key, value.HasValue ? "HIT" : "MISS");
            return value.HasValue ? (string?)value : null;
        }
        catch (RedisException ex)
        {
            _logger.LogError(ex, "Redis error on GET {Key}", key);
            return null;   // degrade gracefully – callers should fall back to the database
        }
    }

    public async Task SetAsync(string key, string value, TimeSpan? expiration = null)
    {
        EnsureConnected();
        try
        {
            Expiration exp = expiration.HasValue ? new Expiration(expiration.Value) : default(Expiration);
            await _db!.StringSetAsync(key, value, exp);
            _logger.LogDebug("Cache SET {Key} (TTL={TTL})", key, expiration?.ToString() ?? "none");
        }
        catch (RedisException ex)
        {
            _logger.LogError(ex, "Redis error on SET {Key}", key);
        }
    }

    // -------------------------------------------------------------------------
    // Typed (JSON) overloads
    // -------------------------------------------------------------------------

    public async Task<T?> GetAsync<T>(string key)
    {
        string? json = await GetAsync(key);
        if (string.IsNullOrEmpty(json))
            return default;

        try
        {
            return JsonSerializer.Deserialize<T>(json);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialise cached value for key {Key} as {Type}", key, typeof(T).Name);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        try
        {
            string json = JsonSerializer.Serialize(value);
            await SetAsync(key, json, expiration);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to serialise value for key {Key} as {Type}", key, typeof(T).Name);
        }
    }

    // -------------------------------------------------------------------------
    // Additional helpers
    // -------------------------------------------------------------------------

    public async Task DeleteAsync(string key)
    {
        EnsureConnected();
        try
        {
            bool deleted = await _db!.KeyDeleteAsync(key);
            _logger.LogDebug("Cache DELETE {Key}: {Result}", key, deleted ? "removed" : "not found");
        }
        catch (RedisException ex)
        {
            _logger.LogError(ex, "Redis error on DELETE {Key}", key);
        }
    }

    public async Task<bool> ExistsAsync(string key)
    {
        EnsureConnected();
        try
        {
            return await _db!.KeyExistsAsync(key);
        }
        catch (RedisException ex)
        {
            _logger.LogError(ex, "Redis error on EXISTS {Key}", key);
            return false;
        }
    }

    /// <summary>
    /// Atomically increments a numeric counter (e.g. post like counts).
    /// The key is created with a value of <paramref name="delta"/> if it does not exist.
    /// </summary>
    public async Task<long> IncrementAsync(string key, long delta = 1)
    {
        EnsureConnected();
        try
        {
            long result = await _db!.StringIncrementAsync(key, delta);
            _logger.LogDebug("Cache INCREMENT {Key} by {Delta} → {Value}", key, delta, result);
            return result;
        }
        catch (RedisException ex)
        {
            _logger.LogError(ex, "Redis error on INCREMENT {Key}", key);
            return 0;
        }
    }

    // -------------------------------------------------------------------------
    // Lifecycle
    // -------------------------------------------------------------------------

    private void EnsureConnected()
    {
        if (_db is null)
            throw new InvalidOperationException("Cache service is not connected to Redis.");
    }

    public void Dispose()
    {
        if (_disposed) return;
        _connectionMultiplexer?.Dispose();
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}