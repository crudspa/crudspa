namespace Crudspa.Samples.Catalog.Shared.Contracts.Events;

public class CatalogContactPayload
{
    public Guid? Id { get; set; }
}

public class CatalogContactAdded : CatalogContactPayload;

public class CatalogContactSaved : CatalogContactPayload;

public class CatalogContactRemoved : CatalogContactPayload;