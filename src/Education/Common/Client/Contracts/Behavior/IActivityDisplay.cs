namespace Crudspa.Education.Common.Client.Contracts.Behavior;

public interface IActivityDisplay
{
    Activity? Activity { get; set; }
    EventCallback<Guid> ActivityCompleted { get; set; }
    Guid? AssignmentBatchId { get; set; }
    void Reset();
}