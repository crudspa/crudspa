namespace Crudspa.Education.Provider.Shared.Contracts.Events;

public class ProviderPayload
{
    public Guid? Id { get; set; }
}

public class ProviderAdded : ProviderPayload;

public class ProviderSaved : ProviderPayload;

public class ProviderRemoved : ProviderPayload;