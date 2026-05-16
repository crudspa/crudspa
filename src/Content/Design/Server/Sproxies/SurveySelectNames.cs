namespace Crudspa.Content.Design.Server.Sproxies;

public static class SurveySelectNames
{
    public static async Task<IList<Named>> Execute(String connection, Guid? sessionId, Guid? portalId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.SurveySelectNames";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PortalId", portalId);

        return await command.ReadAll<Named>(connection, reader => new()
        {
            Id = reader.ReadGuid(0),
            Name = reader.ReadString(1),
        });
    }
}