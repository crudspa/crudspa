namespace Crudspa.Education.School.Shared.Contracts.Data;

public class StudentSearch : Search
{
    public List<Guid?> Grades
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Guid?> AssessmentLevelGroups
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<Selectable> SchoolYears
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public Boolean? NotInClassroom
    {
        get;
        set => SetProperty(ref field, value);
    } = false;

    public Boolean? IncludeTestAccounts
    {
        get;
        set => SetProperty(ref field, value);
    } = false;
}