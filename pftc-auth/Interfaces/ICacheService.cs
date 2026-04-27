namespace pftc_auth.Interfaces;

public interface ICacheService
{
    /// <summary>Returns the raw string value for the given key, or null if not found.</summary>
    Task<string?> GetAsync(string key);

    /// <summary>Deserialises a JSON-encoded value back to type <typeparamref name="T"/>.</summary>
    Task<T?> GetAsync<T>(string key);

    /// <summary>Stores a raw string value with an optional TTL.</summary>
    Task SetAsync(string key, string value, TimeSpan? expiration = null);

    /// <summary>Serialises <paramref name="value"/> as JSON and stores it with an optional TTL.</summary>
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);

    /// <summary>Removes the key from the cache.</summary>
    Task DeleteAsync(string key);

    /// <summary>Returns true if the key exists in the cache.</summary>
    Task<bool> ExistsAsync(string key);

    /// <summary>Atomically increments a counter (e.g. post likes) and returns the new value.</summary>
    Task<long> IncrementAsync(string key, long delta = 1);
}