namespace Crudspa.Framework.Core.Client.Components;

public partial class Pager
{
    [Parameter, EditorRequired] public Paged Paged { get; set; } = null!;
    [Parameter, EditorRequired] public EventCallback<Int32> PageNumberChanged { get; set; }

    private async Task OnPageChanged(PagerEventArgs args)
    {
        var newPage = args.Skip / Paged.PageSize + 1;
        await PageNumberChanged.InvokeAsync(newPage);
    }
}