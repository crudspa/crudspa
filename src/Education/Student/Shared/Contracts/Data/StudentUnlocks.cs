namespace Crudspa.Education.Student.Shared.Contracts.Data;

public class StudentUnlocks : Observable
{
    public ObservableCollection<BookUnlock> Books
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<GameUnlock> Games
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<LessonUnlock> Lessons
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<ModuleUnlock> Modules
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<ObjectiveUnlock> Objectives
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<TrifoldUnlock> Trifolds
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<UnitUnlock> Units
    {
        get;
        set => SetProperty(ref field, value);
    } = [];
}