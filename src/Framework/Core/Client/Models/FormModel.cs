namespace Crudspa.Framework.Core.Client.Models;

public class FormModel<T> : ScreenModel, ICardModel<T>
    where T : IObservable, INamed
{
    private void HandleEntityChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(Entity));

    private T _entity;
    private Boolean _isNew;
    private Boolean _readOnly;
    private Int32? _sortIndex;

    public FormModel(T entity, IScrollService scrollService, Boolean isNew = false)
    {
        _isNew = isNew;
        _readOnly = !isNew;
        _sortIndex = -1;

        _entity = entity;
        _entity.PropertyChanged += HandleEntityChanged;

        ConfirmationModel = new(scrollService);
        ConfirmationModel.PropertyChanged += HandleEntityChanged;
    }

    public override void Dispose()
    {
        _entity.PropertyChanged -= HandleEntityChanged;
        ConfirmationModel.PropertyChanged -= HandleEntityChanged;
        base.Dispose();
    }

    public T Entity
    {
        get => _entity;
        set => SetProperty(ref _entity, value);
    }

    public ModalModel ConfirmationModel { get; }

    public Boolean Hidden
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean IsNew
    {
        get => _isNew;
        set => SetProperty(ref _isNew, value);
    }

    public Boolean ReadOnly
    {
        get => _readOnly;
        set => SetProperty(ref _readOnly, value);
    }

    public Int32? SortIndex
    {
        get => _sortIndex;
        set => SetProperty(ref _sortIndex, value);
    }

    public Boolean Reordering
    {
        get;
        set => SetProperty(ref field, value);
    }
}