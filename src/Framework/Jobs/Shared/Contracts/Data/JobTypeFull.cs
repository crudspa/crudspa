namespace Crudspa.Framework.Jobs.Shared.Contracts.Data;

public class JobTypeFull : Named
{
    public String? EditorView
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ActionClass
    {
        get;
        set => SetProperty(ref field, value);
    }
}