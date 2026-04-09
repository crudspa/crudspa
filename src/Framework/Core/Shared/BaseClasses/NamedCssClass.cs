namespace Crudspa.Framework.Core.Shared.BaseClasses;

public class NamedCssClass : Named, ICssClass
{
    public String? CssClass
    {
        get;
        set => SetProperty(ref field, value);
    }
}