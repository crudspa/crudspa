namespace Crudspa.Samples.Catalog.Shared.Contracts.Data;

public class BookSearch : Search
{

    public List<Guid?> Genres
    {
        get;
        set => SetProperty(ref field, value);
    } = [];
}