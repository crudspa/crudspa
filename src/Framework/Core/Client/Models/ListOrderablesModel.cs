namespace Crudspa.Framework.Core.Client.Models;

public abstract class ListOrderablesModel<T>(IScrollService scrollService) : ListModel<T>(scrollService)
    where T : class, INamed, IObservable, IOrderable
{
    public Boolean Reordering
    {
        get;
        set => SetProperty(ref field, value);
    }

    public async Task Reorder()
    {
        Filter = String.Empty;
        base.ApplyFilter();

        Reordering = true;

        Cards.Apply(x => x.Reordering = true);

        await Task.CompletedTask;
    }

    public async Task SaveReordering()
    {
        Reordering = false;
        Cards.Apply(x => x.Reordering = false);
        await SaveOrder();
    }

    public async Task CancelReordering()
    {
        Reordering = false;
        Cards.Apply(x => x.Reordering = false);
        await Refresh();
    }

    public void MoveUp(Guid? id)
    {
        var card = Cards.FirstOrDefault(x => Matches(id, x));

        if (card is null)
            return;

        var ordinal = card.Entity.Ordinal;

        if (ordinal <= 0)
            return;

        var previous = Cards.Where(x => x.Entity.Ordinal < ordinal).MaxBy(x => x.Entity.Ordinal);

        if (previous is null)
            return;

        card.Entity.Ordinal = ordinal - 1;
        previous.Entity.Ordinal = ordinal;

        UpdateSort();
    }

    public void MoveDown(Guid? id)
    {
        var card = Cards.FirstOrDefault(x => Matches(id, x));

        if (card is null)
            return;

        var ordinal = card.Entity.Ordinal;

        if (ordinal > Cards.Count - 2)
            return;

        var next = Cards.Where(x => x.Entity.Ordinal > ordinal).MinBy(x => x.Entity.Ordinal);

        if (next is null)
            return;

        card.Entity.Ordinal = ordinal + 1;
        next.Entity.Ordinal = ordinal;

        UpdateSort();
    }

    public abstract Task<Response> SaveOrder();

    public override String OrderBy(CardModel<T> card)
    {
        return card.Entity.Ordinal.GetValueOrDefault().ToString("0000000");
    }

    public override void UpdateSort()
    {
        Cards.Select(x => x.Entity).EnsureOrder();
        base.UpdateSort();
    }
}