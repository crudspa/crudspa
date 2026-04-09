namespace Crudspa.Framework.Core.Client.Components;

public class ReportBase : ComponentBase
{
    [Inject] protected NavigationManager Navigation { get; set; } = null!;
    protected String CurrentUrl { get; private set; } = String.Empty;

    protected override void OnInitialized()
    {
        CurrentUrl = Navigation.Uri;
    }
}