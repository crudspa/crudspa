namespace Crudspa.Education.Student.Server.Sproxies;

public static class ObjectiveProgressSelectAll
{
    public static async Task<IList<ObjectiveProgress>> Execute(String connection, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.ObjectiveProgressSelectAll";

        command.AddParameter("@SessionId", sessionId);

        return await command.ReadAll(connection, ReadObjectiveProgress);
    }

    public static ObjectiveProgress ReadObjectiveProgress(SqlDataReader reader)
    {
        return new()
        {
            StudentId = reader.ReadGuid(0),
            ObjectiveId = reader.ReadGuid(1),
            TimesCompleted = reader.ReadInt32(2),
        };
    }
}