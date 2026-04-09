namespace Crudspa.Education.Student.Shared.Contracts.Data;

public class BookContent : Observable, IUnique
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

    public ObservableCollection<Chapter> Chapters
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<Trifold> Trifolds
    {
        get;
        set => SetProperty(ref field, value);
    } = [];
}