namespace Crudspa.Framework.Core.Shared.Contracts.Data;

public class LinkClick : Observable, IValidates
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Url
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (Url.HasNothing())
                errors.AddError("URL is required.", nameof(Url));
        });
    }
}