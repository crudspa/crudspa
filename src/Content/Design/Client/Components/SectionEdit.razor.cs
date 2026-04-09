namespace Crudspa.Content.Design.Client.Components;

public partial class SectionEdit : IDisposable
{
    private const String StylesTab = "Styles";
    private const String LayoutTab = "Layout";

    private BoxModel? _boxModel;
    private ContainerModel? _containerModel;

    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter, EditorRequired] public SectionEditModel Model { get; set; } = null!;

    [Inject] public IScrollService ScrollService { get; set; } = null!;

    public String ActiveTab { get; private set; } = StylesTab;
    public String SettingsDescription => BuildSettingsDescription(Model.BoxModel?.Description, Model.ContainerModel?.Description);

    protected override void OnParametersSet()
    {
        RebindBoxModel(Model.BoxModel);
        RebindContainerModel(Model.ContainerModel);
    }

    protected override async Task OnAfterRenderAsync(Boolean firstRender)
    {
        if (firstRender && Model.ConsumeAutoShowSectionShape())
            await Model.SectionShapeModel.Show();

        if (Model.ConsumePendingAddedElementId() is Guid elementId)
            await ScrollService.ToId(elementId);
    }

    private Task OpenSectionSettings()
    {
        ActiveTab = StylesTab;
        return Model.BoxModel?.Show() ?? Task.CompletedTask;
    }

    private Task AddElement(ElementType elementType) => Model.AddElement(elementType.Id);

    private Task HideSectionSettings() => Model.BoxModel?.Hide() ?? Task.CompletedTask;

    private static String GetElementTypeIconClass(ElementType elementType)
    {
        return elementType.IconCssClass.HasSomething()
            ? elementType.IconCssClass!
            : "c-icon-shape";
    }

    private void SetSectionSettingsTab(String tab) => ActiveTab = tab;

    private void RebindBoxModel(BoxModel? next)
    {
        if (ReferenceEquals(_boxModel, next))
            return;

        if (_boxModel is not null)
            _boxModel.PropertyChanged -= HandleModelChanged;

        _boxModel = next;

        if (_boxModel is not null)
            _boxModel.PropertyChanged += HandleModelChanged;
    }

    private void RebindContainerModel(ContainerModel? next)
    {
        if (ReferenceEquals(_containerModel, next))
            return;

        if (_containerModel is not null)
            _containerModel.PropertyChanged -= HandleModelChanged;

        _containerModel = next;

        if (_containerModel is not null)
            _containerModel.PropertyChanged += HandleModelChanged;
    }

    private static String BuildSettingsDescription(params String?[] descriptions)
    {
        var values = descriptions
            .Where(x => x.HasSomething())
            .SelectMany(x => x!.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .Where(x => x.HasSomething() && !x.IsBasically("None"))
            .ToList();

        return values.HasItems() ? String.Join(" | ", values) : "Default";
    }

    public void Dispose()
    {
        if (_boxModel is not null)
            _boxModel.PropertyChanged -= HandleModelChanged;

        if (_containerModel is not null)
            _containerModel.PropertyChanged -= HandleModelChanged;
    }
}