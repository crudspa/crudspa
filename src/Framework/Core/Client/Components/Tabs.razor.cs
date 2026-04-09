using G = System.Collections.Generic;

namespace Crudspa.Framework.Core.Client.Components;

public partial class Tabs : IDisposable, IHandle<QueryStringChanged>
{
    private readonly G.List<Crudspa.Framework.Core.Client.Contracts.Data.Tab> _tabs = [];

    private TabsModel? _childModel;
    private String? _childQueryKey;
    private String? _childPath;
    private Boolean _childLockOnNew;

    [Parameter, EditorRequired] public String Path { get; set; } = String.Empty;
    [Parameter] public String? QueryKey { get; set; }
    [Parameter] public Boolean LockOnNew { get; set; }
    [Parameter] public Boolean HideTabs { get; set; }
    [Parameter] public Boolean Vertical { get; set; }
    [Parameter] public TabsModel? Model { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;

    [CascadingParameter] public TabScope? Scope { get; set; }

    public TabsModel? ActiveModel => Model ?? _childModel;
    private Boolean _building;

    protected override Task OnInitializedAsync()
    {
        if (Model is not null)
            Model.PropertyChanged += HandleModelChanged;

        EventBus.Subscribe(this);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        if (Model is not null)
            Model.PropertyChanged -= HandleModelChanged;

        if (_childModel is not null)
            _childModel.PropertyChanged -= HandleModelChanged;
    }

    protected override void OnParametersSet()
    {
        if (Model is not null && ChildContent is not null)
            throw new("Tabs cannot use both Model and ChildContent.");

        if (Model is not null)
        {
            Model.ApplyPath(Path);
            return;
        }

        if (ChildContent is null)
            return;

        var scope = Scope ?? TabScope.Root;
        var desiredKey = QueryKey.HasSomething() ? QueryKey! : scope.Key;

        var rebuild = _childModel is null
            || !_childQueryKey.IsBasically(desiredKey)
            || !_childPath.IsBasically(Path)
            || _childLockOnNew != LockOnNew;

        if (!rebuild)
            return;

        _tabs.Clear();

        if (_childModel is not null)
            _childModel.PropertyChanged -= HandleModelChanged;

        _childModel = null;
        _childQueryKey = desiredKey;
        _childPath = Path;
        _childLockOnNew = LockOnNew;
    }

    protected override void OnAfterRender(Boolean firstRender)
    {
        if (ChildContent is null)
            return;

        if (_childModel is not null)
            return;

        if (_tabs.Count == 0)
            return;

        EnsureChildModel();
        StateHasChanged();
    }

    internal void Add(Crudspa.Framework.Core.Client.Contracts.Data.Tab tab)
    {
        if (tab.Key.HasNothing())
            return;

        var existing = _tabs.FindIndex(x => x.Key.IsBasically(tab.Key));

        if (existing >= 0)
            _tabs[existing] = tab;
        else
            _tabs.Add(tab);
    }

    private void EnsureChildModel()
    {
        var scope = Scope ?? TabScope.Root;
        var key = QueryKey.HasSomething() ? QueryKey! : scope.Key;

        _childQueryKey = key;

        _childModel = new(Navigator, Path, queryKey: key, lockOnNew: LockOnNew);
        _childModel.PropertyChanged += HandleModelChanged;

        _building = true;
        try
        {
            _childModel.Initialize(_tabs);
        }
        finally
        {
            _building = false;
        }
    }

    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args)
    {
        if (_building)
            return;

        InvokeAsync(StateHasChanged);
    }

    public Task Handle(QueryStringChanged payload)
    {
        var model = ActiveModel;

        if (model is null)
            return Task.CompletedTask;

        if (payload.Path.IsBasically(Path))
            model.ApplyPath(payload.Path);

        return Task.CompletedTask;
    }
}