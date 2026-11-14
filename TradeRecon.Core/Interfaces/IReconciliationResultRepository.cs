using TradeRecon.Core.Models;

namespace TradeRecon.Core.Interfaces;

public interface IReconciliationResultRepository
{
    Task AddRangeAsync(IEnumerable<ReconciliationResult> results);

    Task<List<ReconciliationResult>> GetAllAsync();

    Task<ReconciliationResult?> GetByIdAsync(int id);
}
