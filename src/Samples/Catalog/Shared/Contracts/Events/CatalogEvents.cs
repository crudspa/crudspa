namespace Crudspa.Samples.Catalog.Shared.Contracts.Events;

public class CatalogPayload
{
    public Guid? Id { get; set; }
}

public class CatalogAdded : CatalogPayload;

public class CatalogSaved : CatalogPayload;

public class CatalogRemoved : CatalogPayload;