using System.Collections.Specialized;

namespace Crudspa.Framework.Core.Client.Models;

public class FindModel<TSearch, TEntity> : ScreenModel
    where TSearch : Search, new()
    where TEntity : ICountable, INamed, IObservable
{
    private async void HandleSearchChanged(Object? sender, PropertyChangedEventArgs args)
    {
        try
        {
            await Refresh();
        }
        catch
        {
            // avoid compiler warning on `async void`
        }
    }

    private void HandleCardsChanged(Object? sender, NotifyCollectionChangedEventArgs args)
    {
        if (!_updating) RaisePropertyChanged(nameof(Cards));
    }

    private void HandleCardChanged(Object? sender, PropertyChangedEventArgs args)
    {
        if (!_updating) RaisePropertyChanged(nameof(Cards));
    }

    private readonly IScrollService _scrollService;
    private TSearch _search;
    private ObservableCollection<CardModel<TEntity>> _cards;
    private Boolean _updating;

    public FindModel(IScrollService scrollService)
    {
        _scrollService = scrollService;
        _search = new();
        _search.PropertyChanged += HandleSearchChanged;

        _cards = [];
        _cards.CollectionChanged += HandleCardsChanged;
    }

    public override void Dispose()
    {
        _search.PropertyChanged -= HandleSearchChanged;
        _cards.CollectionChanged -= HandleCardsChanged;
        _cards.Reset(HandleCardChanged);

        base.Dispose();
    }

    public Task ShowConfirmation(Guid? entityId)
    {
        var card = _cards.FirstOrDefault(x => x.Entity.Id.Equals(entityId));

        if (card is not null)
            card.ConfirmationModel.Visible = true;

        return Task.CompletedTask;
    }

    public async Task HandlePageNumberChanged(Int32 pageNumber)
    {
        if (!Search.Paged.PageNumber.Equals(pageNumber))
        {
            Search.Paged.PageNumber = pageNumber;
            await Refresh();
        }
    }

    public TSearch Search
    {
        get => _search;
        set => SetProperty(ref _search, value);
    }

    public ObservableCollection<CardModel<TEntity>> Cards
    {
        get => _cards;
        set => SetProperty(ref _cards, value);
    }

    public void SetCards(IList<TEntity>? list)
    {
        Search.Paged.TotalCount = list!.HasItems() ? list!.First().TotalCount.GetValueOrDefault() : 0;

        _updating = true;
        _cards.Reset(HandleCardChanged, list!.Select(x => new CardModel<TEntity>(x, _scrollService)));
        _updating = false;
        RaisePropertyChanged(nameof(Cards));
    }

    public virtual Task Refresh(Boolean resetAlerts = true)
    {
        return Task.CompletedTask;
    }
}