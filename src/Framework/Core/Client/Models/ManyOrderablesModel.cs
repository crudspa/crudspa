namespace Crudspa.Framework.Core.Client.Models;

public abstract class ManyOrderablesModel<T>(IScrollService scrollService) : ManyModel<T>(scrollService)
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

        Forms.RemoveWhere(x => x.IsNew);

        Forms.Apply(x =>
        {
            x.Reordering = true;
            x.ReadOnly = true;
        });

        await Task.CompletedTask;
    }

    public async Task SaveReordering()
    {
        Reordering = false;
        Forms.Apply(x => x.Reordering = false);
        await SaveOrder();
    }

    public async Task CancelReordering()
    {
        Reordering = false;
        Forms.Apply(x => x.Reordering = false);
        await Refresh();
    }

    public void MoveUp(Guid? id)
    {
        var form = Forms.FirstOrDefault(x => Matches(id, x));

        if (form is null)
            return;

        var ordinal = form.Entity.Ordinal;

        if (ordinal <= 0)
            return;

        var previous = Forms.Where(x => x.Entity.Ordinal < ordinal).MaxBy(x => x.Entity.Ordinal);

        if (previous is null)
            return;

        form.Entity.Ordinal = ordinal - 1;
        previous.Entity.Ordinal = ordinal;

        UpdateSort();
    }

    public void MoveDown(Guid? id)
    {
        var form = Forms.FirstOrDefault(x => Matches(id, x));

        if (form is null)
            return;

        var ordinal = form.Entity.Ordinal;

        if (ordinal > Forms.Count - 2)
            return;

        var next = Forms.Where(x => x.Entity.Ordinal > ordinal).MinBy(x => x.Entity.Ordinal);

        if (next is null)
            return;

        form.Entity.Ordinal = ordinal + 1;
        next.Entity.Ordinal = ordinal;

        UpdateSort();
    }

    public abstract Task<Response> SaveOrder();

    public override String OrderBy(FormModel<T> form)
    {
        return form.Entity.Ordinal.GetValueOrDefault().ToString("0000000");
    }
}