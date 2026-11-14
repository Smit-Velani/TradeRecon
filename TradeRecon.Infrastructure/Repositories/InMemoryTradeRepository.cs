using TradeRecon.Core.Enums;
using TradeRecon.Core.Interfaces;
using TradeRecon.Core.Models;

namespace TradeRecon.Infrastructure.Repositories;

public class InMemoryTradeRepository : ITradeRepository
{
    private readonly List<Trade> _trades = new();

    public Task<List<Trade>> GetAllAsync()
    {
        return Task.FromResult(_trades.ToList());
    }

    public Task<List<Trade>> GetBySourceAsync(TradeSource source)
    {
        var trades = _trades
            .Where(t => t.SourceSystem == source)
            .ToList();

        return Task.FromResult(trades);
    }

    public Task<Trade?> GetByTradeIdAsync(
        string tradeId,
        TradeSource source)
    {
        var trade = _trades.FirstOrDefault(t =>
            t.TradeId == tradeId &&
            t.SourceSystem == source);

        return Task.FromResult(trade);
    }

    public Task AddAsync(Trade trade)
    {
        _trades.Add(trade);
        return Task.CompletedTask;
    }

    public Task AddRangeAsync(IEnumerable<Trade> trades)
    {
        _trades.AddRange(trades);
        return Task.CompletedTask;
    }

    public Task ClearAsync()
    {
        _trades.Clear();
        return Task.CompletedTask;
    }
}
