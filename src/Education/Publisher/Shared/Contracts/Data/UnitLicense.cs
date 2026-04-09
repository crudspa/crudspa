namespace Crudspa.Education.Publisher.Shared.Contracts.Data;

public class UnitLicense : Observable, IValidates
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

    public Guid? UnitId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? UnitTitle
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? AllBooks
    {
        get;
        set => SetProperty(ref field, value);
    } = true;

    public Boolean? AllLessons
    {
        get;
        set => SetProperty(ref field, value);
    } = true;

    public ObservableCollection<Selectable> Books
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<Selectable> Lessons
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