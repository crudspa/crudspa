namespace Crudspa.Framework.Core.Client.Models;

public class CardModel<T> : Observable, IDisposable, ICardModel<T>
    where T : INamed, IObservable
{
    private void HandleEntityChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(Entity));

    private T _entity;

    public CardModel(T entity, IScrollService scrollService)
    {
        _entity = entity;
        _entity.PropertyChanged += HandleEntityChanged;

        ConfirmationModel = new(scrollService);
        ConfirmationModel.PropertyChanged += HandleEntityChanged;
    }

    public void Dispose()
    {
        _entity.PropertyChanged -= HandleEntityChanged;
        ConfirmationModel.PropertyChanged -= HandleEntityChanged;
        ConfirmationModel.Dispose();
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

    public Int32 SortIndex
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean Reordering
    {
        get;
        set => SetProperty(ref field, value);
    }
}