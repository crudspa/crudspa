using System.Collections.Specialized;
using System.Web;
using Microsoft.AspNetCore.Components.Routing;

namespace Crudspa.Framework.Core.Client.Services;

public class NavigatorCore(
    NavigationManager navigationManager,
    INavigationInterception interceptor,
    IEventBus eventBus,
    ISessionState sessionState)
    : Observable, INavigator, IDisposable
{
    private IDisposable? _locationWire;
    private IScrollService? _scrollService;
    private String _appTitlePrefix = null!;
    private Boolean _isBouncing;

    public Boolean Initialized
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String AppTitle
    {
        get;
        set => SetProperty(ref field, value);
    } = null!;

    public String ScreenTitle
    {
        get;
        set => SetProperty(ref field, value);
    } = String.Empty;

    public Boolean NavigationVisible
    {
        get;
        set
        {
            if (SetProperty(ref field, value))
                _scrollService?.ToTop();
        }
    }

    public ObservableCollection<NavSegment> Registrations { get; set; } = [];
    public ObservableCollection<Screen> Screens { get; set; } = [];
    public ObservableCollection<Screen> OpenViews { get; set; } = [];

    public async Task Initialize(IScrollService scrollService, String appTitlePrefix, Boolean sessionsPersist = true)
    {
        SetApp(scrollService, appTitlePrefix);

        await interceptor.EnableNavigationInterceptionAsync();

        Wire();

        await sessionState.Initialize(sessionsPersist: sessionsPersist);
    }

    public void Dispose()
    {
        Unwire();
    }

    private void SetApp(IScrollService scrollService, String appTitlePrefix)
    {
        _scrollService = scrollService;
        _appTitlePrefix = appTitlePrefix;
        AppTitle = _appTitlePrefix;
    }

    private void Wire()
    {
        if (_locationWire is not null)
            return;

        _locationWire = navigationManager.RegisterLocationChangingHandler(OnLocationChanging);
        navigationManager.LocationChanged += OnLocationChanged;
        sessionState.SessionInitialized += OnSessionInitialized;
        Screens.CollectionChanged += OnScreensChanged;
        OpenViews.CollectionChanged += OnViewsChanged;
        WireScreens(Screens);
        WireViews(OpenViews);
    }

    private void Unwire()
    {
        _locationWire?.Dispose();
        _locationWire = null;

        navigationManager.LocationChanged -= OnLocationChanged;
        sessionState.SessionInitialized -= OnSessionInitialized;
        Screens.CollectionChanged -= OnScreensChanged;
        OpenViews.CollectionChanged -= OnViewsChanged;

        UnwireViews(OpenViews);
        UnwireScreens(Screens);
    }

    private void WireScreens(IEnumerable<Screen> screens)
    {
        foreach (var screen in screens)
        {
            screen.PropertyChanged -= OnScreenChanged;
            screen.PropertyChanged += OnScreenChanged;
            WireScreens(screen.Children);
        }
    }

    private void UnwireScreens(IEnumerable<Screen> screens)
    {
        foreach (var screen in screens)
        {
            screen.PropertyChanged -= OnScreenChanged;
            UnwireScreens(screen.Children);
        }
    }

    private void WireViews(IEnumerable<Screen> screens)
    {
        foreach (var screen in screens)
        {
            screen.PropertyChanged -= OnViewChanged;
            screen.PropertyChanged += OnViewChanged;
        }
    }

    private void UnwireViews(IEnumerable<Screen> screens)
    {
        foreach (var screen in screens)
            screen.PropertyChanged -= OnViewChanged;
    }

    private void ClearViews()
    {
        UnwireViews(OpenViews);
        OpenViews.Clear();
    }

    private void OnViewsChanged(Object? sender, NotifyCollectionChangedEventArgs args) =>
        RaisePropertyChanged(nameof(OpenViews));

    private void OnViewChanged(Object? sender, PropertyChangedEventArgs args) =>
        RaisePropertyChanged(nameof(OpenViews));

    private void OnScreensChanged(Object? sender, NotifyCollectionChangedEventArgs args) =>
        RaisePropertyChanged(nameof(Screens));

    private void OnScreenChanged(Object? sender, PropertyChangedEventArgs args) =>
        RaisePropertyChanged(nameof(Screens));

    private void OnSessionInitialized(Object? sender, EventArgs args)
    {
        Initialized = false;

        LoadSession();
        OpenCurrent();

        Initialized = true;
    }

    private void LoadSession()
    {
        ClearViews();
        UnwireScreens(Screens);

        Registrations.Clear();
        Screens.Clear();

        Registrations.AddRange(sessionState.Session.Segments);
        Screens.AddRange(sessionState.Session.Screens);

        if (Screens.IsEmpty())
            AddFixed();

        WireScreens(Screens);
    }

    private void OpenCurrent()
    {
        var path = navigationManager.ToBaseRelativePath(navigationManager.Uri);

        if (path.HasNothing() && Screens.HasItems())
            GoToRoot();
        else
            Open(path);
    }

    public void Bounce()
    {
        _isBouncing = true;
        navigationManager.NavigateTo(navigationManager.BaseUri, true);
    }

    public void GoTo(String path)
    {
        navigationManager.NavigateTo(path);
    }

    public void GoToRoot()
    {
        var firstScreen = Screens.FirstOrDefault();

        if (firstScreen is not null)
            GoTo(firstScreen.Path!);
    }

    public void UpdateTitle(String? path, String? title)
    {
        if (path is null)
            return;

        var screen = FindScreen(path);

        if (screen is null)
            return;

        var safeTitle = title.HasSomething() ? title : "[Title]";

        screen.Title = safeTitle;

        var view = OpenViews.FirstOrDefault(x => x.Path.IsBasically(path));

        if (view is not null)
        {
            view.Title = safeTitle;

            if (view.Visible)
            {
                ScreenTitle = safeTitle;
                AppTitle = _appTitlePrefix + " | " + safeTitle;
            }
        }
    }

    public void UpdateQueryParameter(String path, String key, String value)
    {
        var screen = FindScreen(path);

        if (screen is null)
            return;

        var collection = new NameValueCollection();

        if (screen.Query.HasSomething())
        {
            var queryParameters = HttpUtility.ParseQueryString(screen.Query!);

            foreach (var paramKey in queryParameters.AllKeys)
                if (!paramKey.IsBasically(key) && queryParameters[paramKey].HasSomething())
                    collection.Add(paramKey, queryParameters[paramKey]);
        }

        collection.Add(key, value);
        var query = "?" + String.Join("&", collection.AllKeys.Select(paramKey => $"{paramKey}={collection[paramKey]}"));
        screen.Query = query;

        var view = OpenViews.FirstOrDefault(x => x.Path.IsBasically(path));

        if (view is not null && view.Visible)
            GoTo(path + query);
    }

    public NameValueCollection GetQueryParameters(String path)
    {
        var screen = FindScreen(path);

        if (screen is null || screen.Query.HasNothing())
            return new();

        return HttpUtility.ParseQueryString(screen.Query!);
    }

    private static String GetQueryString(String path)
    {
        var questionMark = path.IndexOf('?');
        return questionMark > 0 ? path.Substring(questionMark) : String.Empty;
    }

    private static String GetScreenPath(String path)
    {
        var query = GetQueryString(path);
        return (query.HasSomething() ? path.RemoveLast(query) : path).TrimEnd('/');
    }

    private ValueTask OnLocationChanging(LocationChangingContext args)
    {
        if (_isBouncing)
            return ValueTask.CompletedTask;

        var path = args.TargetLocation.StartsWith('/')
            ? args.TargetLocation
            : "/" + navigationManager.ToBaseRelativePath(args.TargetLocation);

        if (!IsRegistered(path))
            args.PreventNavigation();

        return ValueTask.CompletedTask;
    }

    private void OnLocationChanged(Object? sender, LocationChangedEventArgs args)
    {
        if (_isBouncing)
            return;

        var path = navigationManager.ToBaseRelativePath(args.Location);
        if (!path.StartsWith('/')) path = "/" + path;

        var currentPath = OpenViews.FirstOrDefault(x => x.Visible)?.Path;
        var nextPath = GetScreenPath(path);
        var shouldScrollToTop = !nextPath.IsBasically(currentPath);
        var wasNavigationVisible = NavigationVisible;

        _ = Silence();

        Open(path);

        NavigationVisible = false;

        if (shouldScrollToTop && !wasNavigationVisible)
            _ = _scrollService?.ToTop() ?? Task.CompletedTask;
    }

    private async Task Silence()
    {
        try
        {
            await eventBus.Publish(new SilenceRequested());
        }
        catch
        {
            // this is fine
        }
    }

    private Boolean IsRegistered(String path)
    {
        var screen = FindScreen(path, true);
        return screen is not null;
    }

    private void Open(String path)
    {
        if (!path.StartsWith('/')) path = "/" + path;

        var screen = FindScreen(path, true);

        if (screen is null)
            return;

        EnsureView(screen);

        Show(screen.Path!);

        var query = GetQueryString(path);

        if (query.HasSomething() && !query.IsBasically(screen.Query))
        {
            screen.Query = query;
            eventBus.Publish(new QueryStringChanged { Path = screen.Path! });
        }
    }

    public void Close(String? path = null)
    {
        path ??= "/" + navigationManager.ToBaseRelativePath(navigationManager.Uri);

        var screen = FindScreen(path);

        if (screen is null || screen.Fixed)
            return;

        var wasVisible = HasVisible(screen);
        var nextScreen = FindNextScreen(screen);

        Remove(screen);

        if (nextScreen is not null)
            GoTo(nextScreen.Path!);
        else if (wasVisible)
            GoToRoot();
    }

    private void EnsureView(Screen screen)
    {
        var view = OpenViews.FirstOrDefault(x => x.Path.IsBasically(screen.Path));

        if (view is not null)
            return;

        view = new()
        {
            Id = screen.Id,
            Key = screen.Key,
            Title = screen.Title,
            Path = screen.Path,
            View = screen.View,
            Icon = screen.Icon,
            Fixed = screen.Fixed,
            Panes = screen.Panes,
            ConfigJson = screen.ConfigJson,
        };

        WireViews([view]);
        OpenViews.Add(view);
    }

    private void Show(String path)
    {
        foreach (var screen in OpenViews)
        {
            screen.Visible = screen.Path.IsBasically(path);

            if (screen.Visible)
            {
                ScreenTitle = screen.Title ?? String.Empty;
                AppTitle = _appTitlePrefix + (ScreenTitle.HasSomething() ? " | " + ScreenTitle : String.Empty);
            }
        }

        MarkVisible(path, Screens);
    }

    private Screen? FindNextScreen(Screen screen)
    {
        if (!HasVisible(screen))
            return null;

        var parent = FindParent(screen);

        if (parent is not null)
            return parent;

        var indexOfCurrent = Screens.IndexOf(screen);

        if (indexOfCurrent < 1)
            return null;

        return Screens[indexOfCurrent - 1];
    }

    private static Boolean HasVisible(Screen screen)
    {
        if (screen.Visible)
            return true;

        foreach (var child in screen.Children)
            if (HasVisible(child))
                return true;

        return false;
    }

    private Screen? FindParent(Screen screen, ObservableCollection<Screen>? screens = null)
    {
        screens ??= Screens;

        foreach (var candidate in screens)
        {
            if (candidate.Children.Contains(screen))
                return candidate;

            var parent = FindParent(screen, candidate.Children);

            if (parent is not null)
                return parent;
        }

        return null;
    }

    private void Remove(Screen screen, ObservableCollection<Screen>? screens = null)
    {
        screens ??= Screens;

        if (!screens.Contains(screen))
            foreach (var child in screens)
                Remove(screen, child.Children);
        else
        {
            RemoveChildren(screen.Children);
            RemoveView(screen);

            screen.PropertyChanged -= OnScreenChanged;
            screens.Remove(screen);
        }

        RaisePropertyChanged(nameof(Screens));
    }

    private void RemoveChildren(ObservableCollection<Screen> children)
    {
        foreach (var child in children)
        {
            RemoveChildren(child.Children);
            RemoveView(child);
            child.Children = [];
        }
    }

    private void RemoveView(Screen screen)
    {
        var view = OpenViews.FirstOrDefault(x => x.Path.IsBasically(screen.Path));

        if (view is not null)
        {
            view.PropertyChanged -= OnViewChanged;
            OpenViews.Remove(view);
        }
    }

    private static void MarkVisible(String path, ObservableCollection<Screen> screens)
    {
        foreach (var screen in screens)
        {
            screen.Visible = screen.Path.IsBasically(path);
            MarkVisible(path, screen.Children);
        }
    }

    private void AddFixed(String? path = null, IEnumerable<NavSegment>? registrations = null, ICollection<Screen>? screens = null)
    {
        path ??= String.Empty;
        registrations ??= Registrations;
        screens ??= Screens;

        foreach (var registration in registrations.Where(x => x.Fixed == true))
        {
            var urlPath = path + "/" + registration.Key;

            var screen = new Screen
            {
                Key = registration.Key,
                Title = registration.RequiresId == true ? "..." : registration.Title,
                Path = urlPath,
                View = registration.TypeDisplayView,
                Icon = registration.IconName,
                Fixed = registration.Fixed ?? false,
                Panes = registration.Panes.DeepClone(),
                ConfigJson = registration.ConfigJson,
            };

            screens.Add(screen);

            AddFixed(urlPath, registration.Segments, screen.Children);
        }
    }

    private Screen? FindScreen(String path, Boolean create = false)
    {
        // If there's a query string, move it out of the path variable
        var query = GetQueryString(path);
        if (query.HasSomething()) path = path.RemoveLast(query);

        path = path.TrimEnd('/');

        // Walk the url by segment, while also walking the registration & screen trees
        var registrations = Registrations;
        var screens = Screens;
        var position = 0;

        while (true)
        {
            if (path.Length <= position)
                return null;

            // Build the screen key, path, and registration key for this segment
            var nextSlash = path.IndexOf('/', position + 1);
            var atEnd = nextSlash < 0;

            var key = atEnd ? path.Substring(position + 1) : path.Substring(position + 1, nextSlash - position - 1);
            var urlPath = atEnd ? path : path.Substring(0, nextSlash);

            var registrationKey = key.Key();
            var id = key.Id();

            // See if we have a registration for this segment
            var registration = registrations.FirstOrDefault(x => x.Key.IsBasically(registrationKey)
                && (x.RequiresId != true || x.RequiresId == true && id is not null));

            if (registration is null)
                return null;

            // See if we have a screen in the tree already for this segment
            var screen = screens.FirstOrDefault(x => x.Key.IsBasically(key));

            // If not, create one
            if (screen is null && create)
            {
                screen = new()
                {
                    Id = id,
                    Key = key,
                    Title = registration.RequiresId == true ? "..." : registration.Title,
                    Path = urlPath,
                    Query = atEnd ? query : String.Empty,
                    View = registration.TypeDisplayView,
                    Icon = registration.IconName,
                    Fixed = registration.Fixed ?? false,
                    Panes = registration.Panes.DeepClone(),
                    ConfigJson = registration.ConfigJson,
                };

                screen.PropertyChanged += OnScreenChanged;

                screens.Add(screen);

                EnsureView(screen);
            }

            // We're done, return what we have
            if (atEnd || screen is null)
                return screen;

            // If any of the current registrations are marked as recursive, add them as children
            foreach (var recursiveRegistration in registrations.Where(x => x.Recursive == true).ToList())
                if (!registration.Segments.Any(x => x.Key.IsBasically(recursiveRegistration.Key)))
                    registration.Segments.Add(recursiveRegistration);

            // Prepare to walk the next segment
            registrations = registration.Segments;
            screens = screen.Children;
            position = nextSlash;
        }
    }
}