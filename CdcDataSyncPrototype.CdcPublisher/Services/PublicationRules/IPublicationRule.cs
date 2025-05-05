using CdcDataSyncPrototype.Core.Models;

namespace CdcDataSyncPrototype.CdcPublisher.Services.PublicationRules;

public interface IPublicationRule
{
    PublicationStagingEntry? Apply(PublicationStagingEntry change);
}