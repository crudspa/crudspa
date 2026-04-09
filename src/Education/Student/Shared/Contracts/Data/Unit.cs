namespace Crudspa.Education.Student.Shared.Contracts.Data;

public class Unit : Observable, IUnique
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Title
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? StatusId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? GradeId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? GeneratesAchievementId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? RequiresAchievementId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? Treatment
    {
        get;
        set => SetProperty(ref field, value);
    } = false;

    public Boolean? Control
    {
        get;
        set => SetProperty(ref field, value);
    } = false;

    public Int32? Ordinal
    {
        get;
        set => SetProperty(ref field, value);
    } = 0;

    public ImageFile Image
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public ObservableCollection<LessonSummary> LessonSummaries
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<UnitBookSummary> UnitBookSummaries
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<Unit> ChildUnits
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public Achievement RequiresAchievement
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public Achievement GeneratesAchievement
    {
        get;
        set => SetProperty(ref field, value);
    } = new();
}