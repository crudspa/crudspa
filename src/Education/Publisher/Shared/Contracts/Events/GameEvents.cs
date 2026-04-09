namespace Crudspa.Education.Publisher.Shared.Contracts.Events;

public class GamePayload
{
    public Guid? Id { get; set; }
    public Guid? BookId { get; set; }
}

public class GameAdded : GamePayload;

public class GameSaved : GamePayload;

public class GameRemoved : GamePayload;