namespace Crudspa.Framework.Core.Shared.Contracts.Data;

public class Request
{
    public Request() { }

    public Request(Guid? sessionId)
    {
        SessionId = sessionId;
    }

    public Guid? SessionId { get; set; }
    public virtual Boolean IsGeneric => false;
}

public class Request<T> : Request where T : class
{
    public Request() { }

    public Request(T value)
    {
        Value = value;
    }

    public Request(Guid? sessionId, T value) : base(sessionId)
    {
        Value = value;
    }

    public T Value { get; set; } = null!;
    public override Boolean IsGeneric => true;
}