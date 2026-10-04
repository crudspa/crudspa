namespace Crudspa.Content.Display.Shared.Contracts.Data;

public class PublicationSummary : Observable, IValidates, INamed, IOrderable
{
    public String? Name => Title;

    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Title
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? GroupOrdinal { get; set; }

    public Guid? GroupId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? GroupName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? StatusId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? StatusName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Citation
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Venue
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Sample
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Timeline
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Outcome
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Intervention
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ImageFile ImageFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public String? ImageAlt
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? Year
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? Ordinal
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<PublicationResourceSummary> PublicationResources
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            PublicationResources.Apply(x => errors.AddRange(x.Validate()));
        });
    }
}