namespace Crudspa.Content.Design.Client.Plugins.ElementType;

public partial class ButtonElementDesign : IElementDesign, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public Boolean ReadOnly { get; set; }
    [Parameter] public SectionElement Element { get; set; } = null!;

    [Inject] public IFontService FontService { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;

    public ButtonEditModel Model { get; set; } = null!;
    public ButtonElement ButtonElement { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        ButtonElement = Element.RequireConfig<ButtonElement>();

        Model = new(FontService, ScrollService, ButtonElement.Button);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }

    public void PrepareForSave() { }
}