namespace Crudspa.Content.Display.Server.Sproxies;

public static class QuestionReplySelectForElement
{
    public static async Task<QuestionReply?> Execute(String connection, Guid? sessionId, Guid? elementId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDisplay.QuestionReplySelectForElement";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ElementId", elementId);

        return await command.ExecuteQuery(connection, SurveyDataReaders.ReadQuestionReplyWithChoices);
    }
}