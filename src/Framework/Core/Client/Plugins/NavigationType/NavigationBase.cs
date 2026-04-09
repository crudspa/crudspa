using Microsoft.AspNetCore.Components.Web;

namespace Crudspa.Framework.Core.Client.Plugins.NavigationType;

public class NavigationBase : ComponentBase, IDisposable, INavigationDisplay
{
    private void HandleNavigatorChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);
    public void HandleNodeClosed(String path) => Navigator.Close(path);
    public void HandleClick(MouseEventArgs args) => ClickService.Click(args);

    [Parameter, EditorRequired] public Portal Portal { get; set; } = null!;
    [Parameter] public RenderFragment? Branding { get; set; }
    [Parameter] public RenderFragment? Footer { get; set; }
    [Parameter] public RenderFragment? AuthViews { get; set; }
    [Parameter] public String? Styles { get; set; }

    [Inject] public IClickService ClickService { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        await Navigator.Initialize(ScrollService, Portal.Title!, Portal.SessionsPersist.GetValueOrDefault());
        Navigator.PropertyChanged += HandleNavigatorChanged;
    }

    public void Dispose()
    {
        Navigator.PropertyChanged -= HandleNavigatorChanged;
    }
}