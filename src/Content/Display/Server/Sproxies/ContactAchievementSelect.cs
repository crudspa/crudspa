namespace Crudspa.Content.Display.Server.Sproxies;

public static class ContactAchievementSelect
{
    public static async Task<ContactAchievement?> Execute(String connection, Guid? sessionId, ContactAchievement contactAchievement)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDisplay.ContactAchievementSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", contactAchievement.Id);

        return await command.ReadSingle(connection, ReadContactAchievement);
    }

    private static ContactAchievement ReadContactAchievement(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            ContactId = reader.ReadGuid(1),
            Earned = reader.ReadDateTimeOffset(2),
            Achievement = new()
            {
                Id = reader.ReadGuid(3),
                Title = reader.ReadString(4),
                Description = reader.ReadString(5),
                ImageId = reader.ReadGuid(6),
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
            },
        };
    }
}