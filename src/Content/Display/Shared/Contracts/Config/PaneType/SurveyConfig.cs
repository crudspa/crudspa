namespace Crudspa.Content.Display.Shared.Contracts.Config.PaneType;

public class SurveyConfig : Observable
{
    public Guid? SurveyId
    {
        get;
        set => SetProperty(ref field, value);
    }
}