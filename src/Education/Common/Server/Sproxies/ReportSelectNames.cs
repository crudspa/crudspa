namespace Crudspa.Education.Common.Server.Sproxies;

public static class ReportSelectNames
{
    public static async Task<IList<Named>> Execute(String connection, Guid? sessionId, Guid? portalId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationCommon.ReportSelectNames";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PortalId", portalId);

        return await command.ReadNameds(connection);
    }
}