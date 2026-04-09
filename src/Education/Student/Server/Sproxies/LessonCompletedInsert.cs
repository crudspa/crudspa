namespace Crudspa.Education.Student.Server.Sproxies;

public static class LessonCompletedInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Guid? objectiveId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.LessonCompletedInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ObjectiveId", objectiveId);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}