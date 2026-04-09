using System.Collections.Specialized;
using Crudspa.Framework.Core.Shared.Contracts.Config.SegmentType;

namespace Crudspa.Framework.Core.Client.Plugins.SegmentType;

public partial class TabbedPanesDesign : ISegmentDesign, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [CascadingParameter(Name = nameof(ReadOnly))] public Boolean ReadOnly { get; set; }

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public String? ConfigJson { get; set; }
    [Parameter] public ObservableCollection<Pane>? Panes { get; set; }
    [Parameter] public BatchModel<Pane>? PanesModel { get; set; }
    [Parameter] public List<Named>? PermissionNames { get; set; }
    [Parameter] public List<PaneTypeFull>? PaneTypes { get; set; }
    [Parameter] public Action? AddPane { get; set; }

    public TabbedPanesDesignModel? Model { get; set; }
    public TabbedPanesConfig Config { get; set; } = null!;

    protected override void OnInitialized()
    {
        Config = ConfigJson.FromJson<TabbedPanesConfig>() ?? new();

        if (Panes is not null)
        {
            Model = new(Panes);
            Model.PropertyChanged += HandleModelChanged;
        }
    }

    public void Dispose()
    {
        if (Model is null)
            return;

        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }

    public String? GetConfigJson() => Config.ToJson();

    private void HandleAddPaneClicked()
    {
        AddPane?.Invoke();
    }
}

public class TabbedPanesDesignModel : Observable, IDisposable
{
    private void HandlePanesChanged(Object? sender, NotifyCollectionChangedEventArgs args) => RaisePropertyChanged(nameof(Panes));

    public ObservableCollection<Pane> Panes { get; }

    public TabbedPanesDesignModel(ObservableCollection<Pane> panes)
    {
        Panes = panes;
        Panes.CollectionChanged += HandlePanesChanged;
    }

    public void Dispose()
    {
        Panes.CollectionChanged -= HandlePanesChanged;
    }

    public String ActiveKey
    {
        get
        {
            if (field.HasNothing() && Panes.HasItems())
            {
                var item = Panes.OrderBy(x => x.Ordinal).First();
                SetTab(item.Key);
            }

            return field;
        }

        private set => SetProperty(ref field, value);
    } = String.Empty;

    public void SetTab(String? key)
    {
        if (key.HasNothing())
            return;

        var item = Panes.FirstOrDefault(x => x.Key.IsBasically(key));

        if (item is null)
            return;

        ActiveKey = item.Key!;
    }
}