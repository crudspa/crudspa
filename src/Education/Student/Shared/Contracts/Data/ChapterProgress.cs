namespace Crudspa.Education.Student.Shared.Contracts.Data;

public class ChapterProgress : Observable
{
    public Guid? StudentId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ChapterId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? BookId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? TimesCompleted
    {
        get;
        set => SetProperty(ref field, value);
    } = 0;
}