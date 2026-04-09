namespace Crudspa.Education.Student.Server.Sproxies;

public static class ActivityAssignmentSelectPrevious
{
    public static async Task<IList<ActivityAssignment>> Execute(String connection, Guid? studentId, Guid? gameId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.ActivityAssignmentSelectPrevious";

        command.AddParameter("@StudentId", studentId);
        command.AddParameter("@GameId", gameId);

        return await command.ReadAll(connection, ReadActivityAssignment);
    }

    private static ActivityAssignment ReadActivityAssignment(SqlDataReader reader)
    {
        return new()
        {
            ActivityId = reader.ReadGuid(0),
            ActivityKey = reader.ReadString(1),
        };
    }
}