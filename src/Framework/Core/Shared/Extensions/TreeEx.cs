namespace Crudspa.Framework.Core.Shared.Extensions;

public static class TreeEx
{
    public static ObservableCollection<T> BuildTree<T>(
        this IEnumerable<T> items,
        Func<T, Guid?> id,
        Func<T, Guid?> parentId,
        Action<T, ObservableCollection<T>> setChildren)
        where T : class
    {
        var list = items.ToList();
        var nodesById = list
            .Where(x => id(x).HasValue)
            .GroupBy(x => id(x)!.Value)
            .ToDictionary(x => x.Key, x => x.First());
        var childrenById = nodesById.Keys.ToDictionary(x => x, _ => new ObservableCollection<T>());

        foreach (var item in list)
            setChildren(item, id(item).HasValue ? childrenById[id(item)!.Value] : []);

        var roots = new ObservableCollection<T>();

        foreach (var item in list)
        {
            var currentParentId = parentId(item);

            if (!currentParentId.HasValue || !nodesById.TryGetValue(currentParentId.Value, out var parent))
            {
                roots.Add(item);
                continue;
            }

            childrenById[currentParentId.Value].Add(item);
        }

        return roots;
    }
}