namespace Crudspa.Education.Student.Server.Sproxies;

public static class UnitAllLessonsAreComplete
{
    public static async Task<Boolean> Execute(String connection, Guid? sessionId, Guid? unitId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.UnitAllLessonsAreComplete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@UnitId", unitId);

        return await command.ExecuteBoolean(connection, "@AllAreComplete");
    }
}