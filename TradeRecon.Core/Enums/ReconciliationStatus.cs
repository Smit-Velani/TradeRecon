using System.Text.Json.Serialization;

namespace TradeRecon.Core.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ReconciliationStatus
{
    Matched = 1,
    MissingFromBroker = 2,
    MissingFromInternal = 3,
    QuantityMismatch = 4,
    PriceMismatch = 5,
    Duplicate = 6,
    Invalid = 7
}
