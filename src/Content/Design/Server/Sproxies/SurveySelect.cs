using Crudspa.Content.Display.Server;

namespace Crudspa.Content.Design.Server.Sproxies;

public static class SurveySelect
{
    public static async Task<Survey?> Execute(String connection, Guid? sessionId, Survey survey)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.SurveySelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", survey.Id);

        return await command.ReadSingle(connection, SurveyDataReaders.ReadSurvey);
    }
}