namespace Crudspa.Framework.Core.Client.Components;

public partial class ManyOrderables<T> : IDisposable
    where T : class, INamed, IObservable, IOrderable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter, EditorRequired] public String? EntityName { get; set; } = String.Empty;
    [Parameter, EditorRequired] public ManyOrderablesModel<T> Model { get; set; } = null!;
    [Parameter, EditorRequired] public RenderFragment<T> ReadView { get; set; } = null!;
    [Parameter, EditorRequired] public RenderFragment<T> EditView { get; set; } = null!;
    [Parameter] public RenderFragment<T>? Children { get; set; }
    [Parameter] public RenderFragment? Modals { get; set; }
    [Parameter] public Boolean ShowFilter { get; set; }
    [Parameter] public EventCallback<Guid?> DeleteRequested { get; set; }
    [Parameter] public EventCallback<Guid?> SaveRequested { get; set; }
    [Parameter] public EventCallback<Guid?> CancelRequested { get; set; }
    [Parameter] public EventCallback<Guid?> MoveUpRequested { get; set; }
    [Parameter] public EventCallback<Guid?> MoveDownRequested { get; set; }
    [Parameter] public Card<T>.Containers ReadViewContainer { get; set; } = Card<T>.Containers.TitleAndWrappedValues;
    [Parameter] public Boolean Tight { get; set; }

    public IEnumerable<FormModel<T>> SortedForms => Model.Forms.OrderBy(x => x.SortIndex);

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