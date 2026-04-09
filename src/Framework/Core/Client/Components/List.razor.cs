namespace Crudspa.Framework.Core.Client.Components;

public partial class List<T> : IDisposable
    where T : class, INamed, IObservable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter, EditorRequired] public ListModel<T> Model { get; set; } = null!;
    [Parameter, EditorRequired] public RenderFragment<T> ReadView { get; set; } = null!;
    [Parameter, EditorRequired] public String? EntityName { get; set; } = String.Empty;
    [Parameter] public RenderFragment<T>? PaneLinks { get; set; }
    [Parameter] public RenderFragment? ToolbarMenuItems { get; set; }
    [Parameter] public RenderFragment<T>? CardMenuItems { get; set; }
    [Parameter] public RenderFragment? Modals { get; set; }
    [Parameter] public String? UrlPrefix { get; set; } = String.Empty;
    [Parameter] public String? UrlSuffix { get; set; } = String.Empty;
    [Parameter] public Boolean ShowFilter { get; set; }
    [Parameter] public Boolean ShowAdd { get; set; } = true;
    [Parameter] public Boolean ShowDelete { get; set; } = true;
    [Parameter] public Boolean ShowView { get; set; } = true;
    [Parameter] public EventCallback<Guid?> DeleteRequested { get; set; }

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

    public void AddNew()
    {
        var suffix = UrlSuffix.HasSomething() ? UrlSuffix + "&state=new" : "?state=new";
        Navigator.GoTo($"{UrlPrefix}-{Guid.NewGuid():D}{suffix}");
    }
}