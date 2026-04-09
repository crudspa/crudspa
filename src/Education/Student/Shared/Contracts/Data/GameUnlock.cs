namespace Crudspa.Education.Student.Shared.Contracts.Data;

public class GameUnlock : Observable
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

    public String? IconName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? BookTitle
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ImageFile BookCoverImage
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public Guid? BookId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? UnitId
    {
        get;
        set => SetProperty(ref field, value);
    }
}