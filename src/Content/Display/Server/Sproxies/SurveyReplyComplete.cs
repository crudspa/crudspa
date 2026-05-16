namespace Crudspa.Content.Display.Server.Sproxies;

public static class SurveyReplyComplete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SurveyReply reply)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDisplay.SurveyReplyComplete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", reply.Id);

        await command.Execute(connection, transaction);
    }
}