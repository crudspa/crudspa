namespace Crudspa.Framework.Core.Shared.Extensions;

public static class ThreadSafeRandom
{
    [field: ThreadStatic] public static Random ThisThreadsRandom => field ??= new(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId));
}

public class StaticRandom
{
    private static readonly Object RandomLock = new();
    private static readonly Random RandomInterval = new(Guid.NewGuid().GetHashCode());

    public static Int32 Next(Int32 maxValue)
    {
        lock (RandomLock)
        {
            return RandomInterval.Next(maxValue);
        }
    }

    public static Int32 Next(Int32 minValue, Int32 maxValue)
    {
        lock (RandomLock)
        {
            return RandomInterval.Next(minValue, maxValue);
        }
    }
}