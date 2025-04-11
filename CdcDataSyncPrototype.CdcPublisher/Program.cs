using CdcDataSyncPrototype.CdcPublisher;
using CdcDataSyncPrototype.CdcPublisher.Infrastructure;
using CdcDataSyncPrototype.CdcPublisher.Services;

var builder = Host.CreateApplicationBuilder(args);


builder.Services.AddSingleton<ILsnTracker>(sp =>
    new LsnTracker(sp.GetRequiredService<IConfiguration>().GetConnectionString("DefaultConnection")!));

builder.Services.AddSingleton<IAzureServiceBusPublisher, AzureServiceBusPublisher>();

builder.Services.AddHostedService<CdcSyncWorker>();

var host = builder.Build();
host.Run();
