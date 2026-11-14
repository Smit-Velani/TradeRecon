using Microsoft.EntityFrameworkCore;
using TradeRecon.Core.Interfaces;
using TradeRecon.Core.Models;
using TradeRecon.Infrastructure.Data;

namespace TradeRecon.Infrastructure.Repositories;

public class EfReconciliationResultRepository
    : IReconciliationResultRepository
{
    private readonly TradeReconDbContext _context;

    public EfReconciliationResultRepository(
        TradeReconDbContext context)
    {
        _context = context;
    }

    public async Task AddRangeAsync(
        IEnumerable<ReconciliationResult> results)
    {
        await _context.ReconciliationResults
            .AddRangeAsync(results);

        await _context.SaveChangesAsync();
    }

    public async Task<List<ReconciliationResult>> GetAllAsync()
    {
        return await _context.ReconciliationResults
            .AsNoTracking()
            .OrderByDescending(r => r.ReconciledAt)
            .ToListAsync();
    }

    public async Task<ReconciliationResult?> GetByIdAsync(int id)
    {
        return await _context.ReconciliationResults
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id);
    }
}
