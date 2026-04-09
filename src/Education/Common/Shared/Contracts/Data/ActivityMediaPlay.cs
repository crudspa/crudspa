namespace Crudspa.Education.Common.Shared.Contracts.Data;

public class ActivityMediaPlay : Observable
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? AssignmentBatchId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ActivityId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ActivityChoiceId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public MediaPlay? MediaPlay
    {
        get;
        set => SetProperty(ref field, value);
    }
}