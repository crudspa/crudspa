namespace Crudspa.Framework.Core.Client.Components;

public partial class Form<T> : IDisposable
    where T : INamed, IObservable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter, EditorRequired] public FormModel<T> Model { get; set; } = null!;
    [Parameter, EditorRequired] public RenderFragment ReadView { get; set; } = null!;
    [Parameter, EditorRequired] public RenderFragment EditView { get; set; } = null!;
    [Parameter] public String? Title { get; set; }
    [Parameter] public Boolean SupportsDelete { get; set; } = true;
    [Parameter] public EventCallback DeleteRequested { get; set; }
    [Parameter] public EventCallback SaveRequested { get; set; }
    [Parameter] public EventCallback CancelRequested { get; set; }
    [Parameter] public EventCallback MoveUpRequested { get; set; }
    [Parameter] public EventCallback MoveDownRequested { get; set; }
    [Parameter] public Card<T>.Containers ReadViewContainer { get; set; } = Card<T>.Containers.TitleAndWrappedValues;
    [Parameter] public Boolean Tight { get; set; }

    protected override Task OnInitializedAsync()
    {
        Model.PropertyChanged += HandleModelChanged;

        return Task.CompletedTask;
    }

    public async Task HandleEditClicked()
    {
        Model.ReadOnly = false;
        await Task.CompletedTask;
    }

    public async Task HandleSaveRequested()
    {
        await SaveRequested.InvokeAsync();
    }

    public async Task HandleCancelRequested()
    {
        await CancelRequested.InvokeAsync();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}