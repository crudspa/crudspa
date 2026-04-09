namespace Crudspa.Education.Publisher.Shared.Contracts.Events;

public class PublisherPayload
{
    public Guid? Id { get; set; }
}

public class PublisherAdded : PublisherPayload;

public class PublisherSaved : PublisherPayload;

public class PublisherRemoved : PublisherPayload;