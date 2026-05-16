using Crudspa.Content.Display.Server;

namespace Crudspa.Content.Design.Server.Sproxies;

public static class SurveyQuestionSelect
{
    public static async Task<SurveyQuestion?> Execute(String connection, Guid? sessionId, SurveyQuestion question)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.SurveyQuestionSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", question.Id);

        var questions = await command.ExecuteQuery(connection, SurveyDataReaders.ReadSurveyQuestionsWithAnswers);
        return questions.FirstOrDefault();
    }
}