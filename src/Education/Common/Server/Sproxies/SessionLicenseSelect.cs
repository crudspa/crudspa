namespace Crudspa.Education.Common.Server.Sproxies;

public static class SessionLicenseSelect
{
    public static async Task<IList<Guid?>> Execute(String connection, Guid? sessionId)
    {
        await using var command = new SqlCommand { CommandText = "EducationCommon.SessionLicenseSelect" };
        command.AddParameter("@SessionId", sessionId);
        return await command.ReadAll(connection, reader => reader.ReadGuid(0));
    }
}