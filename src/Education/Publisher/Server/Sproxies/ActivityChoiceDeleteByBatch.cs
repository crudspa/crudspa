namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ActivityChoiceDeleteByBatch
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ActivityChoice activityChoice)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ActivityChoiceDeleteByBatch";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", activityChoice.Id);

        await command.Execute(connection, transaction);
    }
}