namespace Crudspa.Framework.Core.Shared.BaseClasses;

public class Linked : Named
{
    public String? Url
    {
        get;
        set => SetProperty(ref field, value);
    }
}