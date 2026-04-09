namespace Crudspa.Framework.Core.Client.Components;

public partial class Sorter
{
    [Parameter, EditorRequired] public IEnumerable<String> Sorts { get; set; } = null!;
    [Parameter, EditorRequired] public String Selected { get; set; } = null!;
    [Parameter, EditorRequired] public Boolean? Ascending { get; set; } = true;
    [Parameter] public EventCallback<String> SelectedChanged { get; set; }
    [Parameter] public EventCallback<Boolean?> AscendingChanged { get; set; }

    public readonly Guid InstanceId = Guid.NewGuid();

    private async Task HandleSortChanged(String sort)
    {
        Selected = sort;
        await SelectedChanged.InvokeAsync(sort);
    }

    private async Task HandleAscendingChanged()
    {
        Ascending = !Ascending;
        await AscendingChanged.InvokeAsync(Ascending);
    }
}