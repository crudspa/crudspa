namespace Crudspa.Framework.Core.Client.Models;

public sealed class TabsModel(INavigator navigator, String path, String queryKey = "pane", Boolean lockOnNew = false)
    : Observable
{
    public ObservableCollection<Tab> Tabs
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public String? ActiveKey
    {
        get;
        private set
        {
            if (SetProperty(ref field, value))
                navigator.UpdateQueryParameter(path, queryKey, field!);
        }
    } = String.Empty;

    public void Initialize(IEnumerable<Tab> tabs)
    {
        Tabs = tabs.DistinctByKey(x => x.Key).ToObservable();

        ApplyPath(path);

        if (ActiveKey.HasNothing() && Tabs.HasItems())
            SetActive(Tabs.First().Key);
    }

    public void ApplyPath(String pathToApply)
    {
        if (lockOnNew && ActiveKey.HasSomething())
            return;

        var parameters = navigator.GetQueryParameters(pathToApply);
        var requested = parameters[queryKey];

        if (requested.HasSomething())
            SetActive(requested);
    }

    public void SetActive(String? key)
    {
        if (key.HasNothing())
            return;

        var tab = Tabs.FirstOrDefault(x => x.Key.IsBasically(key));

        if (tab is null)
            return;

        tab.Loaded = true;

        ActiveKey = tab.Key;
    }
}