namespace Crudspa.Framework.Jobs.Shared.Contracts.Data;

public class Device : Observable
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }
}