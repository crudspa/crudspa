namespace Crudspa.Framework.Core.Client.Components;

public partial class Tree
{
    [Parameter, EditorRequired] public ObservableCollection<Expandable> Items { get; set; } = null!;
    [Parameter, EditorRequired] public Guid? SelectedId { get; set; }
    [Parameter] public EventCallback<Guid?> SelectedIdChanged { get; set; }

    private async Task HandleSelectionChanged(Guid? id)
    {
        foreach (var expandable in Items)
            expandable.Select(id);

        SelectedId = id;
        await SelectedIdChanged.InvokeAsync(id);
    }
}