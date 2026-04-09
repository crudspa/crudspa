namespace Crudspa.Education.Student.Shared.Contracts.Data;

public class BookProgress : Observable
{
    public Guid? StudentId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? BookId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? PrefaceCompletedCount
    {
        get;
        set => SetProperty(ref field, value);
    } = 0;

    public Int32? ContentCompletedCount
    {
        get;
        set => SetProperty(ref field, value);
    } = 0;

    public Int32? MapCompletedCount
    {
        get;
        set => SetProperty(ref field, value);
    } = 0;
}