namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class SessionLicenseSelectForPublisher
{
    public static async Task<IList<Guid?>> Execute(String connection, Guid? sessionId)
    {
        await using var command = new SqlCommand { CommandText = "EducationPublisher.SessionLicenseSelectForPublisher" };
        command.AddParameter("@SessionId", sessionId);
        return await command.ReadAll(connection, reader => reader.ReadGuid(0));
    }
}