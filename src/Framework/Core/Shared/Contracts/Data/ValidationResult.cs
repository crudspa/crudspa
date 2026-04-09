namespace Crudspa.Framework.Core.Shared.Contracts.Data;

public class ValidationResult : Observable
{
    public Boolean IsValid
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