namespace Crudspa.Education.Student.Shared.Contracts.Data;

public class BookLite : Observable, IUnique
{
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

    public String? Author
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Summary
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? CoverImageId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? PrefaceBinderId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? CoverImageUri
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ImageFile CoverImage
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public Boolean? HasPreface
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? HasSomething
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? HasMap
    {
        get;
        set => SetProperty(ref field, value);
    }

    public BookProgress Progress
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public ObservableCollection<Game> Games
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<Module> Modules
    {
        get;
        set => SetProperty(ref field, value);
    } = [];
}