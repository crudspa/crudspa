namespace Crudspa.Education.Publisher.Shared.Contracts.Data;

public class UnitLicenseBook : Observable, IValidates
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? UnitLicenseId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? BookId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? BookTitle
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? Selected
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors => { });
    }
}