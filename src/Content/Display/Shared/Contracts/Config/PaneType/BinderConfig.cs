namespace Crudspa.Content.Display.Shared.Contracts.Config.PaneType;

public class BinderConfig : Observable
{
    public Guid? BinderId
    {
        get;
        set => SetProperty(ref field, value);
    }
}