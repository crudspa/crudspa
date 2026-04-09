namespace Crudspa.Education.Publisher.Shared.Contracts.Events;

public class ClassRecordingPayload
{
    public Guid? Id { get; set; }
}

public class ClassRecordingAdded : ClassRecordingPayload;

public class ClassRecordingSaved : ClassRecordingPayload;

public class ClassRecordingRemoved : ClassRecordingPayload;