using CdcDataSyncPrototype.CdcPublisher.Infrastructure;
using CdcDataSyncPrototype.Core.Models;

namespace CdcDataSyncPrototype.CdcPublisher.Services.PublicationRules;

public class PublicationChangeRulesEngine(IEnumerable<IPublicationRule> rules,
    IAuditLogger? auditLogger = null,
    ILogger<PublicationChangeRulesEngine>? logger = null) : IPublicationChangeRulesEngine
{
    public PublicationStagingEntry? Apply(PublicationStagingEntry change)
    {
        foreach (var rule in rules)
        {
            var result = rule.Apply(change);

            var logEntry = new PublicationAuditLog
            {
                Id = Guid.NewGuid(),
                PublicationId = change.Id,
                Title = change.Title ?? "[Untitled]",
                RuleOutcome = result == null
                    ? $"Suppressed: {rule.GetType().Name}"
                    : $"Passed: {rule.GetType().Name}",
                Timestamp = DateTime.UtcNow
            };

            if (auditLogger != null)
            {
                // Fire and forget — we don't want audit failures to block publishing
                _ = auditLogger.LogAsync(logEntry);
            }

            if (result == null)
            {
                logger?.LogDebug("Change {Id} was suppressed by {Rule}", change.Id, rule.GetType().Name);
                return null;
            }
        }

        return change;
    }

}

