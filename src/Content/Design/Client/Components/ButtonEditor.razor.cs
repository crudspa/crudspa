namespace Crudspa.Content.Design.Client.Components;

public partial class ButtonEditor : IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter, EditorRequired] public ButtonEditModel Model { get; set; } = null!;
    [Parameter] public Boolean ReadOnly { get; set; }

    protected override Task OnInitializedAsync()
    {
        Model.PropertyChanged += HandleModelChanged;
        return Task.CompletedTask;
    }

    protected override void OnParametersSet()
    {
        if (!ReadOnly)
            EnsureIconSelected();
    }

    private void HandleGraphicChanged(Button.Graphics graphic)
    {
        Model.Button.GraphicIndex = graphic;
        EnsureIconSelected();
    }

    private void EnsureIconSelected()
    {
        if (Model.Button.GraphicIndex != Button.Graphics.Icon || Model.Button.IconId.HasValue || Model.Icons.Count == 0)
            return;

        Model.Button.IconId = Model.Icons[0].Id;
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}