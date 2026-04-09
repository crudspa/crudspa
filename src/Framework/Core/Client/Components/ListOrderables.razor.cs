namespace Crudspa.Framework.Core.Client.Components;

public partial class ListOrderables<T> : IDisposable
    where T : class, INamed, IObservable, IOrderable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter, EditorRequired] public ListOrderablesModel<T> Model { get; set; } = null!;
    [Parameter, EditorRequired] public RenderFragment<T> ReadView { get; set; } = null!;
    [Parameter, EditorRequired] public String? EntityName { get; set; } = String.Empty;
    [Parameter] public RenderFragment<T>? PaneLinks { get; set; }
    [Parameter] public RenderFragment? ToolbarMenuItems { get; set; }
    [Parameter] public RenderFragment<T>? CardMenuItems { get; set; }
    [Parameter] public RenderFragment? Modals { get; set; }
    [Parameter] public String? UrlPrefix { get; set; } = String.Empty;
    [Parameter] public String? UrlSuffix { get; set; } = String.Empty;
    [Parameter] public EventCallback AddRequested { get; set; }
    [Parameter] public Boolean ShowFilter { get; set; }
    [Parameter] public Boolean ShowAdd { get; set; } = true;
    [Parameter] public Boolean ShowDelete { get; set; } = true;
    [Parameter] public Boolean ShowView { get; set; } = true;
    [Parameter] public EventCallback<Guid?> DeleteRequested { get; set; }
    [Parameter] public EventCallback<Guid?> MoveUpRequested { get; set; }
    [Parameter] public EventCallback<Guid?> MoveDownRequested { get; set; }
    [Parameter] public Card<T>.Containers ReadViewContainer { get; set; } = Card<T>.Containers.TitleAndWrappedValues;
    [Parameter] public Boolean Tight { get; set; }

    [Inject] public INavigator Navigator { get; set; } = null!;

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

    public async Task AddNew()
    {
        if (AddRequested.HasDelegate)
        {
            await AddRequested.InvokeAsync();
            return;
        }

        Navigator.GoTo(BuildUrl(Guid.NewGuid(), "new"));
    }

    public String BuildUrl(Guid? id, String? state = null)
    {
        var suffix = UrlSuffix ?? String.Empty;

        if (state.HasSomething())
            suffix = suffix.HasSomething() ? suffix + $"&state={state}" : $"?state={state}";

        return $"{UrlPrefix}-{id:D}{suffix}";
    }
}