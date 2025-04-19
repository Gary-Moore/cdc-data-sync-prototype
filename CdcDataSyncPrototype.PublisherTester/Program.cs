using System.Data.Common;
using Azure.Messaging.ServiceBus;
using CdcDataSyncPrototype.Core.Models;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddUserSecrets<Program>()
    .Build();

var connectionString = config["ServiceBus:ConnectionString"];
var topicName = config["ServiceBus:TopicName"];

var publication = new PublicationChange
{
    Id = Random.Shared.Next(1000, 9999),
    Title = "Test Publication",
    Type = "Report",
    PublishedDate = DateTime.UtcNow.Date,
    LastModified = DateTime.UtcNow,
    Operation = 2
};

var json = System.Text.Json.JsonSerializer.Serialize(publication);

var client = new ServiceBusClient(connectionString);
var sender = client.CreateSender(topicName);

var message = new ServiceBusMessage(json)
{
    MessageId = Guid.NewGuid().ToString(),
    Subject = nameof(PublicationChange)
};

await sender.SendMessageAsync(message);

Console.WriteLine($"✅ Test message sent: ID {publication.Id}, Title: {publication.Title}");