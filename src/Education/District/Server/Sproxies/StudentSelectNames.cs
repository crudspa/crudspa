namespace Crudspa.Education.District.Server.Sproxies;

public static class StudentSelectNames
{
    public static async Task<IList<Named>> Execute(String connection, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationDistrict.StudentSelectNames";

        command.AddParameter("@SessionId", sessionId);

        return await command.ReadNameds(connection);
    }
}