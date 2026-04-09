namespace Crudspa.Education.Student.Shared.Contracts.Data;

public class AssessmentAssignmentSearch : Search
{
    public ObservableCollection<Selectable> Classrooms
    {
        get;
        set => SetProperty(ref field, value);
    } = [];
}