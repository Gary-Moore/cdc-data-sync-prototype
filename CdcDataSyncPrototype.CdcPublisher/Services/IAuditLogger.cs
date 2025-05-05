using CdcDataSyncPrototype.Core.Models;

namespace CdcDataSyncPrototype.CdcPublisher.Services;

public interface IAuditLogger
{
    Task LogAsync(PublicationAuditLog entry, CancellationToken cancellationToken = default);
}