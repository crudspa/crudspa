using Crudspa.Content.Display.Server;

namespace Crudspa.Content.Design.Server.Sproxies;

public static class SurveySelectForPortal
{
    public static async Task<IList<Survey>> Execute(String connection, Guid? sessionId, Guid? portalId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.SurveySelectForPortal";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PortalId", portalId);

        return await command.ReadAll(connection, SurveyDataReaders.ReadSurvey);
    }
}