namespace CdcDataSyncPrototype.CdcReceiver.Configuration;

public class ReceiverOptions
{
    public string ServiceBusConnectionString { get; set; }
    public string SubscriptionName { get; set; }
    public string TopicName { get; set; }
}