using System.Runtime.CompilerServices;
using Azure.Messaging.ServiceBus;

public class AzureServiceBusPublisher : IAzureServiceBusPublisher
{
    private readonly ServiceBusClient _client;
    private readonly ServiceBusSender _sender;
    private readonly ILogger<AzureServiceBusPublisher> _logger;

    public AzureServiceBusPublisher(IConfiguration configuration, ILogger<AzureServiceBusPublisher> logger)
    {
        var connectionString = configuration.GetConnectionString("ServiceBus");
        var topicName = configuration["ServiceBus:TopicName"];

        _client = new ServiceBusClient(connectionString);
        _sender = _client.CreateSender(topicName);
        _logger = logger;
    }

    public async Task PublishMessageAsync(string messageBody, string subject, CancellationToken cancellationToken = default)
    {
        try
        {
            var serviceBusMessage = new ServiceBusMessage(messageBody);
            
            if (!string.IsNullOrWhiteSpace(subject))
            {
                serviceBusMessage.Subject = subject;
            }
           

            await _sender.SendMessageAsync(serviceBusMessage, cancellationToken);

            _logger.LogInformation("Published message to Service Bus: {Subject}", subject ?? "<no subject>");
        }
        catch (Exception ex)
        {
             _logger.LogError(ex, "Failed to publish message to Service Bus.");
            throw;
        }
    }
}