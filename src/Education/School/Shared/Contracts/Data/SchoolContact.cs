namespace Crudspa.Education.School.Shared.Contracts.Data;

public class SchoolContact : Observable, IValidates, INamed, ICountable
{
    public String Name => Contact.Name;

    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? SchoolId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? TitleId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? TitleName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? TestAccount
    {
        get;
        set => SetProperty(ref field, value);
    } = false;

    public Guid? ContactId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? UserId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Contact Contact
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public User User
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public Int32? TotalCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<RelatedEntity> Relations
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (!TitleId.HasValue)
                errors.AddError("Title is required.", nameof(TitleId));

            if (!TestAccount.HasValue)
                errors.AddError("Test Account is required.", nameof(TestAccount));

            errors.AddRange(Contact.Validate());

            errors.AddRange(User.Validate());
        });
    }
}