using Forum = Crudspa.Education.Publisher.Shared.Contracts.Data.Forum;

namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ForumDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Forum forum)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ForumDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", forum.Id);

        await command.Execute(connection, transaction);
    }
}