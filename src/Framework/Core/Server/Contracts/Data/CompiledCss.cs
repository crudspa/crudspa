namespace Crudspa.Framework.Core.Server.Contracts.Data;

public class CompiledCss : Observable
{
    public String? Css
    {
        get;
        set => SetProperty(ref field, value);
    }
}