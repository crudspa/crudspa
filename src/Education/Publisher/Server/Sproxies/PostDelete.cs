using Post = Crudspa.Education.Publisher.Shared.Contracts.Data.Post;

namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class PostDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Post post)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.PostDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", post.Id);

        await command.Execute(connection, transaction);
    }
}