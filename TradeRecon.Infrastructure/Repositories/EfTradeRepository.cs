using Microsoft.EntityFrameworkCore;
using TradeRecon.Core.Enums;
using TradeRecon.Core.Interfaces;
using TradeRecon.Core.Models;
using TradeRecon.Infrastructure.Data;

namespace TradeRecon.Infrastructure.Repositories;

public class EfTradeRepository : ITradeRepository
{
    private readonly TradeReconDbContext _context;

    public EfTradeRepository(TradeReconDbContext context)
    {
        _context = context;
    }

    public async Task<List<Trade>> GetAllAsync()
    {
        return await _context.Trades
            .AsNoTracking()
            .OrderBy(t => t.TradeId)
            .ToListAsync();
    }

    public async Task<List<Trade>> GetBySourceAsync(
        TradeSource source)
    {
        return await _context.Trades
            .AsNoTracking()
            .Where(t => t.SourceSystem == source)
            .OrderBy(t => t.TradeId)
            .ToListAsync();
    }

    public async Task<Trade?> GetByTradeIdAsync(
        string tradeId,
        TradeSource source)
    {
        return await _context.Trades
            .AsNoTracking()
            .FirstOrDefaultAsync(t =>
                t.TradeId == tradeId &&
                t.SourceSystem == source);
    }

    public async Task AddAsync(Trade trade)
    {
        await _context.Trades.AddAsync(trade);
        await _context.SaveChangesAsync();
    }

    public async Task AddRangeAsync(IEnumerable<Trade> trades)
    {
        await _context.Trades.AddRangeAsync(trades);
        await _context.SaveChangesAsync();
    }

    public async Task ClearAsync()
    {
        await _context.Trades.ExecuteDeleteAsync();
    }
}
