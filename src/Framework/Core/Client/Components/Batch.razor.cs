namespace Crudspa.Framework.Core.Client.Components;

public partial class Batch<T> : IDisposable
    where T : class, IObservable, IOrderable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter, EditorRequired] public BatchModel<T> Model { get; set; } = null!;
    [Parameter, EditorRequired] public RenderFragment<T> EditView { get; set; } = null!;
    [Parameter] public Boolean ReadOnly { get; set; }
    [Parameter] public String? NoRecordsText { get; set; } = "None";
    [Parameter] public Boolean AllowReordering { get; set; } = true;

    protected override Task OnInitializedAsync()
    {
        Model.PropertyChanged += HandleModelChanged;
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}