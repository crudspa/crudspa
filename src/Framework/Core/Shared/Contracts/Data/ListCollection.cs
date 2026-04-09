namespace Crudspa.Framework.Core.Shared.Contracts.Data;

public class ListCollection<T>
{
    public List<List<T>> Lists { get; set; } = [];
    public List<T> Current { get; set; } = [];
}