namespace Crudspa.Education.Publisher.Shared.Contracts.Events;

public class ReadParagraphPayload
{
    public Guid? Id { get; set; }
    public Guid? ReadPartId { get; set; }
}

public class ReadParagraphAdded : ReadParagraphPayload;

public class ReadParagraphSaved : ReadParagraphPayload;

public class ReadParagraphRemoved : ReadParagraphPayload;

public class ReadParagraphsReordered : ReadParagraphPayload;