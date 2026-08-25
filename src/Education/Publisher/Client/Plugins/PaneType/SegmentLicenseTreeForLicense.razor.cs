namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

using License = Shared.Contracts.Data.License;

public partial class SegmentLicenseTreeForLicense : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public ILicenseService LicenseService { get; set; } = null!;
    [Inject] public ISegmentService SegmentService { get; set; } = null!;

    public SegmentLicenseTreeForLicenseModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Id, LicenseService, SegmentService);
        Model.PropertyChanged += HandleModelChanged;
        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class SegmentLicenseTreeForLicenseModel(
    Guid? licenseId,
    ILicenseService licenseService,
    ISegmentService segmentService) : ScreenModel
{
    private HashSet<Guid> _originalSelection = [];
    private Int32 _removedSegmentCount;

    public License? License
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<Expandable> Portals
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public String? SearchText
    {
        get;
        set
        {
            if (!SetProperty(ref field, value))
                return;

            ExpandMatches();
            RaiseFilterProperties();
        }
    }

    public Boolean? SelectedOnly
    {
        get;
        set
        {
            if (!SetProperty(ref field, value))
                return;

            if (value == true)
                ExpandSelected();

            RaiseFilterProperties();
        }
    }

    public Int32 SelectedCount => Flatten().Count(x => x.Key.HasSomething() && x.Selected == true);
    public Int32 UniversalCount => Flatten().Count(x => x.Key.HasSomething() && x.ReadOnly == true);
    public Int32 RemovedSegmentCount => _removedSegmentCount;
    public Int32 ChangeCount => CurrentSelection().SymmetricExceptCount(_originalSelection);
    public Boolean HasVisibleSegments => Portals.Any(IsVisible);

    public async Task Initialize()
    {
        await Refresh();
    }

    public async Task Refresh()
    {
        var licenseTask = WithAlerts(() => licenseService.Fetch(new(new() { Id = licenseId })), false);
        var nestTask = WithAlerts(() => segmentService.FetchNest(new()), false);

        await WithMany("Loading license segments...", licenseTask, nestTask);

        var licenseResponse = await licenseTask;
        var nestResponse = await nestTask;

        if (!licenseResponse.Ok || !nestResponse.Ok || licenseResponse.Value is null)
            return;

        License = licenseResponse.Value;
        Portals = nestResponse.Value.ToObservable();

        var selected = License.Segments
            .Where(x => x.Selected == true && x.Id.HasValue)
            .Select(x => x.Id!.Value)
            .ToHashSet();

        foreach (var portal in Portals)
        {
            portal.Expanded = true;
            ApplySelection(portal, selected);
        }

        foreach (var selectedId in selected)
            SelectAncestors(selectedId);

        _originalSelection = selected;
        _removedSegmentCount = Math.Max(0, (License.SegmentLicenseCount ?? 0) - selected.Count);
        ExpandSelected();
        RaiseAllProperties();
    }

    public async Task Save()
    {
        if (License is null)
            return;

        var selected = CurrentSelection();

        foreach (var segment in License.Segments)
            segment.Selected = segment.Id.HasValue && selected.Contains(segment.Id.Value);

        var response = await WithWaiting("Saving license segments...", () => licenseService.Save(new(License)));

        if (!response.Ok)
            return;

        _originalSelection = selected;
        RaiseAllProperties();
    }

    public void ChangeSelection(Expandable node)
    {
        if (node.ReadOnly == true)
            return;

        if (node.Selected == true)
            SelectAncestors(node.Id);
        else
            SetDescendants(node, false);

        RaiseAllProperties();
    }

    public void IncludeBranch(Expandable node)
    {
        SetDescendants(node, true);
        SelectAncestors(node.Id);
        node.Expanded = true;
        RaiseAllProperties();
    }

    public void ExcludeBranch(Expandable node)
    {
        SetDescendants(node, false);
        RaiseAllProperties();
    }

    public void ExpandSelected()
    {
        foreach (var portal in Portals)
            ExpandWhere(portal, x => x.Selected == true);

        RaisePropertyChanged(nameof(Portals));
    }

    public void CollapseAll()
    {
        foreach (var node in Flatten())
            node.Expanded = false;

        RaisePropertyChanged(nameof(Portals));
    }

    public Boolean IsVisible(Expandable node)
    {
        var matchesSelection = SelectedOnly != true || node.Selected == true;
        var matchesSearch = SearchText.HasNothing() || MatchesSearch(node);

        return (matchesSelection && matchesSearch) || node.Children.Any(IsVisible);
    }

    public Boolean IsIndeterminate(Expandable node)
    {
        var descendants = Flatten(node.Children)
            .Where(x => x.Key.HasSomething())
            .ToList();

        if (descendants.Count == 0)
            return false;

        var included = descendants.Count(x => x.Selected == true || x.ReadOnly == true);
        return included > 0 && included < descendants.Count;
    }

    private void ApplySelection(Expandable node, HashSet<Guid> selected)
    {
        node.Selected = node.Id.HasValue && selected.Contains(node.Id.Value);

        foreach (var child in node.Children)
            ApplySelection(child, selected);
    }

    private void SetDescendants(Expandable node, Boolean selected)
    {
        if (node.Key.HasSomething() && node.ReadOnly != true)
            node.Selected = selected;

        foreach (var child in node.Children)
            SetDescendants(child, selected);
    }

    private void SelectAncestors(Guid? id)
    {
        foreach (var portal in Portals)
        {
            var path = new List<Expandable>();

            if (!FindPath(portal, id, path))
                continue;

            foreach (var ancestor in path.Where(x => x.Key.HasSomething() && x.ReadOnly != true))
                ancestor.Selected = true;

            return;
        }
    }

    private static Boolean FindPath(Expandable node, Guid? id, IList<Expandable> path)
    {
        path.Add(node);

        if (node.Id.Equals(id))
            return true;

        foreach (var child in node.Children)
            if (FindPath(child, id, path))
                return true;

        path.RemoveAt(path.Count - 1);
        return false;
    }

    private void ExpandMatches()
    {
        if (SearchText.HasNothing())
            return;

        foreach (var portal in Portals)
            ExpandWhere(portal, MatchesSearch);
    }

    private static Boolean ExpandWhere(Expandable node, Func<Expandable, Boolean> predicate)
    {
        var descendantMatches = false;

        foreach (var child in node.Children)
            descendantMatches |= ExpandWhere(child, predicate);

        if (descendantMatches)
            node.Expanded = true;

        return predicate(node) || descendantMatches;
    }

    private Boolean MatchesSearch(Expandable node)
    {
        return node.Name?.Contains(SearchText!, StringComparison.OrdinalIgnoreCase) == true ||
               node.Key?.Contains(SearchText!, StringComparison.OrdinalIgnoreCase) == true;
    }

    private HashSet<Guid> CurrentSelection()
    {
        return Flatten()
            .Where(x => x.Key.HasSomething() && x.Selected == true && x.Id.HasValue)
            .Select(x => x.Id!.Value)
            .ToHashSet();
    }

    private IEnumerable<Expandable> Flatten() => Flatten(Portals);

    private static IEnumerable<Expandable> Flatten(IEnumerable<Expandable> nodes)
    {
        foreach (var node in nodes)
        {
            yield return node;

            foreach (var child in Flatten(node.Children))
                yield return child;
        }
    }

    private void RaiseFilterProperties()
    {
        RaisePropertyChanged(nameof(Portals));
        RaisePropertyChanged(nameof(HasVisibleSegments));
    }

    private void RaiseAllProperties()
    {
        RaisePropertyChanged(nameof(Portals));
        RaisePropertyChanged(nameof(SelectedCount));
        RaisePropertyChanged(nameof(UniversalCount));
        RaisePropertyChanged(nameof(RemovedSegmentCount));
        RaisePropertyChanged(nameof(ChangeCount));
        RaisePropertyChanged(nameof(HasVisibleSegments));
    }
}

internal static class SegmentLicenseSetExtensions
{
    public static Int32 SymmetricExceptCount<T>(this HashSet<T> values, HashSet<T> other)
    {
        var copy = values.ToHashSet();
        copy.SymmetricExceptWith(other);
        return copy.Count;
    }
}