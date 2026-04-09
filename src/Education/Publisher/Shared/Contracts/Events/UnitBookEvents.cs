namespace Crudspa.Education.Publisher.Shared.Contracts.Events;

public class UnitBookPayload
{
    public Guid? Id { get; set; }
    public Guid? UnitId { get; set; }
}

public class UnitBookAdded : UnitBookPayload;

public class UnitBookSaved : UnitBookPayload;

public class UnitBookRemoved : UnitBookPayload;

public class UnitBooksReordered : UnitBookPayload;