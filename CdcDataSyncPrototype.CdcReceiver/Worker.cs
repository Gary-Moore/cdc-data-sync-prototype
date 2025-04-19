using System.Text.Json;
using Azure.Messaging.ServiceBus;
using CdcDataSyncPrototype.CdcReceiver.Configuration;
using CdcDataSyncPrototype.Core.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace CdcDataSyncPrototype.CdcReceiver;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly ReceiverOptions _options;
    private ServiceBusClient _client;
    private ServiceBusProcessor _processor;
    private readonly IConfiguration _configuration;


    public Worker(ILogger<Worker> logger, IOptions<ReceiverOptions> options, IConfiguration configuration)
    {
        _logger = logger;
        _options = options.Value;
        _configuration = configuration;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _client = new ServiceBusClient(_options.ServiceBusConnectionString);

        _processor = _client.CreateProcessor(
            topicName: _options.TopicName, 
            subscriptionName: _options.SubscriptionName, 
            new ServiceBusProcessorOptions());

        _processor.ProcessMessageAsync += HandleMessageAsync;
        _processor.ProcessErrorAsync += HandleErrorAsync;   

        await _processor.StartProcessingAsync(cancellationToken);

        _logger.LogInformation("Receiver started and listening on {Topic}/{Subscription}",
            _options.TopicName, _options.SubscriptionName);
    }

    private async Task HandleMessageAsync(ProcessMessageEventArgs args)
    {
        // Process the message
        var body = args.Message.Body.ToString();
        var messageId = args.Message.MessageId;
        var messageType = args.Message.Subject ?? "PublicationChange";

        try
        {
            var inboxEntry = new SyncInboxEntry
            {
                Id = Guid.NewGuid(),
                MessageId = messageId,
                MessageType = messageType,
                Payload = body,
                ReceivedAt = DateTime.UtcNow,
                Processed = false,
            };

            var connStr = _configuration.GetConnectionString("ReceiverDB");

            const string sql = @"
                INSERT INTO dbo.SyncInbox (Id, MessageId, MessageType, Payload, ReceivedAt, Processed)
                VALUES (@Id, @MessageId, @MessageType, @Payload, @ReceivedAt, @Processed);
            ";

            using var connection = new SqlConnection(connStr);
            await connection.ExecuteAsync(sql, inboxEntry);

            _logger.LogInformation("Stored message in SyncInbox: {MessageId}", messageId);

            await args.CompleteMessageAsync(args.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log message to SyncInbox: {Body}", body);
            // Optionally: args.DeadLetterMessageAsync(args.Message);
        }
    }

    private Task HandleErrorAsync(ProcessErrorEventArgs args)
    {
        
        _logger.LogError(args.Exception, "Error processing message");

        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_processor != null)
        {
            await _processor.StopProcessingAsync(cancellationToken);
            await _processor.DisposeAsync();
        }
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken) => Task.CompletedTask;
}
