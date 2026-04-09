namespace Crudspa.Framework.Core.Client.Contracts.Data;

public class ContextMenuItem : Observable
{
    public String? Icon
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Text
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean StartsGroup
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Action? OnClick { get; set; }
}