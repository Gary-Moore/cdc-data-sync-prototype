using CdcDataSyncPrototype.CdcReceiver;
using CdcDataSyncPrototype.CdcReceiver.Configuration;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<ReceiverOptions>(builder.Configuration.GetSection("Receiver"));

builder.Services.AddHostedService<ServiceBusReceiverWorker>();
builder.Services.AddHostedService<InboxProcessor>();
builder.Services.AddHostedService<RetryInboxProcessor>();

var host = builder.Build();
host.Run();
