namespace Crudspa.Content.Design.Server.Sproxies;

public static class TrackSelectForPortal
{
    public static async Task<IList<Track>> Execute(String connection, Guid? sessionId, Guid? portalId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.TrackSelectForPortal";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PortalId", portalId);

        return await command.ReadAll(connection, ReadTrack);
    }

    private static Track ReadTrack(SqlDataReader reader)
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
            RequiresAchievementId = reader.ReadGuid(7),
            RequiresAchievementTitle = reader.ReadString(8),
            GeneratesAchievementId = reader.ReadGuid(9),
            GeneratesAchievementTitle = reader.ReadString(10),
            RequireSequentialCompletion = reader.ReadBoolean(11),
            Ordinal = reader.ReadInt32(12),
            CourseCount = reader.ReadInt32(13),
        };
    }
}