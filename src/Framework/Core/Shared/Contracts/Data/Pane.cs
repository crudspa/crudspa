namespace Crudspa.Framework.Core.Shared.Contracts.Data;

public class Pane : Observable, IValidates, INamed, IOrderable
{
    public String? Name => Title;

    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? SegmentId
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

    public String? TypeEditorView
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

    public String? ConfigJson
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? Ordinal
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (Key.HasNothing())
                errors.AddError("Key is required.", nameof(Key));
            else if (Key!.Length > 100)
                errors.AddError("Key cannot be longer than 100 characters.", nameof(Key));

            if (Title.HasNothing())
                errors.AddError("Title is required.", nameof(Title));
            else if (Title!.Length > 150)
                errors.AddError("Title cannot be longer than 150 characters.", nameof(Title));

            if (!SegmentId.HasValue)
                errors.AddError("Segment is required.", nameof(SegmentId));

            if (!TypeId.HasValue)
                errors.AddError("Type is required.", nameof(TypeId));
        });
    }
}