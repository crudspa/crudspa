namespace Crudspa.Education.Publisher.Shared.Contracts.Events;

public class ActivityPayload
{
    public Guid? Id { get; set; }
}

public class ActivityAdded : ActivityPayload;

public class ActivitySaved : ActivityPayload;

public class ActivityRemoved : ActivityPayload;