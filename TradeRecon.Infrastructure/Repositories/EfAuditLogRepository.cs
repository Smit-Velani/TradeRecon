using Microsoft.EntityFrameworkCore;
using TradeRecon.Core.Interfaces;
using TradeRecon.Core.Models;
using TradeRecon.Infrastructure.Data;

namespace TradeRecon.Infrastructure.Repositories;

public class EfAuditLogRepository : IAuditLogRepository
{
    private readonly TradeReconDbContext _context;

    public EfAuditLogRepository(
        TradeReconDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(AuditLog auditLog)
    {
        await _context.AuditLogs.AddAsync(auditLog);

        await _context.SaveChangesAsync();
    }

    public async Task<List<AuditLog>> GetAllAsync()
    {
        return await _context.AuditLogs
            .AsNoTracking()
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }
}
