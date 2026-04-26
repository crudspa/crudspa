using System.Collections.Specialized;

namespace Crudspa.Framework.Core.Client.Models;

public abstract class TreeModel<T> : ScreenModel
    where T : class, INamed, IObservable
{
    private void HandleRootsChanged(Object? sender, NotifyCollectionChangedEventArgs args)
    {
        if (!_updating)
            RaisePropertyChanged(nameof(Roots));
    }

    private void HandleNodeChanged(Object? sender, PropertyChangedEventArgs args)
    {
        if (!_updating)
            RaisePropertyChanged(nameof(Roots));
    }

    protected internal readonly IScrollService ScrollService;

    private readonly Func<T, IEnumerable<T>?> _childrenSelector;
    private ObservableCollection<TreeNodeModel<T>> _roots = [];
    private Boolean _updating;

    protected TreeModel(IScrollService scrollService, Func<T, IEnumerable<T>?> childrenSelector)
    {
        ScrollService = scrollService;
        _childrenSelector = childrenSelector;
        _roots.CollectionChanged += HandleRootsChanged;
    }

    public override void Dispose()
    {
        _roots.CollectionChanged -= HandleRootsChanged;
        ClearRoots();
        base.Dispose();
    }

    public ObservableCollection<TreeNodeModel<T>> Roots
    {
        get => _roots;
        set => SetProperty(ref _roots, value);
    }

    public void SetRoots(IEnumerable<T>? roots)
    {
        _updating = true;

        _roots.CollectionChanged -= HandleRootsChanged;
        ClearRoots();

        _roots = (roots ?? [])
            .Select(x => CreateNodeModel(x, null, 0))
            .ToObservable();

        _roots.CollectionChanged += HandleRootsChanged;

        _updating = false;

        RaisePropertyChanged(nameof(Roots));
    }

    public async Task CreateRoot()
    {
        await Create(null);
    }

    public async Task<TreeNodeModel<T>> CreateNode(T entity, Guid? parentId)
    {
        var node = CreateNodeModel(entity, parentId, parentId.HasValue ? FindNode(parentId)?.Depth + 1 ?? 0 : 0, true);

        if (!parentId.HasValue)
            Roots.Add(node);
        else
        {
            var parent = FindNode(parentId);

            if (parent is null)
                Roots.Add(node);
            else
            {
                parent.Expanded = true;
                parent.Children.Add(node);
            }
        }

        RaisePropertyChanged(nameof(Roots));

        if (entity.Id.HasValue)
            await ScrollService.ToId(entity.Id.Value);

        return node;
    }

    public TreeNodeModel<T>? FindNode(Guid? id)
    {
        if (!id.HasValue)
            return null;

        return FindNode(Roots, id.Value);
    }

    public async Task Save(Guid? id)
    {
        var node = FindNode(id);

        if (node is null)
            return;

        if (node.Form.IsNew)
        {
            var response = await node.Form.WithWaiting("Adding...", () => Add(node.Form));

            if (response.Ok)
            {
                RemoveNode(node);
                await Refresh(false);
            }

            return;
        }

        var saveResponse = await node.Form.WithWaiting("Saving...", () => Save(node.Form));

        if (saveResponse.Ok)
        {
            node.Form.ReadOnly = true;
            await Refresh(false);
        }
    }

    public async Task Cancel(Guid? id)
    {
        var node = FindNode(id);

        if (node is null)
            return;

        if (node.Form.IsNew)
        {
            RemoveNode(node);
            return;
        }

        await Refresh();
    }

    public async Task Delete(Guid? id)
    {
        var node = FindNode(id);

        if (node is null)
            return;

        await node.Form.ConfirmationModel.Hide();
        var response = await node.Form.WithWaiting("Deleting...", () => Remove(id));

        if (response.Ok)
            await Refresh(false);
    }

    protected void RemoveNode(TreeNodeModel<T> node)
    {
        if (!node.ParentId.HasValue)
        {
            Roots.Remove(node);
            Unsubscribe(node);
            node.Dispose();
            RaisePropertyChanged(nameof(Roots));
            return;
        }

        var parent = FindNode(node.ParentId);

        if (parent is null)
        {
            Roots.Remove(node);
            Unsubscribe(node);
            node.Dispose();
            RaisePropertyChanged(nameof(Roots));
            return;
        }

        parent.Children.Remove(node);
        Unsubscribe(node);
        node.Dispose();
        RaisePropertyChanged(nameof(Roots));
    }

    public abstract Task Create(Guid? parentId);
    public abstract Task Refresh(Boolean resetAlerts = true);
    public abstract Task<Response<T?>> Add(FormModel<T> form);
    public abstract Task<Response> Save(FormModel<T> form);
    public abstract Task<Response> Remove(Guid? id);

    public virtual Boolean InScope(Guid? scopeId) => true;

    private TreeNodeModel<T> CreateNodeModel(T entity, Guid? parentId, Int32 depth, Boolean isNew = false)
    {
        var form = new FormModel<T>(entity, ScrollService, isNew);
        var node = new TreeNodeModel<T>(form, parentId, depth);
        Subscribe(node);

        foreach (var child in _childrenSelector(entity) ?? [])
            node.Children.Add(CreateNodeModel(child, entity.Id, depth + 1));

        return node;
    }

    private TreeNodeModel<T>? FindNode(IEnumerable<TreeNodeModel<T>> nodes, Guid id)
    {
        foreach (var node in nodes)
        {
            if (node.Entity.Id.Equals(id))
                return node;

            var child = FindNode(node.Children, id);

            if (child is not null)
                return child;
        }

        return null;
    }

    private void ClearRoots()
    {
        foreach (var root in _roots)
        {
            Unsubscribe(root);
            root.Dispose();
        }

        _roots.Clear();
    }

    private void Subscribe(TreeNodeModel<T> node)
    {
        node.PropertyChanged += HandleNodeChanged;

        foreach (var child in node.Children)
            Subscribe(child);
    }

    private void Unsubscribe(TreeNodeModel<T> node)
    {
        node.PropertyChanged -= HandleNodeChanged;

        foreach (var child in node.Children)
            Unsubscribe(child);
    }
}