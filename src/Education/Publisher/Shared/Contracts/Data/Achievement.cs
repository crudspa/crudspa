namespace Crudspa.Education.Publisher.Shared.Contracts.Data;

public class Achievement : Observable, IValidates, INamed, ICountable
{
    public String? Name => Title;

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

    public String? RarityName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ImageFile TrophyImageFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public Boolean? VisibleToStudents
    {
        get;
        set => SetProperty(ref field, value);
    } = true;

    public Int32? TotalCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (Title.HasNothing())
                errors.AddError("Title is required.", nameof(Title));
            else if (Title!.Length > 75)
                errors.AddError("Title cannot be longer than 75 characters.", nameof(Title));

            if (!RarityId.HasValue)
                errors.AddError("Rarity is required.", nameof(RarityId));

            if (TrophyImageFile.Name.HasNothing())
                errors.AddError("Trophy Image is required.", nameof(TrophyImageFile));

            if (!VisibleToStudents.HasValue)
                errors.AddError("Visible To Students is required.", nameof(VisibleToStudents));
        });
    }
}