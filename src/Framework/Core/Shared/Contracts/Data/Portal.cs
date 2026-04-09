namespace Crudspa.Framework.Core.Shared.Contracts.Data;

public class Portal : Observable, IValidates
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Key
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Title
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? SessionsPersist
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? AllowSignIn
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? RequireSignIn
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? SegmentCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<Segment> Segments
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<PortalFeature> Features
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public String? NavigationTypeDisplayView
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (Key.HasNothing())
                errors.AddError("Key is required.", nameof(Key));
            else if (Key!.Length > 75)
                errors.AddError("Key cannot be longer than 75 characters.", nameof(Key));
            else if (!Key.IsSimpleKey())
                errors.AddError("Key must be all lowercase letters, numbers, or hyphens.", nameof(Key));

            if (Title.HasNothing())
                errors.AddError("Title is required.", nameof(Title));
            else if (Title!.Length > 75)
                errors.AddError("Title cannot be longer than 75 characters.", nameof(Title));
        });
    }
}