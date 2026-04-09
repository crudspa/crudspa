namespace Crudspa.Framework.Core.Shared.Contracts.Data;

public class Notice
{
    public DateTimeOffset? Posted { get; set; }
    public String? Payload { get; set; }
    public String? Type { get; set; }
}

public class Notice<T> : Notice where T : class
{
    public Notice() { }

    public Notice(T payload)
    {
        Payload = payload.ToJson();

        var type = payload.GetType();
        Type = $"{type.FullName}, {type.Assembly.GetName().Name}";

        Posted = DateTimeOffset.Now;
    }
}