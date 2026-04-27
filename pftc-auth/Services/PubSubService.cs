using Google.Cloud.PubSub.V1;
using Google.Protobuf;

namespace pftc_auth.Services;

public class PubSubService
{
        private readonly ILogger<PubSubService> _logger;
        private readonly TopicName _topic;
        
        public PubSubService(ILogger<PubSubService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _topic = TopicName.FromProjectTopic(configuration["PubSub:Google:ProjectId"]!, 
                configuration["PubSub:Google:TopicName"]!);
        }

        public async Task PublishMessageAsync(string message)
        {
            var publisher = await PublisherClient.CreateAsync(_topic);
            try
            {
                PubsubMessage pubSubMessage = new PubsubMessage
                {
                    Data = ByteString.CopyFromUtf8(message)
                };
                
                string messageId = await publisher.PublishAsync(pubSubMessage);
                _logger.LogInformation("Published message to Pub/Sub with ID: {MessageId}", messageId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to publish message to Pub/Sub: {Message}", e.Message);
                throw;
            }
            finally
            {
                await publisher.ShutdownAsync(TimeSpan.FromSeconds(5));
            }
        }
}