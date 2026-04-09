namespace Crudspa.Education.District.Server.Sproxies;

public static class SchoolSelectNames
{
    public static async Task<IList<Named>> Execute(String connection, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationDistrict.SchoolSelectNames";

        command.AddParameter("@SessionId", sessionId);

        return await command.ReadNameds(connection);
    }
}