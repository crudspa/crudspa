namespace Crudspa.Content.Design.Shared.Contracts.Events;

public class SectionPayload
{
    public Guid? Id { get; set; }
    public Guid? PageId { get; set; }
}

public class SectionAdded : SectionPayload;

public class SectionSaved : SectionPayload;

public class SectionRemoved : SectionPayload;

public class SectionsReordered : SectionPayload;