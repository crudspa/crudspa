namespace Crudspa.Education.Publisher.Shared.Contracts.Data;

public class GameActivityShare : Observable
{
    public Guid? SourceGameActivityId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? TargetGameSectionId
    {
        get;
        set => SetProperty(ref field, value);
    }
}