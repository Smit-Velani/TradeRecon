using TradeRecon.Core.Enums;

namespace TradeRecon.Core.Models;

public class Trade
{
    public int Id { get; set; }

    public string TradeId { get; set; } = string.Empty;

    public string Symbol { get; set; } = string.Empty;

    public decimal Quantity { get; set; }

    public decimal Price { get; set; }

    public string Currency { get; set; } = "USD";

    public DateTime TradeDate { get; set; }

    public TradeSource SourceSystem { get; set; }
}
