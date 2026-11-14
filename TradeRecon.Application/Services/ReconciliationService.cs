using TradeRecon.Core.Enums;
using TradeRecon.Core.Interfaces;
using TradeRecon.Core.Models;

namespace TradeRecon.Application.Services;

public class ReconciliationService : IReconciliationService
{
    private readonly ITradeRepository _tradeRepository;
    private readonly IReconciliationResultRepository? _resultRepository;

    public ReconciliationService(
        ITradeRepository tradeRepository,
        IReconciliationResultRepository? resultRepository = null)
    {
        _tradeRepository = tradeRepository;
        _resultRepository = resultRepository;
    }

    public async Task<List<ReconciliationResult>> ReconcileAsync()
    {
        var internalTrades =
            await _tradeRepository.GetBySourceAsync(
                TradeSource.Internal);

        var brokerTrades =
            await _tradeRepository.GetBySourceAsync(
                TradeSource.Broker);

        var results = new List<ReconciliationResult>();

        var duplicateInternalIds = internalTrades
            .GroupBy(t => t.TradeId)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToHashSet();

        var duplicateBrokerIds = brokerTrades
            .GroupBy(t => t.TradeId)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToHashSet();

        var allTradeIds = internalTrades
            .Select(t => t.TradeId)
            .Union(brokerTrades.Select(t => t.TradeId))
            .Distinct();

        foreach (var tradeId in allTradeIds)
        {
            if (duplicateInternalIds.Contains(tradeId) ||
                duplicateBrokerIds.Contains(tradeId))
            {
                results.Add(CreateResult(
                    tradeId,
                    ReconciliationStatus.Duplicate,
                    "Duplicate TradeId detected."));
                continue;
            }

            var internalTrade =
                internalTrades.FirstOrDefault(
                    t => t.TradeId == tradeId);

            var brokerTrade =
                brokerTrades.FirstOrDefault(
                    t => t.TradeId == tradeId);

            if (internalTrade is null)
            {
                results.Add(CreateResult(
                    tradeId,
                    ReconciliationStatus.MissingFromInternal,
                    "Trade exists in broker system but not internal system."));
                continue;
            }

            if (brokerTrade is null)
            {
                results.Add(CreateResult(
                    tradeId,
                    ReconciliationStatus.MissingFromBroker,
                    "Trade exists internally but not in broker system."));
                continue;
            }

            if (internalTrade.Quantity != brokerTrade.Quantity)
            {
                results.Add(CreateResult(
                    tradeId,
                    ReconciliationStatus.QuantityMismatch,
                    $"Internal quantity {internalTrade.Quantity} differs from broker quantity {brokerTrade.Quantity}."));
                continue;
            }

            if (internalTrade.Price != brokerTrade.Price)
            {
                results.Add(CreateResult(
                    tradeId,
                    ReconciliationStatus.PriceMismatch,
                    $"Internal price {internalTrade.Price} differs from broker price {brokerTrade.Price}."));
                continue;
            }

            results.Add(CreateResult(
                tradeId,
                ReconciliationStatus.Matched,
                "Trade matched successfully."));
        }

        if (_resultRepository is not null &&
            results.Count > 0)
        {
            await _resultRepository.AddRangeAsync(results);
        }

        return results;
    }

    private static ReconciliationResult CreateResult(
        string tradeId,
        ReconciliationStatus status,
        string message)
    {
        return new ReconciliationResult
        {
            TradeId = tradeId,
            Status = status,
            Message = message,
            ReconciledAt = DateTime.UtcNow
        };
    }
}
