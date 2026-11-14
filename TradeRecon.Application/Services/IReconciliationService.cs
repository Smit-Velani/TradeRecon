using TradeRecon.Core.Models;

namespace TradeRecon.Application.Services;

public interface IReconciliationService
{
    Task<List<ReconciliationResult>> ReconcileAsync();
}
