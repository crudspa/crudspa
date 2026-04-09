namespace Crudspa.Framework.Core.Client.Contracts.Events;

public class SilenceRequested
{
    private static Int64 _nextNumber;

    public Guid? ExceptInstanceId { get; set; }
    public Int64 Number { get; init; } = Interlocked.Increment(ref _nextNumber);
}