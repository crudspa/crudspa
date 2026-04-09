namespace Crudspa.Framework.Core.Shared.Extensions;

public static class EnumerableEx
{
    extension<T>(IEnumerable<T>? enumerable)
    {
        public Boolean HasItems()
        {
            return enumerable is not null && enumerable.Any();
        }

        public Boolean IsEmpty()
        {
            return !enumerable.HasItems();
        }

        public Boolean HasAny(Func<T, Boolean> predicate)
        {
            if (enumerable is null)
                return false;

            return enumerable.Any(predicate);
        }

        public void Apply(Action<T> action)
        {
            if (enumerable is null)
                return;

            foreach (var item in enumerable) action(item);
        }

        public ObservableCollection<T> ToObservable()
        {
            return enumerable is null ? [] : new ObservableCollection<T>(enumerable);
        }

        public ObservableCollection<T> ToObservable(Func<T, Boolean> predicate)
        {
            return enumerable is null ? [] : new(enumerable.Where(predicate));
        }

        public IEnumerable<T> DistinctByKey(Func<T, String?> keySelector)
        {
            if (enumerable is null)
                yield break;

            var keys = new HashSet<String>(StringComparer.OrdinalIgnoreCase);

            foreach (var item in enumerable)
            {
                var key = keySelector(item);

                if (key.HasNothing())
                    continue;

                if (keys.Add(key!))
                    yield return item;
            }
        }
    }

    extension<T>(ICollection<T>? collection)
    {
        public Boolean HasItems()
        {
            return collection is not null && collection.Count > 0;
        }

        public Boolean IsEmpty()
        {
            return !collection.HasItems();
        }

        public Boolean HasSomething()
        {
            return collection.HasItems();
        }

        public Boolean HasNothing()
        {
            return !collection.HasItems();
        }
    }

    extension<T>(IList<T> list)
    {
        public IList<T> Shuffle()
        {
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }

            return list;
        }

        public void RemoveWhere(Predicate<T> predicate)
        {
            for (var i = list.Count - 1; i >= 0; i--)
                if (predicate(list[i]))
                    list.RemoveAt(i);
        }
    }

    extension<T>(Collection<T> collection)
    {
        public void RemoveWhere(Func<T, Boolean> predicate)
        {
            for (var i = collection.Count - 1; i >= 0; i--)
                if (predicate(collection[i]))
                    collection.RemoveAt(i);
        }
    }

    extension(IList<String>? list)
    {
        public Boolean Has(String? value)
        {
            if (list is null || list.Count == 0)
                return false;

            if (value.HasNothing())
                return false;

            return list.Any(x => x.IsBasically(value));
        }
    }

    extension(IEnumerable<IOrderable> list)
    {
        public void EnsureOrder()
        {
            var ordinal = 0;
            foreach (var orderable in list.OrderBy(x => x.Ordinal))
                orderable.Ordinal = ordinal++;
        }
    }

    extension(Dictionary<String, String> dictionary)
    {
        public void Set(String key, String value)
        {
            dictionary[key] = value;
        }

        public String? Get(String key)
        {
            return dictionary?.GetValueOrDefault(key);
        }
    }

    extension<T>(ObservableCollection<T> collection)
    {
        public void AddRange(IEnumerable<T> items)
        {
            if (items.HasItems())
                foreach (var item in items)
                    collection.Add(item);
        }
    }

    extension<T>(ObservableCollection<T> collection) where T : class, IOrderable
    {
        public void EnsureOrder()
        {
            if (collection.Count == 0)
                return;

            var normalized = true;

            for (var i = 0; i < collection.Count; i++)
            {
                var ordinal = collection[i].Ordinal;

                if (ordinal is null || ordinal.Value != i)
                {
                    normalized = false;
                    break;
                }
            }

            if (normalized)
                return;

            var ordered = collection.OrderBy(x => x.Ordinal).ToList();

            for (var i = 0; i < ordered.Count; i++)
            {
                var target = ordered[i];
                var currentIndex = collection.IndexOf(target);

                if (currentIndex != i)
                    collection.Move(currentIndex, i);

                collection[i].Ordinal = i;
            }
        }
    }

    extension<T>(ObservableCollection<T> collection) where T : IObservable
    {
        public void Reset(PropertyChangedEventHandler handler, IEnumerable<T>? enumerable = null)
        {
            foreach (var item in collection)
                item.PropertyChanged -= handler;

            collection.Clear();

            if (enumerable is null || enumerable.IsEmpty())
                return;

            foreach (var item in enumerable)
            {
                item.PropertyChanged += handler;
                collection.Add(item);
            }
        }
    }

    extension<TEnum>(TEnum enumValue) where TEnum : Enum
    {
        public String GetName() =>
            Enum.GetName(typeof(TEnum), enumValue) ?? String.Empty;

        public String GetLabel()
        {
            return enumValue.GetName().InsertSpaces();
        }
    }
}