namespace Crudspa.Content.Design.Server.Sproxies;

public static class SurveyDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Survey survey)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.SurveyDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", survey.Id);

        await command.Execute(connection, transaction);
    }
}