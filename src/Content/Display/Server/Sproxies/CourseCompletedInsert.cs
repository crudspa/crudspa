namespace Crudspa.Content.Display.Server.Sproxies;

public static class CourseCompletedInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, CourseCompleted courseCompleted)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDisplay.CourseCompletedInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@CourseId", courseCompleted.CourseId);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}