namespace Crudspa.Content.Display.Server.Sproxies;

public static class ForumRunReactionSet
{
    public static async Task Execute(String connection, Guid? sessionId, CommentReaction reaction, IEnumerable<Guid?> licenseIds)
    {
        await using var command = new SqlCommand { CommandText = "ContentDisplay.ForumRunReactionSet" };
        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@CommentId", reaction.CommentId);
        command.AddParameter("@Emoji", 4, reaction.Emoji);
        command.AddParameter("@LicenseIds", licenseIds);
        await command.Execute(connection);
    }
}