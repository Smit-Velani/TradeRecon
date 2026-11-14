using System.Text.Json.Serialization;

namespace TradeRecon.Core.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TradeSource
{
    Internal = 1,
    Broker = 2
}
