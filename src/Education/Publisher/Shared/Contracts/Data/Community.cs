namespace Crudspa.Education.Publisher.Shared.Contracts.Data;

public class Community : Observable, IValidates, INamed
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? DistrictId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Name
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? SchoolCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<CommunitySteward> CommunityStewards
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<Selectable> Schools
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (Name.HasNothing())
                errors.AddError("Name is required.", nameof(Name));
            else if (Name!.Length > 100)
                errors.AddError("Name cannot be longer than 100 characters.", nameof(Name));

            CommunityStewards.Apply(x => errors.AddRange(x.Validate()));
        });
    }
}