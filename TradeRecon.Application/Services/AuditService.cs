using TradeRecon.Core.Interfaces;
using TradeRecon.Core.Models;

namespace TradeRecon.Application.Services;

public class AuditService
{
    private readonly IAuditLogRepository _repository;

    public AuditService(
        IAuditLogRepository repository)
    {
        _repository = repository;
    }

    public async Task LogAsync(
        string action,
        string entityType,
        string details,
        string userName = "System")
    {
        var audit = new AuditLog
        {
            Action = action,
            EntityType = entityType,
            Details = details,
            UserName = userName,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(audit);
    }
}
