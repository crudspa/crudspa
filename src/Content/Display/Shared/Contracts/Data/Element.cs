namespace Crudspa.Content.Display.Shared.Contracts.Data;

public class Element : Observable, IValidates, IOrderable
{
    private Guid? _elementId;

    public Guid? Id
    {
        get => _elementId;
        set => SetProperty(ref _elementId, value);
    }

    public Guid? ElementId
    {
        get => _elementId;
        set => SetProperty(ref _elementId, value);
    }

    public Guid? SectionId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? TypeId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? RequireInteraction
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? Ordinal
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ElementType? ElementType
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Box Box
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public Item Item
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public virtual List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (!TypeId.HasValue)
                errors.AddError("Type is required.", nameof(TypeId));

            errors.AddRange(Box.Validate());
            errors.AddRange(Item.Validate());
        });
    }
}