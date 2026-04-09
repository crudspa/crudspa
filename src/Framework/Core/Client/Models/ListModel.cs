using System.Collections.Specialized;

namespace Crudspa.Framework.Core.Client.Models;

public abstract class ListModel<T> : ScreenModel
    where T : class, INamed, IObservable
{
    private void HandleCardsChanged(Object? sender, NotifyCollectionChangedEventArgs args)
    {
        if (!_updating) RaisePropertyChanged(nameof(Cards));
    }

    private void HandleEntityChanged(Object? sender, PropertyChangedEventArgs args)
    {
        if (!_updating) RaisePropertyChanged(nameof(Cards));
    }

    private readonly IScrollService _scrollService;
    private ObservableCollection<CardModel<T>> _cards;
    private String? _filter;
    private Boolean _updating;

    protected ListModel(IScrollService scrollService)
    {
        _scrollService = scrollService;

        _cards = [];
        _cards.CollectionChanged += HandleCardsChanged;
    }

    public override void Dispose()
    {
        _cards.CollectionChanged -= HandleCardsChanged;
        _cards.Reset(HandleEntityChanged);

        base.Dispose();
    }

    public ObservableCollection<CardModel<T>> Cards
    {
        get => _cards;
        set => SetProperty(ref _cards, value);
    }

    public String? Filter
    {
        get => _filter;
        set => SetProperty(ref _filter, value);
    }

    public void SetCards(IList<T>? list)
    {
        _updating = true;

        Cards.Reset(HandleEntityChanged, list!.Select(x => new CardModel<T>(x, _scrollService)));

        UpdateSort();
        ApplyFilter();
    }

    public void AddCard(T entity)
    {
        entity.PropertyChanged += HandleEntityChanged;
        Cards.Add(new(entity, _scrollService));

        UpdateSort();
        ApplyFilter();
    }

    public void ReplaceCard(CardModel<T> card, T entity)
    {
        card.Entity.PropertyChanged -= HandleEntityChanged;
        card.Entity = entity;
        card.Entity.PropertyChanged += HandleEntityChanged;

        UpdateSort();
        ApplyFilter();
    }

    public void RemoveCard(CardModel<T> card)
    {
        card.Entity.PropertyChanged -= HandleEntityChanged;
        Cards.Remove(card);
    }

    public async Task Replace(Guid? id, Guid? scopeId = null)
    {
        if (!InScope(scopeId))
            return;

        var response = await Fetch(id);

        if (!response.Ok)
            return;

        var card = Cards.FirstOrDefault(x => Matches(id, x));

        if (card is null)
            AddCard(response.Value);
        else
            ReplaceCard(card, response.Value);
    }

    public async Task Rid(Guid? id, Guid? scopeId = null)
    {
        if (!InScope(scopeId))
            return;

        var card = Cards.FirstOrDefault(x => Matches(id, x));

        if (card is null)
            return;

        RemoveCard(card);

        await Task.CompletedTask;
    }

    public async Task Delete(Guid? id)
    {
        var card = Cards.FirstOrDefault(x => Matches(id, x));

        if (card is null)
            return;

        card.ConfirmationModel.Visible = false;

        await Remove(id);
    }

    // Required overrides

    public abstract Task Refresh(Boolean resetAlerts = true);
    public abstract Task<Response<T?>> Fetch(Guid? id);
    public abstract Task<Response> Remove(Guid? id);

    // Optional overrides

    public virtual Boolean InScope(Guid? scopeId)
    {
        return true;
    }

    public virtual Boolean Matches(Guid? id, CardModel<T> card)
    {
        return card.Entity.Id.Equals(id);
    }

    public virtual String? OrderBy(CardModel<T> card)
    {
        return card.Entity.Name;
    }

    public virtual void ApplyFilter()
    {
        _updating = true;

        if (Filter.HasNothing())
            Cards.Apply(x => x.Hidden = false);
        else
            Cards.Apply(x => x.Hidden = !x.Entity.Name!.Contains(Filter, StringComparison.OrdinalIgnoreCase));

        _updating = false;

        RaisePropertyChanged(nameof(Cards));
    }

    public virtual void UpdateSort()
    {
        var index = 0;

        _updating = true;

        foreach (var card in Cards.OrderBy(OrderBy))
            card.SortIndex = index++;

        _updating = false;

        RaisePropertyChanged(nameof(Cards));
    }

    public void HandleFilterChanged(String? filter)
    {
        _filter = filter;
        ApplyFilter();
    }
}