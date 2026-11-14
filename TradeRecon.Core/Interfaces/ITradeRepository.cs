using TradeRecon.Core.Enums;
using TradeRecon.Core.Models;

namespace TradeRecon.Core.Interfaces;

public interface ITradeRepository
{
    Task<List<Trade>> GetAllAsync();

    Task<List<Trade>> GetBySourceAsync(TradeSource source);

    Task<Trade?> GetByTradeIdAsync(string tradeId, TradeSource source);

    Task AddAsync(Trade trade);

    Task AddRangeAsync(IEnumerable<Trade> trades);

    Task ClearAsync();
}
