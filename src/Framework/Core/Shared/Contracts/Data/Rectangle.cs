namespace Crudspa.Framework.Core.Shared.Contracts.Data;

public class Rectangle : Observable
{
    public Int32? Width
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? Height
    {
        get;
        set => SetProperty(ref field, value);
    }
}