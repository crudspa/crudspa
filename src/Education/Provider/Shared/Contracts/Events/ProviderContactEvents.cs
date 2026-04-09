namespace Crudspa.Education.Provider.Shared.Contracts.Events;

public class ProviderContactPayload
{
    public Guid? Id { get; set; }
}

public class ProviderContactAdded : ProviderContactPayload;

public class ProviderContactSaved : ProviderContactPayload;

public class ProviderContactRemoved : ProviderContactPayload;