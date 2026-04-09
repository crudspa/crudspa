namespace Crudspa.Education.School.Shared.Contracts.Data;

public class StudentClassroomSearch : Observable
{
    public Guid? StudentId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? SchoolYearId
    {
        get;
        set => SetProperty(ref field, value);
    }
}