namespace Crudspa.Education.Publisher.Shared.Contracts.Data;

public class AchievementSearch : Search
{
    public List<Guid?> Rarities
    {
        get;
        set => SetProperty(ref field, value);
    } = [];
}