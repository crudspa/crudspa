namespace Crudspa.Framework.Jobs.Shared.Contracts.Data;

public class SanitizeHtmlRun : Observable
{
    public Int32 RowsScanned
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32 RowsUpdated
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32 RowsSkippedRequired
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<SanitizeHtmlTargetRun> Targets
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<Guid> AffectedPageIds
    {
        get;
        set => SetProperty(ref field, value);
    } = [];
}

public class SanitizeHtmlTargetRun : Observable
{
    public String Name
    {
        get;
        set => SetProperty(ref field, value);
    } = String.Empty;

    public Int32 RowsScanned
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32 RowsUpdated
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32 RowsSkippedRequired
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<Guid> AffectedPageIds
    {
        get;
        set => SetProperty(ref field, value);
    } = [];
}