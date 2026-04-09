namespace Crudspa.Education.Publisher.Shared.Contracts.Data;

public class DistrictContactSearch : Search
{
    public List<Guid?> Districts
    {
        get;
        set => SetProperty(ref field, value);
    } = [];
}