using CdcDataSyncPrototype.CdcPublisher;
using CdcDataSyncPrototype.CdcPublisher.Infrastructure;
using CdcDataSyncPrototype.CdcPublisher.Services;
using CdcDataSyncPrototype.CdcPublisher.Services.PublicationRules;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables();


builder.Services.AddSingleton<ILsnTracker>(sp =>
    new LsnTracker(sp.GetRequiredService<IConfiguration>().GetConnectionString("SyncDb")!));

builder.Services.AddSingleton<IAzureServiceBusPublisher, AzureServiceBusPublisher>();
builder.Services.AddSingleton<IPublicationChangeRulesEngine, PublicationChangeRulesEngine>();
builder.Services.AddSingleton<IAuditLogger, AuditLogger>();

builder.Services.AddTransient<IPublicationRule, SuppressInternalOnlyRule>();
builder.Services.AddTransient<IPublicationRule, PublishWindowRule>();
builder.Services.AddSingleton<IPublicationChangeRulesEngine, PublicationChangeRulesEngine>();


builder.Services.AddHostedService<CdcSyncWorker>();

var host = builder.Build();
host.Run();
