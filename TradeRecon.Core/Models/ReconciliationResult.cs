using TradeRecon.Core.Enums;

namespace TradeRecon.Core.Models;

public class ReconciliationResult
{
    public int Id { get; set; }

    public string TradeId { get; set; } = string.Empty;

    public ReconciliationStatus Status { get; set; }

    public string Message { get; set; } = string.Empty;

    public DateTime ReconciledAt { get; set; } = DateTime.UtcNow;
}
