namespace Crudspa.Framework.Core.Client.Components;

public partial class Card<T> : IDisposable
    where T : INamed, IObservable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter, EditorRequired] public ICardModel<T> Model { get; set; } = null!;
    [Parameter, EditorRequired] public RenderFragment Values { get; set; } = null!;
    [Parameter] public String? Title { get; set; }
    [Parameter] public RenderFragment? PaneLinks { get; set; }
    [Parameter] public Boolean ShowPaneLinks { get; set; } = true;
    [Parameter] public RenderFragment Buttons { get; set; } = null!;
    [Parameter] public Boolean SupportsDelete { get; set; }
    [Parameter] public Containers Container { get; set; } = Containers.TitleAndWrappedValues;
    [Parameter] public EventCallback DeleteRequested { get; set; }
    [Parameter] public Boolean Tight { get; set; }

    public enum Containers { TitleAndWrappedValues, None }

    protected override Task OnInitializedAsync()
    {
        Model.PropertyChanged += HandleModelChanged;
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
    }
}