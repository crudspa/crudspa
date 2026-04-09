namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class DistrictSelectNames
{
    public static async Task<IList<Named>> Execute(String connection, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.DistrictSelectNames";

        command.AddParameter("@SessionId", sessionId);

        return await command.ReadNameds(connection);
    }
}