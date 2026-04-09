namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class PostReactionInsert
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, PostReaction postReaction)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.PostReactionInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PostId", postReaction.PostId);
        command.AddParameter("@Character", 2, postReaction.Character);

        await command.Execute(connection, transaction);
    }
}