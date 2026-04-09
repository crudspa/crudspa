namespace Crudspa.Education.Publisher.Shared.Contracts.Data;

public class DistrictLicense : Observable, IValidates
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? LicenseId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? DistrictId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? DistrictName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? AllSchools
    {
        get;
        set => SetProperty(ref field, value);
    } = true;

    public ObservableCollection<Selectable> Schools
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
        });
    }
}