namespace Crudspa.Education.Publisher.Shared.Contracts.Events;

public class GameSectionPayload
{
    public Guid? Id { get; set; }
    public Guid? GameId { get; set; }
}

public class GameSectionAdded : GameSectionPayload;

public class GameSectionSaved : GameSectionPayload;

public class GameSectionRemoved : GameSectionPayload;

public class GameSectionsReordered : GameSectionPayload;