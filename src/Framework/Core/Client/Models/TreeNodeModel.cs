namespace Crudspa.Framework.Core.Client.Models;

public class TreeNodeModel<T> : Observable, IDisposable
    where T : class, INamed, IObservable
{
    private void HandleFormChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(Form));

    private readonly FormModel<T> _form;
    private ObservableCollection<TreeNodeModel<T>> _children = [];

    public TreeNodeModel(FormModel<T> form, Guid? parentId, Int32 depth)
    {
        _form = form;
        _form.PropertyChanged += HandleFormChanged;
        ParentId = parentId;
        Depth = depth;
    }

    public void Dispose()
    {
        _form.PropertyChanged -= HandleFormChanged;

        foreach (var child in Children)
            child.Dispose();

        _form.Dispose();
    }

    public FormModel<T> Form => _form;

    public T Entity => Form.Entity;

    public Guid? ParentId { get; }

    public Int32 Depth { get; }

    public ObservableCollection<TreeNodeModel<T>> Children
    {
        get => _children;
        set => SetProperty(ref _children, value);
    }

    public Boolean Expanded
    {
        get;
        set => SetProperty(ref field, value);
    } = true;
}