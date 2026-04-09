namespace Crudspa.Samples.Catalog.Shared.Contracts.Data;

public class ShirtSearch : Search
{

    public List<Guid?> Brands
    {
        get;
        set => SetProperty(ref field, value);
    } = [];
}