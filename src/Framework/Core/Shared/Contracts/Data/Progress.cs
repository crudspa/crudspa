namespace Crudspa.Framework.Core.Shared.Contracts.Data;

public class Progress : Observable
{
    public enum States
    {
        Pending, InProgress, Succeeded, Failed,
    }

    public Guid? EntityId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateTimeOffset Timestamp
    {
        get;
        set => SetProperty(ref field, value);
    }

    public States State
    {
        get;
        set => SetProperty(ref field, value);
    } = States.Pending;

    public Decimal Percentage
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Method
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Message
    {
        get;
        set => SetProperty(ref field, value);
    }
}