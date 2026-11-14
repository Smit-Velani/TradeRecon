using TradeRecon.Core.Models;

namespace TradeRecon.Core.Interfaces;

public interface IAuditLogRepository
{
    Task AddAsync(AuditLog auditLog);

    Task<List<AuditLog>> GetAllAsync();
}
