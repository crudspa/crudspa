namespace Crudspa.Education.Student.Server.Sproxies;

public static class TrifoldAllAreComplete
{
    public static async Task<Boolean> Execute(String connection, Guid? sessionId, Guid? trifoldId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.TrifoldAllAreComplete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@TrifoldId", trifoldId);

        return await command.ExecuteBoolean(connection, "@AllAreComplete");
    }
}