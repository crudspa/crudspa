namespace Crudspa.Education.Student.Shared.Contracts.Data;

public class TrifoldProgress : Observable
{
    public Guid? StudentId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? TrifoldId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? BookId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? TrifoldCompletedCount
    {
        get;
        set => SetProperty(ref field, value);
    } = 0;
}