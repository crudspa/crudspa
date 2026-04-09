namespace Crudspa.Education.Student.Server.Sproxies;

public static class UnitSelectBySession
{
    public static async Task<IList<Unit>> Execute(String connection, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.UnitSelectBySession";

        command.AddParameter("@SessionId", sessionId);

        return await command.ReadAll(connection, ReadUnit);
    }

    public static Unit ReadUnit(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Title = reader.ReadString(1),
            GeneratesAchievementId = reader.ReadGuid(2),
            RequiresAchievementId = reader.ReadGuid(3),
            Image = new()
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