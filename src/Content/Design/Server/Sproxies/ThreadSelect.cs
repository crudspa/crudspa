using Thread = Crudspa.Content.Display.Shared.Contracts.Data.Thread;

namespace Crudspa.Content.Design.Server.Sproxies;

public static class ThreadSelect
{
    public static async Task<Thread?> Execute(String connection, Guid? sessionId, Thread thread)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.ThreadSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", thread.Id);

        return await command.ReadSingle(connection, ReadThread);
    }

    private static Thread ReadThread(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            ForumId = reader.ReadGuid(1),
            ForumTitle = reader.ReadString(2),
            Title = reader.ReadString(3),
            Pinned = reader.ReadBoolean(4),
            Comment = new()
            {
                Id = reader.ReadGuid(5),
                Body = reader.ReadString(6),
                ById = reader.ReadGuid(7),
                ByName = reader.ReadString(8),
                Posted = reader.ReadDateTimeOffset(9),
                Edited = reader.ReadDateTimeOffset(10),
            },
            CommentCount = reader.ReadInt32(11),
        };
    }
}