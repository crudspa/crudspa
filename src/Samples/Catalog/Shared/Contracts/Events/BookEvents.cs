namespace Crudspa.Samples.Catalog.Shared.Contracts.Events;

public class BookPayload
{
    public Guid? Id { get; set; }
}

public class BookAdded : BookPayload;

public class BookSaved : BookPayload;

public class BookRemoved : BookPayload;