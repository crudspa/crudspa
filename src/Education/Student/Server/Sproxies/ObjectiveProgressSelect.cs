namespace Crudspa.Education.Student.Server.Sproxies;

public static class ObjectiveProgressSelect
{
    public static async Task<ObjectiveProgress> Execute(String connection, Guid? sessionId, Guid? objectiveId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.ObjectiveProgressSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ObjectiveId", objectiveId);

        var progress = await command.ReadSingle(connection, ReadObjectiveProgress);

        return progress ?? new ObjectiveProgress
        {
            ObjectiveId = objectiveId,
            TimesCompleted = 0,
        };
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