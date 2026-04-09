namespace Crudspa.Framework.Core.Shared.Contracts.Data;

public class ContactNotebook : Observable, IUnique
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ContactId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? NotebookId
    {
        get;
        set => SetProperty(ref field, value);
    }
}