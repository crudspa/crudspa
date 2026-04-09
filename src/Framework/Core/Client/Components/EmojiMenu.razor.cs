namespace Crudspa.Framework.Core.Client.Components;

public partial class EmojiMenu : IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter, EditorRequired] public RenderFragment ChildContent { get; set; } = null!;
    [Parameter, EditorRequired] public String? SelectedEmoji { get; set; }

    [Inject] public IClickService ClickService { get; set; } = null!;

    public MenuModel Model { get; set; } = null!;

    protected override Task OnInitializedAsync()
    {
        Model = new(ClickService);
        Model.PropertyChanged += HandleModelChanged;
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}