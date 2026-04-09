namespace Crudspa.Education.Student.Shared.Contracts.Data;

public class GameProgress : Observable
{
    public Guid? StudentId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? GameId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? BookId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? GameCompletedCount
    {
        get;
        set => SetProperty(ref field, value);
    } = 0;
}