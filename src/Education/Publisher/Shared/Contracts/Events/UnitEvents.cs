namespace Crudspa.Education.Publisher.Shared.Contracts.Events;

public class UnitPayload
{
    public Guid? Id { get; set; }
}

public class UnitAdded : UnitPayload;

public class UnitSaved : UnitPayload;

public class UnitRemoved : UnitPayload;

public class UnitsReordered : UnitPayload;