using Container = Crudspa.Content.Display.Shared.Contracts.Data.Container;

namespace Crudspa.Content.Design.Client.Models;

public class ContainerModel : ModalModel
{
    private void HandleContainerChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(Container));
    public void SetActiveTab(String key) => ActiveTab = key;

    private readonly IContainerService _containerService;

    private Container _container;

    public ContainerModel(IScrollService scrollService, IContainerService containerService, Container container) : base(scrollService)
    {
        _containerService = containerService;

        _container = container;
        _container.PropertyChanged += HandleContainerChanged;
    }

    public override void Dispose()
    {
        _container.PropertyChanged -= HandleContainerChanged;
        base.Dispose();
    }

    public Container Container
    {
        get => _container;
        set => SetProperty(ref _container, value);
    }

    public String ActiveTab
    {
        get;
        set => SetProperty(ref field, value);
    } = "Direction";

    public List<Orderable> DirectionNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Orderable> WrapNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Orderable> JustifyContentNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Orderable> AlignItemsNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Orderable> AlignContentNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public async Task Initialize()
    {
        await WithMany("Initializing...",
            FetchDirectionNames(),
            FetchWrapNames(),
            FetchJustifyContentNames(),
            FetchAlignItemsNames(),
            FetchAlignContentNames());

        _container.DirectionId ??= DirectionIds.Row;
    }

    public async Task FetchDirectionNames()
    {
        var response = await WithAlerts(() => _containerService.FetchDirectionNames(new()), false);
        if (response.Ok) DirectionNames = response.Value.ToList();
    }

    public async Task FetchWrapNames()
    {
        var response = await WithAlerts(() => _containerService.FetchWrapNames(new()), false);
        if (response.Ok) WrapNames = response.Value.ToList();
    }

    public async Task FetchJustifyContentNames()
    {
        var response = await WithAlerts(() => _containerService.FetchJustifyContentNames(new()), false);
        if (response.Ok) JustifyContentNames = response.Value.ToList();
    }

    public async Task FetchAlignItemsNames()
    {
        var response = await WithAlerts(() => _containerService.FetchAlignItemsNames(new()), false);
        if (response.Ok) AlignItemsNames = response.Value.ToList();
    }

    public async Task FetchAlignContentNames()
    {
        var response = await WithAlerts(() => _containerService.FetchAlignContentNames(new()), false);
        if (response.Ok) AlignContentNames = response.Value.ToList();
    }

    public String Description
    {
        get
        {
            List<String> values = [];

            if (Container.DirectionId is not null)
                values.Add($"Direction: {DirectionNames.FirstOrDefault(x => x.Id.Equals(Container.DirectionId))?.Name}");

            if (Container.Gap.HasSomething())
                values.Add($"Gap: {Container.Gap}");

            if (Container.WrapId is not null)
                values.Add($"Wrap: {WrapNames.FirstOrDefault(x => x.Id.Equals(Container.WrapId))?.Name}");

            if (Container.JustifyContentId is not null)
                values.Add($"Justify Content: {JustifyContentNames.FirstOrDefault(x => x.Id.Equals(Container.JustifyContentId))?.Name}");

            if (Container.AlignItemsId is not null)
                values.Add($"Align Items: {AlignItemsNames.FirstOrDefault(x => x.Id.Equals(Container.AlignItemsId))?.Name}");

            if (Container.AlignContentId is not null)
                values.Add($"Align Content: {AlignContentNames.FirstOrDefault(x => x.Id.Equals(Container.AlignContentId))?.Name}");

            return values.HasItems() ? String.Join(" | ", values) : "None";
        }
    }
}