namespace Crudspa.Education.District.Server.Sproxies;

public static class ClassroomSelectNames
{
    public static async Task<IList<Named>> Execute(String connection, Guid? sessionId, Guid? schoolId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationDistrict.ClassroomSelectNames";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@SchoolId", schoolId);

        return await command.ReadNameds(connection);
    }
}