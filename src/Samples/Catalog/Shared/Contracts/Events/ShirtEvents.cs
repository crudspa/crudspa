namespace Crudspa.Samples.Catalog.Shared.Contracts.Events;

public class ShirtPayload
{
    public Guid? Id { get; set; }
}

public class ShirtAdded : ShirtPayload;

public class ShirtSaved : ShirtPayload;

public class ShirtRemoved : ShirtPayload;