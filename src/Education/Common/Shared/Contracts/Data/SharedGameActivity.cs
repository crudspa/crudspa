namespace Crudspa.Education.Common.Shared.Contracts.Data;

public class SharedGameActivity : Observable
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? SectionId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ActivityId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? SectionGameId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? SectionTitle
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? SectionGameKey
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? SectionGameBookId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? SectionGameBookTitle
    {
        get;
        set => SetProperty(ref field, value);
    }
}