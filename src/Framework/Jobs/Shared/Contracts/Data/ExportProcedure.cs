namespace Crudspa.Framework.Jobs.Shared.Contracts.Data;

public class ExportProcedure : Observable
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ProcedureName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateTimeOffset? LastRun
    {
        get;
        set => SetProperty(ref field, value);
    }
}