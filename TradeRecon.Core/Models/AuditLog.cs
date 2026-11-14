namespace TradeRecon.Core.Models;

public class AuditLog
{
    public int Id { get; set; }

    public string Action { get; set; } = string.Empty;

    public string EntityType { get; set; } = string.Empty;

    public string Details { get; set; } = string.Empty;

    public string UserName { get; set; } = "System";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
