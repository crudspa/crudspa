namespace Crudspa.Content.Display.Server.Sproxies;

public static class SurveyRunSelect
{
    public static async Task<Survey?> Execute(String connection, Guid? sessionId, Survey survey)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDisplay.SurveyRunSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", survey.Id);

        return await command.ExecuteQuery(connection, SurveyDataReaders.ReadSurveyWithQuestions);
    }
}