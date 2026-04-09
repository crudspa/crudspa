namespace Crudspa.Education.Common.Shared.Contracts.Data;

public class SchoolYear : Observable, IValidates, INamed
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Name
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateOnly? Starts
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateOnly? Ends
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors => { });
    }
}