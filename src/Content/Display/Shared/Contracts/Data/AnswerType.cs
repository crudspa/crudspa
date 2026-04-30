namespace Crudspa.Content.Display.Shared.Contracts.Data;

public class AnswerType : Observable, INamed
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Name
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? DesignView
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? DisplayView
    {
        get;
        set => SetProperty(ref field, value);
    }
}