namespace Crudspa.Content.Display.Shared.Contracts.Data;

public class Section : Observable, IValidates, IOrderable
{
    public String? Name => null;

    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? PageId
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
    }

    public ObservableCollection<SectionElement> Elements
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public Box Box
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public Container Container
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            errors.AddRange(Box.Validate());
            errors.AddRange(Container.Validate());

            if (Elements.IsEmpty())
                errors.AddError("At least one Element is required.");
            else
                Elements.Apply(x => errors.AddRange(x.Validate()));

            errors.AddRange(Box.Validate());
        });
    }
}