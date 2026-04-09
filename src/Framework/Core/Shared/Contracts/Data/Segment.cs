namespace Crudspa.Framework.Core.Shared.Contracts.Data;

public class Segment : Observable, IValidates, INamed, IOrderable
{
    public String? Name => Title;

    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? PortalId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Title
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Key
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? StatusId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? StatusName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? PermissionId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? PermissionName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? IconId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? IconCssClass
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? Fixed
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? RequiresId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? Recursive
    {
        get;
        set => SetProperty(ref field, value);
    } = false;

    public Guid? TypeId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? TypeName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? AllLicenses
    {
        get;
        set => SetProperty(ref field, value);
    } = true;

    public Guid? ParentId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ConfigJson
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? TypeDisplayView
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? TypeEditorView
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<Selectable> Licenses
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public Int32? SegmentCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? PaneCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? Ordinal
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<Pane> Panes
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<Segment> Segments
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (Key.HasNothing())
                errors.AddError("Key is required.", nameof(Key));
            else if (Key!.Length > 100)
                errors.AddError("Key cannot be longer than 100 characters.", nameof(Key));
            else if (!Key.IsSimpleKey())
                errors.AddError("Key must be all lowercase letters, numbers, or hyphens.", nameof(Key));

            if (Title.HasNothing())
                errors.AddError("Title is required.", nameof(Title));
            else if (Title!.Length > 150)
                errors.AddError("Title cannot be longer than 150 characters.", nameof(Title));

            if (!StatusId.HasValue)
                errors.AddError("Status is required.", nameof(StatusId));

            if (!Fixed.HasValue)
                errors.AddError("Fixed is required.", nameof(Fixed));

            if (!RequiresId.HasValue)
                errors.AddError("Requires ID is required.", nameof(RequiresId));

            if (!TypeId.HasValue)
                errors.AddError("Type is required.", nameof(TypeId));

            if (!Recursive.HasValue)
                errors.AddError("Recursive is required.", nameof(Recursive));

            if (!AllLicenses.HasValue)
                errors.AddError("All Licenses is required.", nameof(AllLicenses));
        });
    }
}