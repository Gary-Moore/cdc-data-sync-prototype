using CdcDataSyncPrototype.CdcReceiver;
using CdcDataSyncPrototype.CdcReceiver.Configuration;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<ReceiverOptions>(builder.Configuration.GetSection("Receiver"));

builder.Services.AddHostedService<Worker>();
builder.Services.AddHostedService<InboxProcessor>();

var host = builder.Build();
host.Run();
