namespace Crudspa.Content.Display.Server.Sproxies;

public static class TrackCompletedInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Guid? courseId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDisplay.TrackCompletedInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@CourseId", courseId);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}