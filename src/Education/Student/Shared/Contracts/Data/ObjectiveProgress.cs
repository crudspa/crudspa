namespace Crudspa.Education.Student.Shared.Contracts.Data;

public class ObjectiveProgress : Observable
{
    public Guid? StudentId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ObjectiveId
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