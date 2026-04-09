namespace Crudspa.Content.Design.Server.Sproxies;

public static class ForumSelect
{
    public static async Task<Forum?> Execute(String connection, Guid? sessionId, Forum forum)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.ForumSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", forum.Id);

        return await command.ReadSingle(connection, ReadForum);
    }

    private static Forum ReadForum(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            PortalId = reader.ReadGuid(1),
            PortalKey = reader.ReadString(2),
            Title = reader.ReadString(3),
            StatusId = reader.ReadGuid(4),
            StatusName = reader.ReadString(5),
            Description = reader.ReadString(6),
            ImageFile = new()
            {
                Id = reader.ReadGuid(7),
                BlobId = reader.ReadGuid(8),
                Name = reader.ReadString(9),
                Format = reader.ReadString(10),
                Width = reader.ReadInt32(11),
                Height = reader.ReadInt32(12),
                Caption = reader.ReadString(13),
            },
            Ordinal = reader.ReadInt32(14),
        };
    }
}