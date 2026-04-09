namespace Crudspa.Education.School.Shared.Contracts.Events;

public class PostReactionPayload
{
    public Guid? PostId { get; set; }
    public Guid? ContactId { get; set; }
    public String? Character { get; set; }
}

public class PostReactionAdded : PostReactionPayload;