namespace Crudspa.Samples.Catalog.Shared.Contracts.Events;

public class ShirtOptionPayload
{
    public Guid? Id { get; set; }
    public Guid? ShirtId { get; set; }
}

public class ShirtOptionAdded : ShirtOptionPayload;

public class ShirtOptionSaved : ShirtOptionPayload;

public class ShirtOptionRemoved : ShirtOptionPayload;

public class ShirtOptionRelationsSaved : ShirtOptionPayload;

public class ShirtOptionsReordered : ShirtOptionPayload;