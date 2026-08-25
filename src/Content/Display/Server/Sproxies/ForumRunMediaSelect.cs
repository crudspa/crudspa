namespace Crudspa.Content.Display.Server.Sproxies;

public static class ForumRunMediaSelect
{
    public static async Task<ForumMedia?> Execute(String connection, Guid? sessionId, Guid? id, IEnumerable<Guid?> licenseIds)
    {
        await using var command = new SqlCommand { CommandText = "ContentDisplay.ForumRunMediaSelect" };
        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", id);
        command.AddParameter("@LicenseIds", licenseIds);

        return await command.ReadSingle(connection, reader => new ForumMedia
        {
            BlobId = reader.ReadGuid(0),
            Name = reader.ReadString(1),
            Format = reader.ReadString(2),
        });
    }
}