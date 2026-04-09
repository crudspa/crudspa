namespace Crudspa.Education.Student.Shared.Contracts.Data;

public class Achievement : Observable, IUnique, IValidates
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

    public Guid? RarityId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? TrophyImageId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? VisibleToStudents
    {
        get;
        set => SetProperty(ref field, value);
    } = true;

    public String? RarityName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ImageFile TrophyImage
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public ObservableCollection<AchievementContent> RequiredBy
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<AchievementContent> GeneratedBy
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public Int32? TotalCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (String.IsNullOrWhiteSpace(Title))
                errors.AddError("Title is required.", nameof(Title));
            else if (Title.Length > 75)
                errors.AddError("Title cannot be longer than 75 characters.", nameof(Title));

            if (!RarityId.HasValue)
                errors.AddError("Rarity is required.", nameof(RarityId));

            if (!VisibleToStudents.HasValue)
                errors.AddError("Visible To Students is required.", nameof(VisibleToStudents));
        });
    }
}