using Crudspa.Content.Display.Server;

namespace Crudspa.Content.Design.Server.Sproxies;

public static class SurveyPartSelectForSurvey
{
    public static async Task<IList<SurveyPart>> Execute(String connection, Guid? sessionId, Guid? surveyId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.SurveyPartSelectForSurvey";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@SurveyId", surveyId);

        return await command.ReadAll(connection, SurveyDataReaders.ReadSurveyPart);
    }
}