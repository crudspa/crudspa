namespace Crudspa.Content.Display.Server.Sproxies;

public static class TrackSelectRun
{
    public static async Task<Track?> Execute(String connection, Guid? trackId, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDisplay.TrackSelectRun";

        command.AddParameter("@Id", trackId);
        command.AddParameter("@SessionId", sessionId);

        return await command.ExecuteQuery(connection, async reader =>
        {
            if (!await reader.ReadAsync())
                return null;

            var track = ReadTrack(reader);

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                track.Courses.Add(ReadCourse(reader));

            return track;
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