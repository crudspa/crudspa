namespace Crudspa.Content.Design.Server.Sproxies;

public static class SurveyPartDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SurveyPart part)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.SurveyPartDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", part.Id);

        await command.Execute(connection, transaction);
    }
}