using Container = Crudspa.Content.Display.Shared.Contracts.Data.Container;

namespace Crudspa.Content.Display.Shared.Contracts.Config.ElementType;

public class MultimediaElement : Observable, IValidates
{
    private ObservableCollection<MultimediaItem> _multimediaItems = [];

    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ElementId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Container Container
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public ObservableCollection<MultimediaItem> MultimediaItems
    {
        get => _multimediaItems;
        set => SetProperty(ref _multimediaItems, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (MultimediaItems.Count == 0)
                errors.AddError("One or more media items required.");

            foreach (var multimediaItem in _multimediaItems)
                errors.AddRange(multimediaItem.Validate());
        });
    }
}