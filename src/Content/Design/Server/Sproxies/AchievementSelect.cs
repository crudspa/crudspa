namespace Crudspa.Content.Design.Server.Sproxies;

public static class AchievementSelect
{
    public static async Task<Achievement?> Execute(String connection, Guid? sessionId, Achievement achievement)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.AchievementSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", achievement.Id);

        return await command.ReadSingle(connection, ReadAchievement);
    }

    private static Achievement ReadAchievement(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            PortalId = reader.ReadGuid(1),
            Title = reader.ReadString(2),
            Description = reader.ReadString(3),
            ImageFile = new()
            {
                Id = reader.ReadGuid(4),
                BlobId = reader.ReadGuid(5),
                Name = reader.ReadString(6),
                Format = reader.ReadString(7),
                Width = reader.ReadInt32(8),
                Height = reader.ReadInt32(9),
                Caption = reader.ReadString(10),
            },
        };
    }
}