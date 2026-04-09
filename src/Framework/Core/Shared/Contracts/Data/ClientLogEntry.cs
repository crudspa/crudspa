namespace Crudspa.Framework.Core.Shared.Contracts.Data;

public class ClientLogEntry
{
    public String? CategoryName { get; set; }
    public Int32 LogLevel { get; set; }
    public Int32 EventId { get; set; }
    public String? EventName { get; set; }
    public String? Message { get; set; }
    public String? Exception { get; set; }
    public Dictionary<String, Object?> Data { get; set; } = new();
}