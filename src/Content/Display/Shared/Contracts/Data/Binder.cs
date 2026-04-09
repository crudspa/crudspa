namespace Crudspa.Content.Display.Shared.Contracts.Data;

public class Binder : Observable, IValidates
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? TypeId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? TypeName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? DisplayView
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<Page>? Pages
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? LastPageViewed
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Page? InitialPage
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (!TypeId.HasValue)
                errors.AddError("Type is required.", nameof(TypeId));
        });
    }
}