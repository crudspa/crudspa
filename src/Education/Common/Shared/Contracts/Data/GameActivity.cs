namespace Crudspa.Education.Common.Shared.Contracts.Data;

public class GameActivity : Observable, IOrderable, IValidates
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? SectionId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ActivityId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ActivityKey
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ThemeWord
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? Rigorous
    {
        get;
        set => SetProperty(ref field, value);
    } = false;

    public Boolean? Multisyllabic
    {
        get;
        set => SetProperty(ref field, value);
    } = false;

    public Guid? GroupId
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

    public String? GroupName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? TypeName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Activity? Activity
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? SectionTitle
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? SectionGameId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? SectionGameKey
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? SectionGameBookId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? SectionGameBookTitle
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? SectionGameTitle
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? AssignmentBatchId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<SharedGameActivity> SharedWith
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (!ActivityId.HasValue)
                errors.AddError("Activity is required.", nameof(ActivityId));

            if (!Rigorous.HasValue)
                errors.AddError("Rigorous is required.", nameof(Rigorous));

            if (!Multisyllabic.HasValue)
                errors.AddError("Multisyllabic is required.", nameof(Multisyllabic));

            if (!TypeId.HasValue)
                errors.AddError("Type is required.", nameof(TypeId));
        });
    }
}