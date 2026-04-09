namespace Crudspa.Framework.Core.Shared.BaseClasses;

public class OrderableCssClass : Orderable, ICssClass
{
    public String? CssClass
    {
        get;
        set => SetProperty(ref field, value);
    }
}