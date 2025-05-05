using Azure.Messaging.ServiceBus;

Console.WriteLine("Starting Mock Receiver...");

// Required connection string
string connectionString = Environment.GetEnvironmentVariable("SERVICEBUS_CONNECTION")
                          ?? throw new InvalidOperationException("Missing SERVICEBUS_CONNECTION env var");

// Optional topic/subscription — fall back to defaults
string topicName = Environment.GetEnvironmentVariable("SERVICEBUS_TOPIC") ?? "cdc-updates";
string subscriptionName = Environment.GetEnvironmentVariable("SERVICEBUS_SUBSCRIPTION") ?? "mock-subscriber";

Console.WriteLine($"Listening on topic '{topicName}' and subscription '{subscriptionName}'");

var client = new ServiceBusClient(connectionString);
var processor = client.CreateProcessor(topicName, subscriptionName, new ServiceBusProcessorOptions());

processor.ProcessMessageAsync += async args =>
{
    var body = args.Message.Body.ToString();
    Console.WriteLine($"Received message: {body}");

    await Task.Delay(500); // Simulated work
    await args.CompleteMessageAsync(args.Message);
};

processor.ProcessErrorAsync += args =>
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"Error: {args.Exception.Message}");
    Console.ResetColor();
    return Task.CompletedTask;
};

await processor.StartProcessingAsync();

Console.WriteLine("Listening for messages... Press Enter to exit.");
Console.ReadLine();

await processor.StopProcessingAsync();
await processor.DisposeAsync();
await client.DisposeAsync();