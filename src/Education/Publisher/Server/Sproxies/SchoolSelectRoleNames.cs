namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class SchoolSelectRoleNames
{
    public static async Task<IList<Named>> Execute(String connection, Guid? sessionId, Guid? schoolId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.SchoolSelectRoleNames";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@SchoolId", schoolId);

        return await command.ReadNameds(connection);
    }
}