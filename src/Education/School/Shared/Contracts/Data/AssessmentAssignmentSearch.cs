namespace Crudspa.Education.School.Shared.Contracts.Data;

public class AssessmentAssignmentSearch : Search
{
    public List<Guid?> Classrooms
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Guid?> Assessments
    {
        get;
        set => SetProperty(ref field, value);
    } = [];
}