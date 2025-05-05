using CdcDataSyncPrototype.Core.Models;

public interface IPublicationChangeRulesEngine
{
    PublicationStagingEntry? Apply(PublicationStagingEntry change);
}
