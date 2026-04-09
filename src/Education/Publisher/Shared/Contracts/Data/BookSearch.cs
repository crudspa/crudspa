namespace Crudspa.Education.Publisher.Shared.Contracts.Data;

public class BookSearch : Search
{
    public List<Guid?> Status
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Guid?> Seasons
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Guid?> Categories
    {
        get;
        set => SetProperty(ref field, value);
    } = [];
}