namespace Crudspa.Education.Student.Shared.Contracts.Data;

public class UnitUnlock : Observable
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

    public ImageFile Image
    {
        get;
        set => SetProperty(ref field, value);
    } = new();
}