namespace Crudspa.Content.Design.Server.Sproxies;

public static class ForumSelect
{
    public static async Task<Forum?> Execute(String connection, Guid? sessionId, Forum forum)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.ForumSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", forum.Id);

        return await command.ExecuteQuery(connection, async reader =>
        {
            if (!await reader.ReadAsync())
                return null;

            forum = ReadForum(reader);

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                forum.Licenses.Add(reader.ReadSelectable());

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
            {
                forum.ForumBundles.Add(new()
                {
                    ForumId = reader.ReadGuid(0),
                    BundleId = reader.ReadGuid(1),
                    BundleName = reader.ReadString(2),
                    ThreadRule = reader.ReadEnum<ForumBundle.Rules>(3),
                    CommentRule = reader.ReadEnum<ForumBundle.Rules>(4),
                });
            }

            return forum;
        });
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
            PermissionId = reader.ReadGuid(14),
            PermissionName = reader.ReadString(15),
            AccessMode = reader.ReadEnum<Forum.AccessModes>(16),
            Ordinal = reader.ReadInt32(17),
        };
    }
}