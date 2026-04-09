namespace Crudspa.Education.District.Shared.Contracts.Data;

public class SchoolSearch : Search
{
    public List<Guid?> Communities
    {
        get;
        set => SetProperty(ref field, value);
    } = [];
}