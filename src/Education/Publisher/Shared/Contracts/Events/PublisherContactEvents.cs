namespace Crudspa.Education.Publisher.Shared.Contracts.Events;

public class PublisherContactPayload
{
    public Guid? Id { get; set; }
}

public class PublisherContactAdded : PublisherContactPayload;

public class PublisherContactSaved : PublisherContactPayload;

public class PublisherContactRemoved : PublisherContactPayload;