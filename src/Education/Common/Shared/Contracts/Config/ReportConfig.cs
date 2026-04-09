namespace Crudspa.Education.Common.Shared.Contracts.Config;

public class ReportConfig : Observable
{
    public enum IdSources { FromUrl, SpecificReport }

    public IdSources IdSource
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ReportId
    {
        get;
        set => SetProperty(ref field, value);
    }
}