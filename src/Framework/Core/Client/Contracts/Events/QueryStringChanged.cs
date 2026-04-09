namespace Crudspa.Framework.Core.Client.Contracts.Events;

public class QueryStringChanged
{
    public String Path { get; set; } = null!;
    public String Query { get; set; } = null!;
}