namespace Crudspa.Content.Display.Shared.Contracts.Data;

public class ContentPortal : Observable, IValidates
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? MaxWidth
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? StyleRevision
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? StyleCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? FontCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ImageFile BrandingImageFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public Portal Portal
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public Guid? FooterPageId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? AchievementCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? BlogCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? ForumCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? TrackCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Page? FooterPage
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            errors.AddRange(Portal.Validate());
        });
    }
}