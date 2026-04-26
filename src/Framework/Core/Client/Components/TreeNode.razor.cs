namespace Crudspa.Framework.Core.Client.Components;

public partial class TreeNode<T>
    where T : class, INamed, IObservable
{
    [Parameter, EditorRequired] public TreeNodeModel<T> Node { get; set; } = null!;
    [Parameter, EditorRequired] public String? EntityName { get; set; }
    [Parameter, EditorRequired] public RenderFragment<T> ReadView { get; set; } = null!;
    [Parameter, EditorRequired] public RenderFragment<T> EditView { get; set; } = null!;
    [Parameter] public Boolean SupportsCreate { get; set; } = true;
    [Parameter] public Boolean SupportsDelete { get; set; } = true;
    [Parameter] public EventCallback<Guid?> DeleteRequested { get; set; }
    [Parameter] public EventCallback<Guid?> SaveRequested { get; set; }
    [Parameter] public EventCallback<Guid?> CancelRequested { get; set; }
    [Parameter] public EventCallback<Guid?> ReplyRequested { get; set; }
    [Parameter] public Card<T>.Containers ReadViewContainer { get; set; } = Card<T>.Containers.TitleAndWrappedValues;

    private Task ToggleExpanded()
    {
        Node.Expanded = !Node.Expanded;
        return Task.CompletedTask;
    }
}