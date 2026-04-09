namespace Crudspa.Content.Display.Server.Sproxies;

public static class TrackSelectAll
{
    public static async Task<PortalTracks?> Execute(String connection, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDisplay.TrackSelectAll";

        command.AddParameter("@SessionId", sessionId);

        return await command.ExecuteQuery(connection, async reader =>
        {
            var portal = new PortalTracks();

            while (await reader.ReadAsync())
                portal.Tracks.Add(ReadTrack(reader));

            await reader.NextResultAsync();

            var courses = new List<Course>();

            while (await reader.ReadAsync())
                courses.Add(ReadCourse(reader));

            foreach (var track in portal.Tracks)
                track.Courses = courses.ToObservable(x => x.TrackId.Equals(track.Id));

            return portal;
        });
    }

    private static Track ReadTrack(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Title = reader.ReadString(1),
            Description = reader.ReadString(2),
            StatusId = reader.ReadGuid(3),
            RequireSequentialCompletion = reader.ReadBoolean(4),
            Ordinal = reader.ReadInt32(5),
        };
    }

    private static Course ReadCourse(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Title = reader.ReadString(1),
            Description = reader.ReadString(2),
            TrackId = reader.ReadGuid(3),
            Ordinal = reader.ReadInt32(4),
            Binder = new()
            {
                Id = reader.ReadGuid(5),
            },
        };
    }
}