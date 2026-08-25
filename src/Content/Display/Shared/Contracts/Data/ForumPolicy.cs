namespace Crudspa.Content.Display.Shared.Contracts.Data;

public static class ForumPolicy
{
    public const Int32 MaxForumDescriptionCharacters = 100_000;
    public const Int32 MaxBodyCharacters = 10_000;
    public const Int32 MaxCommentsPerThread = 200;
    public const Int32 MaxReplyDepth = 8;
}