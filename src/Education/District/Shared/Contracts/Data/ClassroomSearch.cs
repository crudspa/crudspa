namespace Crudspa.Education.District.Shared.Contracts.Data;

public class ClassroomSearch : Search
{
    public ObservableCollection<Selectable> Schools
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<Selectable> SchoolYears
    {
        get;
        set => SetProperty(ref field, value);
    } = [];
}