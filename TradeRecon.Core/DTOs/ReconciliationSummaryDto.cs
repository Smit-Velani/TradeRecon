namespace TradeRecon.Core.DTOs;

public class ReconciliationSummaryDto
{
    public int Total { get; set; }

    public int Matched { get; set; }

    public int Mismatched { get; set; }

    public int Missing { get; set; }

    public int Duplicates { get; set; }

    public decimal MatchRate { get; set; }
}
