namespace Crudspa.Education.Student.Shared.Contracts.Data;

public class GameSection : Observable, IUnique, IValidates
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? GameId
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

    public Guid? TypeId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? Ordinal
    {
        get;
        set => SetProperty(ref field, value);
    } = 0;

    public String? StatusName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? TypeName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? GameKey
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? GameActivityCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<GameActivity> GameActivities
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (!GameId.HasValue)
                errors.AddError("Game is required.", nameof(GameId));

            if (String.IsNullOrWhiteSpace(Title))
                errors.AddError("Title is required.", nameof(Title));
            else if (Title.Length > 75)
                errors.AddError("Title cannot be longer than 75 characters.", nameof(Title));

            if (!StatusId.HasValue)
                errors.AddError("Status is required.", nameof(StatusId));

            if (!TypeId.HasValue)
                errors.AddError("Type is required.", nameof(TypeId));

            if (!Ordinal.HasValue)
                errors.AddError("Ordinal is required.", nameof(Ordinal));
        });
    }
}