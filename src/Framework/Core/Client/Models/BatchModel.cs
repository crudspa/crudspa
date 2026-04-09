namespace Crudspa.Framework.Core.Client.Models;

public class BatchModel<T>(Boolean ascending = true) : EditModel<T>(false)
    where T : class, IObservable, IOrderable
{
    public ObservableCollection<T> Entities
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public virtual String? OrderBy(T entity)
    {
        return entity.Ordinal?.ToString("D10");
    }

    public virtual void UpdateSort()
    {
        var index = 0;

        if (ascending)
            foreach (var entity in Entities.OrderBy(OrderBy))
                entity.Ordinal = index++;
        else
            foreach (var entity in Entities.OrderByDescending(OrderBy))
                entity.Ordinal = index++;

        RaisePropertyChanged(nameof(Entities));
    }

    public void SetEntities(IEnumerable<T> entities)
    {
        Entities.Clear();
        foreach (var entity in entities)
            Entities.Add(entity);
        UpdateSort();
    }

    public void AddEntity(T entity)
    {
        Entities.Add(entity);
        UpdateSort();
    }

    public void MoveUp(Guid? id)
    {
        if (id is null) return;

        Entities.EnsureOrder();

        var index = IndexOfId(id.Value);
        if (index <= 0) return;

        var entity = Entities[index];
        var previous = Entities[index - 1];

        entity.Ordinal = index - 1;
        previous.Ordinal = index;

        Entities.Move(index, index - 1);

        RaisePropertyChanged(nameof(Entities));
    }

    public void MoveDown(Guid? id)
    {
        if (id is null) return;

        Entities.EnsureOrder();

        var index = IndexOfId(id.Value);
        if (index < 0 || index >= Entities.Count - 1) return;

        var entity = Entities[index];
        var next = Entities[index + 1];

        entity.Ordinal = index + 1;
        next.Ordinal = index;

        Entities.Move(index, index + 1);

        RaisePropertyChanged(nameof(Entities));
    }

    public void Remove(Guid? id)
    {
        if (id is null) return;

        var index = IndexOfId(id.Value);
        if (index < 0) return;

        Entities.RemoveAt(index);

        for (var i = index; i < Entities.Count; i++)
            Entities[i].Ordinal = i;

        RaisePropertyChanged(nameof(Entities));
    }

    private Int32 IndexOfId(Guid id)
    {
        for (var i = 0; i < Entities.Count; i++)
            if (Entities[i].Id.Equals(id))
                return i;
        return -1;
    }
}