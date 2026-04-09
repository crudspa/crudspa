namespace Crudspa.Framework.Core.Shared.BaseClasses;

public class Described : Named, IDescribed
{
    public String? Description
    {
        get;
        set => SetProperty(ref field, value);
    }
}