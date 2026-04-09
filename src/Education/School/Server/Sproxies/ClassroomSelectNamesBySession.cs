namespace Crudspa.Education.School.Server.Sproxies;

public static class ClassroomSelectNamesBySession
{
    public static async Task<IList<Named>> Execute(String connection, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.ClassroomSelectNamesBySession";

        command.AddParameter("@SessionId", sessionId);

        return await command.ReadNameds(connection);
    }
}