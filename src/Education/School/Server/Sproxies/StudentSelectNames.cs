namespace Crudspa.Education.School.Server.Sproxies;

public static class StudentSelectNames
{
    public static async Task<IList<Named>> Execute(String connection, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.StudentSelectNames";

        command.AddParameter("@SessionId", sessionId);

        return await command.ReadNameds(connection);
    }
}