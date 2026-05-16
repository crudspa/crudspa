using Crudspa.Content.Display.Server;

namespace Crudspa.Content.Design.Server.Sproxies;

public static class SurveyPartSelect
{
    public static async Task<SurveyPart?> Execute(String connection, Guid? sessionId, SurveyPart part)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.SurveyPartSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", part.Id);

        return await command.ReadSingle(connection, SurveyDataReaders.ReadSurveyPart);
    }
}