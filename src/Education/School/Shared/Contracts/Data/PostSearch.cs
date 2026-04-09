namespace Crudspa.Education.School.Shared.Contracts.Data;

public class PostSearch : Search
{
    public List<Guid?> Grades
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Guid?> Subjects
    {
        get;
        set => SetProperty(ref field, value);
    } = [];
}