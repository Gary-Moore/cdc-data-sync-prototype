using CdcDataSyncPrototype.Core.Models;

namespace CdcDataSyncPrototype.CdcPublisher.Services.PublicationRules;
public class PublishWindowRule : IPublicationRule
{
    public PublicationStagingEntry? Apply(PublicationStagingEntry change)
    {
        var now = DateTime.UtcNow;

        if (change.PublishStartDate.HasValue && now < change.PublishStartDate.Value)
            return null;

        if (change.PublishEndDate.HasValue && now > change.PublishEndDate.Value)
            return null;

        return change;
    }
}