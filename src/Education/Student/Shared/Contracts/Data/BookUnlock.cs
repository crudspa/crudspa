namespace Crudspa.Education.Student.Shared.Contracts.Data;

public class BookUnlock : Observable
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

    public ImageFile BookCoverImage
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public Guid? UnitId
    {
        get;
        set => SetProperty(ref field, value);
    }
}