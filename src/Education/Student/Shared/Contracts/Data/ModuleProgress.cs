namespace Crudspa.Education.Student.Shared.Contracts.Data;

public class ModuleProgress : Observable
{
    public Guid? StudentId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ModuleId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? BookId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? ModuleCompletedCount
    {
        get;
        set => SetProperty(ref field, value);
    } = 0;
}