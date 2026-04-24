using System.Text.Json;
using Google.Cloud.SecretManager.V1;
using pftc_auth.Interfaces;

namespace pftc_auth.Services;

public class GoogleSecretManagerService : IGoogleSecretManagerService
{
    
    private readonly SecretManagerServiceClient _client;
    private readonly ILogger<GoogleSecretManagerService> _logger;
    private readonly string _projectId;
    
    public GoogleSecretManagerService(string projectId, ILogger<GoogleSecretManagerService> logger)
    {
        _logger = logger;
        _projectId = projectId;
        _client = SecretManagerServiceClient.Create();
    }

    public async Task<string> GetSecretAsync(string secretName)
    {
        try
        {
            var secretVersionName = new SecretVersionName(_projectId, secretName, "latest");
            var result = await _client.AccessSecretVersionAsync(secretVersionName);

            _logger.LogInformation($"Successfully loaded secret {secretName} from Google Secret Manager");
            return result.Payload.Data.ToStringUtf8();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw;
        }
    }

    public async Task LoadSecretsIntoConfigurationAsync(IConfiguration config)
    {
        _logger.LogInformation("Loading secrets from Google Secret Manager..");

        try
        {
            var googleClientJson = await GetSecretAsync("oauth");
            var redisConnectionString = await GetSecretAsync("redis");
            var jsonDoc = JsonDocument.Parse(googleClientJson);
            var web = jsonDoc.RootElement.GetProperty("web");

            var secrets = new Dictionary<string,string>
            {
                {"Authentication:Google:ClientId", web.GetProperty("client_id").GetString()!},
                {"Authentication:Google:ClientSecret", web.GetProperty("client_secret").GetString()!},
                {"Authentication:Redis:ConnectionString", redisConnectionString},
            };
            
            foreach (var secret in secrets)
            {
                config[secret.Key] = secret.Value;
                _logger.LogInformation($"Loaded secret {secret.Key} into configuration");
            }
            _logger.LogInformation("Configuration secrets set successfully");
        }
        catch (Exception e)
        {
            _logger.LogError($"Failed to load secrets into configuration. Error: {e.Message}");
        }
    }
}