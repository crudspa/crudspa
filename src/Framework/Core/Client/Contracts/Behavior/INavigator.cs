using System.Collections.Specialized;

namespace Crudspa.Framework.Core.Client.Contracts.Behavior;

public interface INavigator : IObservable
{
    Boolean Initialized { get; set; }
    String AppTitle { get; set; }
    String ScreenTitle { get; set; }
    Boolean NavigationVisible { get; set; }
    ObservableCollection<Screen> Screens { get; }
    ObservableCollection<Screen> OpenViews { get; }
    Task Initialize(IScrollService scrollService, String appTitlePrefix, Boolean sessionsPersist);
    void Bounce();
    void GoTo(String path);
    void GoToRoot();
    void Close(String? path = null);
    void UpdateTitle(String? path, String? title);
    void UpdateQueryParameter(String path, String key, String value);
    NameValueCollection GetQueryParameters(String path);
}