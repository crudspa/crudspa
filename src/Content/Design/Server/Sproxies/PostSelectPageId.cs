namespace Crudspa.Content.Design.Server.Sproxies;

public static class PostSelectPageId
{
    public static async Task<Post?> Execute(String connection, Guid? sessionId, Post post)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.PostSelectPageId";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", post.Id);

        return await command.ReadSingle(connection, ReadPost);
    }

    private static Post ReadPost(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            PageId = reader.ReadGuid(1),
        };
    }
}