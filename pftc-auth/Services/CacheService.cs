using pftc_auth.Interfaces;

namespace pftc_auth.Services;

public class CacheService : ICacheService, IDisposable
{
    private readonly ILogger<CacheService> _logger;
    
    public CacheService(ILogger<CacheService> logger)
    {
        _logger = logger;
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public Task<string> GetAsync(string key)
    {
        throw new NotImplementedException();
    }

    public Task SetAsync(string key, string value, TimeSpan? expiration = null)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(string key)
    {
        throw new NotImplementedException();
    }
}