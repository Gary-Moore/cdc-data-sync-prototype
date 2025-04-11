public interface IAzureServiceBusPublisher
{
    Task PublishMessageAsync(string message, string subject, CancellationToken cancellationToken = default);
}