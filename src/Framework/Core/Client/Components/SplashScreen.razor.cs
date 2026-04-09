namespace Crudspa.Framework.Core.Client.Components;

public partial class SplashScreen : IDisposable
{
    [Inject] public INavigator Navigator { get; set; } = null!;

    protected override void OnInitialized()
    {
        Navigator.PropertyChanged += HandleNavigatorChanged;
    }

    public void Dispose()
    {
        Navigator.PropertyChanged -= HandleNavigatorChanged;
    }

    private void HandleNavigatorChanged(Object? sender, PropertyChangedEventArgs args) =>
        _ = InvokeAsync(StateHasChanged);
}