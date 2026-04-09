namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ActivityDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Activity activity)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ActivityDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", activity.Id);

        await command.Execute(connection, transaction);
    }
}