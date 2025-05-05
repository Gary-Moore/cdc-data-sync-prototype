using CdcDataSyncPrototype.Core.Models;

namespace CdcDataSyncPrototype.CdcPublisher.Services.PublicationRules;

public class SuppressInternalOnlyRule : IPublicationRule
{
    public PublicationStagingEntry? Apply(PublicationStagingEntry change) => change.InternalOnly ? null : change;
}