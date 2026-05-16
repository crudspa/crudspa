using Crudspa.Content.Display.Server;

namespace Crudspa.Content.Design.Server.Sproxies;

public static class SurveyQuestionSelectForPart
{
    public static async Task<IList<SurveyQuestion>> Execute(String connection, Guid? sessionId, Guid? partId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.SurveyQuestionSelectForPart";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PartId", partId);

        return await command.ExecuteQuery(connection, SurveyDataReaders.ReadSurveyQuestionsWithAnswers);
    }
}