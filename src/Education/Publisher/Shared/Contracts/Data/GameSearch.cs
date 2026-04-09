namespace Crudspa.Education.Publisher.Shared.Contracts.Data;

public class GameSearch : Search
{
    public List<Guid?> Status
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Guid?> Grades
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Guid?> AssessmentLevels
    {
        get;
        set => SetProperty(ref field, value);
    } = [];
}