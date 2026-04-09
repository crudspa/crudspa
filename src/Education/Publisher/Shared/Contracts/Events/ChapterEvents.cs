namespace Crudspa.Education.Publisher.Shared.Contracts.Events;

public class ChapterPayload
{
    public Guid? Id { get; set; }
    public Guid? BookId { get; set; }
}

public class ChapterAdded : ChapterPayload;

public class ChapterSaved : ChapterPayload;

public class ChapterRemoved : ChapterPayload;

public class ChaptersReordered : ChapterPayload;